using System;
using System.Collections.Generic;
using System.Linq;
using Imms.Mes.Data.Domain;
using Microsoft.EntityFrameworkCore;

namespace Imms.Mes.Services.Kanban.Line
{
    public class DataService : KanbanBaseService
    {
        private SortedList<string, KanbanLineData> _AllLineData = new SortedList<string, KanbanLineData>();
        private SortedList<string, Workorder> _ActiveWorkOrders = new SortedList<string, Workorder>();

        private int MAX_ITEM_COUNT = 4;

        public override bool Config()
        {
            base.Config();

            this._DbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;            
            this.RefreshActiveWorkorders();
            return true;
        }

        protected override void DoInternalThreadProc()
        {
            lock (this)
            {
                this.RefreshAllLineData();
            }
        }

        public KanbanLineData GetLineData(string lineNo)
        {
            lock (this)
            {
                if (this._AllLineData.ContainsKey(lineNo))
                {
                    return this._AllLineData[lineNo];
                }
            }
            return null;
        }

        public void RefreshActiveWorkorders()
        {
            lock (this)
            {
                this._ActiveWorkOrders.Clear();
                this._AllLineData.Clear();

                foreach (Workline line in this._Worklines)
                {
                    Workorder activeWorkorder = this._DbContext.Set<Workorder>().Where(x => x.LineNo == line.LineCode && x.OrderStatus == Workorder.WOKORDER_STATUS_STARTED).FirstOrDefault();
                    if (activeWorkorder != null)
                    {
                        this._ActiveWorkOrders.Add(line.LineCode, activeWorkorder);

                        KanbanLineData lineData = new KanbanLineData();
                        lineData.line_code = line.LineCode;
                        Summary summary = new Summary();
                        lineData.line_summary_data = summary;
                        this._AllLineData.Add(line.LineCode, lineData);

                        summary.person_qty = this._DbContext.Set<Operator>().Where(x => x.orgCode == line.LineCode).Count();
                        summary.production_code = activeWorkorder.PartNo;
                        summary.production_name = activeWorkorder.PartName;
                        summary.production_order_no = activeWorkorder.OrderNo;
                        summary.uph = activeWorkorder.UPH;
                    }
                }
            }
        }

        private void RefreshAllLineData()
        {
            foreach (string lineCode in this._AllLineData.Keys)
            {
                this.RefreshOneLineData(lineCode);
            }
        }

        private void RefreshOneLineData(string lineNo)
        {
            KanbanLineData data = this._AllLineData[lineNo];
            DateTime currentTime = DateTime.Now;
            string date = currentTime.ToString("yyyy/MM/dd");
            data.is_break = (this._LineSpans[lineNo].Where(x => x.IsBreak == 0
                                        && DateTime.Parse(date + " " + x.TimeBegin) <= currentTime
                                        && DateTime.Parse(date + " " + x.TimeEnd) >= currentTime)
                                        .Count() == 0)   // 处于休息时间中
                            ;
            if (data.is_break)
            {
                return;
            }

            int startSeq = 0;
            WorkshiftSpan breakSpan = this._LineSpans[lineNo].Where(x => x.IsBreak == 1 && DateTime.Parse(date + " " + x.TimeBegin) <= currentTime)
                .OrderByDescending(x => x.Seq)
                .FirstOrDefault();
            if (breakSpan != null)
            {
                startSeq = breakSpan.Seq;
            }
            WorkshiftSpan[] spanList = this._LineSpans[lineNo].Where(x => x.Seq > startSeq).OrderBy(x => x.Seq).Take(this.MAX_ITEM_COUNT).ToArray();
            string firstBeginTime = spanList.OrderBy(x => x.Seq).Select(x => x.TimeBegin).First();
            DateTime firstTime = DateTime.Parse(date + " " + firstBeginTime);

            data.line_detail_data = new Detail[this.MAX_ITEM_COUNT];
            List<LineProductSummaryDateSpan> summmaryList = this.GetLineProductSummaryDateSpan(lineNo, data.line_summary_data.production_order_no);
            for (int i = 0; i < this.MAX_ITEM_COUNT; i++)
            {
                Detail item = new Detail();
                WorkshiftSpan span = spanList[i];
                DateTime timeStart = DateTime.Parse(date + " " + span.TimeBegin);
                DateTime timeEnd = DateTime.Parse(date + " " + span.TimeEnd);

                data.line_detail_data[i] = item;
                item.seq = span.Seq;
                int secconds = (int)currentTime.Subtract(timeStart).TotalSeconds;
                if (currentTime >= timeStart && currentTime < timeEnd)
                {
                    item.is_current_item = true;
                    item.qty_plan = (int)(secconds * (data.line_summary_data.uph / 3600F));
                }
                else
                {
                    item.is_current_item = false;
                    if (currentTime < firstTime)
                    {
                        item.is_current_item = true;
                    }

                    if (currentTime < timeStart)
                    {
                        item.qty_plan = 0;
                    }
                    else
                    {
                        item.qty_plan = data.line_summary_data.uph;
                    }
                }
                item.qty_good = summmaryList.Where(x => x.SpanId == span.RecordId).Select(x => x.QtyGood).Sum();
                item.qty_bad = summmaryList.Where(x => x.SpanId == span.RecordId).Select(x => x.QtyBad).Sum();
                item.time_begin = span.TimeBegin;
                item.time_end = span.TimeEnd;
                item.delay_time = span.DelayTime;
            }
        }

        private List<LineProductSummaryDateSpan> GetLineProductSummaryDateSpan(string lineNo, string workorderNo)
        {
            DateTime productDate = DateTime.Now.Date;
            return this._DbContext.Set<LineProductSummaryDateSpan>().Where(
                            x => x.LineNo == lineNo
                            && x.WorkorderNo == workorderNo
                            && x.ProductDate == productDate)
                    .ToList();
        }
    }
}