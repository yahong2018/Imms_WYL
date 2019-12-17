using Imms.Data;
using Imms.Mes.Data.Domain;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Imms.WebManager
{
    public class LineKanbanHub : Hub
    {
        private SortedList<string, WebSocketClient> ConnectedIdList = new SortedList<string, WebSocketClient>();
        private SortedList<string, KanbanLineData> DataList = new SortedList<string, KanbanLineData>();
        private SortedList<string, string> _firstSpanList = new SortedList<string, string>();
        public bool Terminated { get; set; }
        private System.Threading.Thread dataPushThread;
        private DbContext dbContext;
        public int MAX_DETAIL_ITEM_COUNT { get; set; }
        public int SKIP_MINUTE { get; set; }
        private SortedList<string, List<WorkshiftSpan>> LineSpanList = new SortedList<string, List<WorkshiftSpan>>();
        private DateTime lastBadCheckTime = new DateTime(9999,12,31);
        private DateTime Yestoday = DateTime.Now.Date;

        private const int LIGHT_GREEN = 3;
        private const int LIGHT_YELLOW = 2;
        private const int LIGHT_RED = 1;

        public LineKanbanHub()
        {
            this.dbContext = GlobalConstants.DbContextFactory.GetContext();
            this.dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            this.MAX_DETAIL_ITEM_COUNT = 4;
            this.SKIP_MINUTE = 60;

            this.RefreshSpanList();
            this.RefreshWorkshopData();
        }

        public void RefreshWorkshopData()
        {
            List<Workshop> workshopList = this.dbContext.Set<Workshop>().ToList();
            List<Workline> worklineList = this.dbContext.Set<Workline>().ToList();
            foreach (Workshop workshop in workshopList)
            {
                foreach (Workline line in worklineList.Where(x => x.ParentId == workshop.RecordId))
                {
                    if (!DataList.ContainsKey(line.LineCode))
                    {
                        KanbanLineData theItem = new KanbanLineData();
                        DataList.Add(line.LineCode, theItem);
                        theItem.line_code = line.LineCode;
                        theItem.GID = line.GID;
                        theItem.DID = line.DID;
                    }
                }
            }
        }

        public void RefreshSpanList()
        {
            List<WorkshiftSpan> allSpanList = this.dbContext.Set<WorkshiftSpan>().ToList();
            List<Workshop> workshopList = this.dbContext.Set<Workshop>().ToList();
            List<Workline> worklineList = this.dbContext.Set<Workline>().ToList();
            foreach (Workshop workshop in workshopList)
            {
                long workshiftId = dbContext.Set<Workshift>().Where(x => x.ShiftCode == workshop.WorkshiftCode).Select(x => x.RecordId).Single();
                foreach (Workline line in worklineList.Where(x => x.ParentId == workshop.RecordId))
                {
                    this.LineSpanList.Add(line.LineCode, allSpanList.Where(x => x.WorkshiftId == workshiftId).ToList());

                    this._firstSpanList.Add(line.LineCode, this.LineSpanList[line.LineCode].OrderBy(x => x.Seq).First().TimeBegin);
                }
            }
            this.lastBadCheckTime = new DateTime(9999, 12, 31);
        }

        public void Start()
        {
            if (dataPushThread == null)
            {
                dataPushThread = new System.Threading.Thread(() =>
                    {
                        while (!Terminated)
                        {
                            try
                            {
                                if (Yestoday.Day != DateTime.Now.Day)
                                {
                                    this.RefreshSpanList();
                                }

                                this.PushData();
                            }
                            catch (Exception e)
                            {
                                GlobalConstants.DefaultLogger.Error(e.Message);
                                GlobalConstants.DefaultLogger.Debug(e.StackTrace);
                            }

                            Thread.Sleep(1000 * 1); // 1秒钟发布1次数据
                        }
                    });
                dataPushThread.Start();
            }
        }

        private void PushData()
        {
            if (this.ConnectedIdList.Count == 0)
            {
                return;
            }

            lock (this)
            {
                if (this.ConnectedIdList.Count == 0)
                {
                    return;
                }

                DateTime Now = DateTime.Now;

                for (int i = 0; i < this.ConnectedIdList.Count; i++)
                {
                    WebSocketClient client = this.ConnectedIdList.Values[i];
                    if (string.IsNullOrEmpty(client.LineNo))
                    {
                        continue;
                    }
                    KanbanLineData kabanData = DataList[client.LineNo];
                    this.FillDetailItems(kabanData, client.LineNo);
                    if (kabanData.is_break)
                    {
                        continue;
                    }

                    if (client.MustLight)
                    {
                        this.LigthLamp(kabanData);
                    }
                }

                foreach (string id in this.ConnectedIdList.Keys)
                {
                    if (string.IsNullOrEmpty(this.ConnectedIdList[id].LineNo))
                    {
                        continue;
                    }

                    this.Clients.Client(id).SendAsync("OnServerData", DataList[this.ConnectedIdList[id].LineNo]);
                }
            }
        }

        private void LigthLamp(KanbanLineData kabanData)
        {
            DetailItem detailItem = kabanData.line_detail_data.Where(x => x.is_current_item).First();
            string firstBeginTime = this._firstSpanList[kabanData.line_code];
                //kabanData.line_detail_data.OrderBy(x => x.time_begin).Select(x => x.time_begin).First();
            DateTime firstDateTime = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd") + " " + firstBeginTime).AddMinutes(this.SKIP_MINUTE);

            int planQty = detailItem.qty_plan;
            if (detailItem.qty_good < planQty)
            {
                DateTime currentTime = DateTime.Now;
                if (currentTime > firstDateTime && this.lastBadCheckTime > currentTime){
                    //第一个小时，不亮红灯
                    this.lastBadCheckTime = currentTime;
                }
                if(currentTime.Subtract(this.lastBadCheckTime).TotalMinutes >= 15)
                {
                    //正常一直亮绿灯，如果15分钟持续不达标，则亮红灯。
                    this.lastBadCheckTime = DateTime.Now;
                    this.DoLight(kabanData, LIGHT_RED);
                }else if (kabanData.Lamp != LIGHT_GREEN)
                {
                    this.DoLight(kabanData, LIGHT_GREEN);
                }
            }
            else //否则亮绿灯
            {
                if (kabanData.Lamp != LIGHT_GREEN)
                {
                    this.DoLight(kabanData, LIGHT_GREEN);
                }
            }
        }

        private void DoLight(KanbanLineData kabanData, int curLamp)
        {
            kabanData.Lamp = curLamp;
            var gidParam = new SqlParameter("GID", kabanData.GID);
            var didParam = new SqlParameter("DID", kabanData.DID);
            var lampParam = new SqlParameter("Lamp", kabanData.Lamp);
            this.dbContext.Database.ExecuteSqlCommand("MES_Light @GID,@DID,@Lamp", gidParam, didParam, lampParam);
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            string id = Context.ConnectionId;
            lock (this)
            {
                if (this.ConnectedIdList.ContainsKey(id))
                {
                    this.ConnectedIdList.Remove(id);
                }
            }
            return base.OnDisconnectedAsync(exception);
        }

        public void RegisterWebClient(string lineNo, string mustLight)
        {
            string id = Context.ConnectionId;

            lock (this)
            {
                if (!this.ConnectedIdList.ContainsKey(id))
                {
                    WebSocketClient client = new WebSocketClient();
                    client.ConnectionId = id;
                    client.LineNo = lineNo;
                    if (mustLight == "999")
                    {
                        client.MustLight = true;
                    }
                    this.ConnectedIdList.Add(id, client);
                    if (DataList.ContainsKey(lineNo))
                    {
                        KanbanLineData theItem = DataList[lineNo];
                        this.Clients.Client(id).SendAsync("OnServerData", theItem);
                    }
                }
            }
        }

        private void FillDetailItems(KanbanLineData lineKanbanItem, string lineNo)
        {
            DateTime currentTime = DateTime.Now;
            string date = currentTime.ToString("yyyy/MM/dd");
            bool isBreak = (this.LineSpanList[lineNo].Where(x => x.IsBreak == 0
                                        && DateTime.Parse(date + " " + x.TimeBegin) <= currentTime
                                        && DateTime.Parse(date + " " + x.TimeEnd) >= currentTime)
                                        .Count() == 0)   // 处于休息时间中
                            ;
            if (isBreak)
            {
                lineKanbanItem.is_break = true;
                return;
            }

            this.FillWorkorder(lineKanbanItem, lineNo);

            int startSeq = 0;
            WorkshiftSpan breakSpan = this.LineSpanList[lineNo].Where(x => x.IsBreak == 1 && DateTime.Parse(date + " " + x.TimeBegin) <= currentTime)
                .OrderByDescending(x => x.Seq)
                .FirstOrDefault();
            if (breakSpan != null)
            {
                startSeq = breakSpan.Seq;
            }
            WorkshiftSpan[] spanList = this.LineSpanList[lineNo].Where(x => x.Seq > startSeq).OrderBy(x => x.Seq).Take(this.MAX_DETAIL_ITEM_COUNT).ToArray();
            string firstBeginTime = spanList.OrderBy(x => x.Seq).Select(x => x.TimeBegin).First();
            DateTime firstTime = DateTime.Parse(date + " " + firstBeginTime);

            lineKanbanItem.line_detail_data = new DetailItem[this.MAX_DETAIL_ITEM_COUNT];
            List<LineProductSummaryDateSpan> summmaryList = this.GetLineProductSummaryDateSpan(lineNo, lineKanbanItem.line_summary_data.production_order_no);
            for (int i = 0; i < this.MAX_DETAIL_ITEM_COUNT; i++)
            {
                DetailItem item = new DetailItem();
                lineKanbanItem.line_detail_data[i] = item;
                WorkshiftSpan span = spanList[i];
                DateTime timeStart = DateTime.Parse(date + " " + span.TimeBegin);
                DateTime timeEnd = DateTime.Parse(date + " " + span.TimeEnd);
                int secconds = (int)currentTime.Subtract(timeStart).TotalSeconds;
                if (currentTime >= timeStart && currentTime < timeEnd)
                {
                    item.is_current_item = true;
                    item.qty_plan = secconds * (3600 /lineKanbanItem.line_summary_data.uph);
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
                        item.qty_plan = lineKanbanItem.line_summary_data.uph;
                    }
                }
                item.qty_good = summmaryList.Where(x => x.SpanId == span.RecordId).Select(x => x.QtyGood).Sum();
                item.qty_bad = summmaryList.Where(x => x.SpanId == span.RecordId).Select(x => x.QtyBad).Sum();
                item.time_begin = span.TimeBegin;
                item.time_end = span.TimeEnd;
            }
        }

        private List<LineProductSummaryDateSpan> GetLineProductSummaryDateSpan(string lineNo, string workorderNo)
        {
            DateTime productDate = DateTime.Now.Date;
            lock (this)
            {
                return dbContext.Set<LineProductSummaryDateSpan>().Where(
                                x => x.LineNo == lineNo
                                && x.WorkorderNo == workorderNo
                                && x.ProductDate == productDate)
                        .ToList();
            }
        }

        private void FillWorkorder(KanbanLineData lineKanbanData, string lineNo)
        {
            lineKanbanData.line_code = lineNo;
            Workorder activeWorkOrder = this.GetLineActiveWorkorder(lineNo);
            if (activeWorkOrder == null)
            {
                return;
            }
            lineKanbanData.line_summary_data = new SummaryItem();
            lineKanbanData.line_summary_data.production_code = activeWorkOrder.PartNo;
            lineKanbanData.line_summary_data.production_name = activeWorkOrder.PartNo;
            lineKanbanData.line_summary_data.production_order_no = activeWorkOrder.OrderNo;
            lineKanbanData.line_summary_data.person_qty = this.GetLineOperatorCount(lineNo);
            lineKanbanData.plan_qty = activeWorkOrder.QtyReq;
            lineKanbanData.time_start_plan = activeWorkOrder.TimeStartPlan;
            lineKanbanData.time_end_plan = activeWorkOrder.TimeEndPlan;
            lineKanbanData.line_summary_data.uph = activeWorkOrder.UPH;
        }

        private int GetLineOperatorCount(string lineNo)
        {
            lock (this)
            {
                return this.dbContext.Set<Operator>().Where(x => x.orgCode == lineNo).Count();
            }
        }

        private Workorder GetLineActiveWorkorder(string lineNo)
        {
            lock (this)
            {
                string orderNo = this.dbContext.Set<ActiveWorkorder>().Where(x => x.LineNo == lineNo).Select(x => x.WorkorderNo).FirstOrDefault();
                if (string.IsNullOrEmpty(orderNo))
                {
                    return null;
                }
                return this.dbContext.Set<Workorder>().First(x => x.OrderNo == orderNo);
            }
        }
    }

    public class WebSocketClient
    {
        public string ConnectionId { get; set; }
        public string LineNo { get; set; }
        public bool MustLight { get; set; }
    }

    public class KanbanLineData
    {
        public string line_code { get; set; }
        public int GID { get; set; }
        public int DID { get; set; }
        public int Lamp { get; set; } // 1.红灯   2.黄灯  3.绿灯

        public int plan_qty { get; set; }
        public bool is_break { get; set; }

        public DateTime time_start_plan { get; set; }
        public DateTime time_end_plan { get; set; }

        public SummaryItem line_summary_data { get; set; }
        public DetailItem[] line_detail_data { get; set; }
    }

    public class SummaryItem
    {
        public string production_code { get; set; }
        public string production_name { get; set; }
        public string production_order_no { get; set; }
        public int uph { get; set; }
        public int person_qty { get; set; }
    }

    public class DetailItem
    {
        public string time_begin { get; set; }
        public string time_end { get; set; }
        public int qty_plan { get; set; }
        public int qty_good { get; set; }
        public int qty_bad { get; set; }
        public bool is_current_item { get; set; }
    }
}