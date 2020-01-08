using System;
using System.Collections.Generic;
using System.Linq;
using Imms.Data;
using Imms.Mes.Data.Domain;
using Microsoft.EntityFrameworkCore;

namespace Imms.Mes.Services.Kanban.Line
{
    public class DataService : KanbanBaseService
    {
        private SortedList<string, KanbanLineData> _AllLineData = new SortedList<string, KanbanLineData>();
        private SortedList<string, Workorder> _ActiveWorkOrders = new SortedList<string, Workorder>();

        private int MAX_ITEM_COUNT = 4;
        private DateTime _LastDay = DateTime.Now;
        private QueryTrackingBehavior _OldBefahavior;

        public override bool Config()
        {
            this.ServiceId = "LINE_KANBAN_DATA_SERVICE";
            base.Config();

            this._OldBefahavior = this._DbContext.ChangeTracker.QueryTrackingBehavior;
            this._DbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            this.RefreshActiveWorkorders();
            return true;
        }

        protected override void DoInternalThreadProc()
        {
            try
            {
                lock (this)
                {
                    if (DateTime.Now.Day != this._LastDay.Day)
                    {
                        this.CloseCompletedWorkorders();
                        this.RefreshActiveWorkorders();

                        this._LastDay = DateTime.Now;
                    }

                    this.RefreshAllLineData();
                }
            }
            catch (Exception e)
            {
                GlobalConstants.DefaultLogger.Error(this.ServiceId + "处理数据出现错误:" + e.Message);
                GlobalConstants.DefaultLogger.Error(e.StackTrace);
            }
        }

        private void CloseCompletedWorkorders()
        {
            GlobalConstants.DefaultLogger.Info(this.ServiceId + "正开始清理已完成的工单...");
            this._DbContext.ChangeTracker.QueryTrackingBehavior = this._OldBefahavior;
            for (int i = 0; i < this._ActiveWorkOrders.Keys.Count; i++)
            {
                string lineNo = this._ActiveWorkOrders.Keys[i];
                Workorder workorder = this._ActiveWorkOrders[lineNo];
                Workorder dbItem = this._DbContext.Set<Workorder>().Where(x => x.RecordId == workorder.RecordId).Single();
                if (dbItem.QtyGood >= dbItem.QtyReq)
                {
                    dbItem.OrderStatus = Workorder.WORKORDER_SATUS_CLOSED;
                    dbItem.TimeEndActual = DateTime.Now;
                    GlobalConstants.ModifyEntityStatus(dbItem, this._DbContext);
                    this._DbContext.SaveChanges();

                    GlobalConstants.DefaultLogger.Info(this.ServiceId + "已关闭工单:" + dbItem.OrderNo);
                }
            }
            this._DbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            GlobalConstants.DefaultLogger.Info(this.ServiceId + "工单清理已完毕...");
        }

        public string GetLineDataString(string lineNo)
        {
            lock (this)
            {
                if (this._AllLineData.ContainsKey(lineNo))
                {
                    return this._AllLineData[lineNo].ToJson();
                }
            }
            return null;
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
            GlobalConstants.DefaultLogger.Info(this.ServiceId + "正在开始刷新工单...");
            lock (this)
            {
                this._ActiveWorkOrders.Clear();
                this._AllLineData.Clear();

                foreach (Workline line in this._Worklines)
                {
                    Workorder activeWorkorder = this._DbContext.Set<Workorder>()
                             .Where(x => x.LineNo == line.LineCode && x.OrderStatus == Workorder.WOKORDER_STATUS_STARTED)
                             .FirstOrDefault();
                    if (activeWorkorder == null)
                    {
                        continue;
                    }

                    this._ActiveWorkOrders.Add(line.LineCode, activeWorkorder);
                    KanbanLineData lineData = this.CreateLineData(activeWorkorder);

                    WorkshiftSpan[] spanList = this._LineSpans[lineData.line_code].OrderBy(x => x.Seq).ToArray();
                    lineData.line_detail_data = new Detail[spanList.Length];
                    for (int i = 0; i < spanList.Length; i++)
                    {
                        Detail item = new Detail();
                        item.seq = spanList[i].Seq;
                        item.time_begin = spanList[i].TimeBegin;
                        item.time_end = spanList[i].TimeEnd;
                        item.delay_time = spanList[i].DelayTime;
                        item.is_break_item = (spanList[i].IsBreak == 1);
                        item.span_id = spanList[i].RecordId;

                        lineData.line_detail_data[i] = item;
                    }
                }
            }
            GlobalConstants.DefaultLogger.Info(this.ServiceId + "工单刷新完毕...");
        }

        private KanbanLineData CreateLineData(Workorder activeWorkorder)
        {
            KanbanLineData lineData = new KanbanLineData();
            lineData.line_code = activeWorkorder.LineNo;
            Summary summary = new Summary();
            lineData.line_summary_data = summary;
            this._AllLineData.Add(activeWorkorder.LineNo, lineData);

            summary.person_qty = activeWorkorder.WorkerCount;
            summary.production_code = activeWorkorder.PartNo;
            summary.production_name = activeWorkorder.PartName;
            summary.production_order_no = activeWorkorder.OrderNo;
            summary.uph = activeWorkorder.UPH;

            return lineData;
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
            data.current_time = currentTime;
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

            this.FillSpanItems(data);
        }

        private void FillSpanItems(KanbanLineData data)
        {
            string date = data.current_time.ToString("yyyy/MM/dd");
            string lineNo = data.line_code;
            int startSeq = 0;

            WorkshiftSpan breakSpan = this._LineSpans[lineNo].Where(x => x.IsBreak == 1 && DateTime.Parse(date + " " + x.TimeBegin) <= data.current_time)
                .OrderByDescending(x => x.Seq)
                .FirstOrDefault();
            if (breakSpan != null)
            {
                startSeq = breakSpan.Seq;
            }

            List<LineProductSummaryDateSpan> summmaryList = this.GetLineProductSummaryDateSpan(lineNo, data.line_summary_data.production_order_no);
            WorkshiftSpan[] currentSpanList = this._LineSpans[lineNo].Where(x => x.Seq > startSeq).OrderBy(x => x.Seq).Take(this.MAX_ITEM_COUNT).ToArray();
            string firstBeginTime = currentSpanList.OrderBy(x => x.Seq).Select(x => x.TimeBegin).First();
            DateTime firstTime = DateTime.Parse(date + " " + firstBeginTime);

            List<WorkshiftSpan> allSpanList = this._LineSpans[lineNo];
            foreach (WorkshiftSpan span in allSpanList)
            {
                Detail detailItem = data.line_detail_data.Where(x => x.span_id == span.RecordId).Single();
                this.FillOneItem(data, date, firstTime, summmaryList, detailItem, span);
                if (currentSpanList.Where(x => x.RecordId == detailItem.span_id).Count() > 0)
                {
                    detailItem.shown_in_detail_table = true;
                }
                else
                {
                    detailItem.shown_in_detail_table = false;
                }
            }
        }

        private void FillOneItem(KanbanLineData data, string date, DateTime firstTime, List<LineProductSummaryDateSpan> summmaryList, Detail item, WorkshiftSpan span)
        {
            DateTime currentTime = data.current_time;
            DateTime timeStart = DateTime.Parse(date + " " + span.TimeBegin);
            DateTime timeEnd = DateTime.Parse(date + " " + span.TimeEnd);
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