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
        public bool Terminated { get; set; }
        private System.Threading.Thread dataPushThread;
        private DbContext dbContext;

        public LineKanbanHub()
        {
            this.dbContext = GlobalConstants.DbContextFactory.GetContext();
            this.dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
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
                    DetailItem detailItem = kabanData.line_detail_data[0];

                    int planQty = detailItem.qty_plan;
                    bool toggleLight = false;
                    int curLamp = 0;
                    if (detailItem.qty_bad > 0 || (DateTime.Now.Hour > 8 && detailItem.qty_good < planQty))
                    {//如果出现了品质问题或者进度落后，则亮红灯
                        if (client.Lamp != 1)
                        {
                            toggleLight = true;
                            curLamp = 1;
                        }
                    }
                    else //否则亮绿灯
                    {
                        if (client.Lamp != 3)
                        {
                            toggleLight = true;
                            curLamp = 3;
                        }
                    }

                    if (toggleLight)
                    {
                        this.LigthLamp(client, curLamp);
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

        private void LigthLamp(WebSocketClient client, int curLamp)
        {
            client.Lamp = curLamp;
            var gidParam = new SqlParameter("GID", client.GID);
            var didParam = new SqlParameter("DID", client.DID);
            var lampParam = new SqlParameter("Lamp", client.Lamp);
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

        public void RegisterClient(string lineNo)
        {
            string id = Context.ConnectionId;
            KanbanLineData theItem;
            lock (this)
            {
                if (!this.ConnectedIdList.ContainsKey(id))
                {
                    WebSocketClient client = new WebSocketClient();
                    client.ConnectionId = id;
                    client.LineNo = lineNo;

                    Workline theLine = this.dbContext.Set<Workline>().Where(x => x.LineCode == lineNo).First();
                    client.GID = theLine.GID;
                    client.DID = theLine.DID;

                    this.ConnectedIdList.Add(id, client);

                    if ((DateTime.Now.Hour == 8) || (DateTime.Now.Hour == 20))  // 如果现在是在在班次的第1个小时内
                    {
                        this.LigthLamp(client, 1);
                    }
                }
                if (lineNo == null)
                {
                    lineNo = "";
                }
                if (!DataList.ContainsKey(lineNo))
                {
                    theItem = new KanbanLineData();
                    DataList.Add(lineNo, theItem);
                    theItem.line_code = lineNo;
                }
                else
                {
                    theItem = DataList[lineNo];
                }
                this.FillDetailItems(theItem, lineNo);
            }

            this.Clients.Client(id).SendAsync("OnServerData", theItem);
        }

        private void FillDetailItems(KanbanLineData lineKanbanItem, string lineNo)
        {
            int[] hours = new int[5];
            DateTime currentTime = DateTime.Now;
            for (int i = 0; i < hours.Length; i++)
            {
                int nHour = (currentTime.Hour - i);
                if (nHour < 0)
                {
                    nHour = 24 + nHour;
                }
                hours[i] = nHour;
            }
            this.FillWorkorder(lineKanbanItem, lineNo);

            lineKanbanItem.line_detail_data = new DetailItem[hours.Length];
            List<LineProductSummaryDateSpan> allList = this.GetLineProductSummaryDateSpan(lineNo, hours[0], hours[4]);
            for (int i = 0; i < hours.Length; i++)
            {
                DetailItem item = new DetailItem();
                lineKanbanItem.line_detail_data[i] = item;
                if (i == 0)
                {
                    int qtyPlan = allList.Where(x => x.SpanId == hours[i]).Select(x => x.QtyPlan).Sum();
                    if (qtyPlan == 0)
                    {
                        qtyPlan = lineKanbanItem.plan_qty;
                    }
                    item.qty_plan = (int)((currentTime.Minute + 1) * (qtyPlan / 60));
                }
                else
                {
                    item.qty_plan = allList.Where(x => x.SpanId == hours[i]).Select(x => x.QtyPlan).Sum();
                    if(item.qty_plan==0 && lineKanbanItem.time_start_actual.HasValue){
                        item.qty_plan = lineKanbanItem.plan_qty;
                    }
                }

                item.qty_good = allList.Where(x => x.SpanId == hours[i]).Select(x => x.QtyGood).Sum();
                item.qty_bad = allList.Where(x => x.SpanId == hours[i]).Select(x => x.QtyBad).Sum();

                item.hour = hours[i];
            }
        }

        private List<LineProductSummaryDateSpan> GetLineProductSummaryDateSpan(string lineNo, int hourEnd, int hourStart)
        {
            DateTime now = DateTime.Now;
            DateTime productDate = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            lock (this)
            {
                if (hourEnd > hourStart)
                {
                    return dbContext.Set<LineProductSummaryDateSpan>().Where(
                                    x => x.LineNo == lineNo
                                    && x.ProductDate == productDate
                                    && (x.SpanId >= hourStart && x.SpanId <= hourEnd))
                            .ToList();
                }
                else
                {
                    DateTime prevDate = productDate.AddDays(-1);
                    IQueryable<LineProductSummaryDateSpan> query = dbContext.Set<LineProductSummaryDateSpan>().Where(
                            x => x.LineNo == lineNo
                            && ((x.ProductDate == productDate && x.SpanId <= hourEnd)
                               || (x.ProductDate == prevDate && x.SpanId >= hourStart)
                            )
                        );
                    return query.ToList();
                }
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
            if (lineKanbanData.line_summary_data.person_qty == 0)
            {
                lineKanbanData.line_summary_data.person_qty = 4;
            }
            lineKanbanData.line_summary_data.uph = (activeWorkOrder.QtyBad + activeWorkOrder.QtyGood) / lineKanbanData.line_summary_data.person_qty;

            lineKanbanData.plan_qty = activeWorkOrder.QtyReq;
            int hours = activeWorkOrder.TimeEndPlan.Subtract(activeWorkOrder.TimeStartPlan).Hours;
            if (hours > 0)
            {
                lineKanbanData.plan_qty = lineKanbanData.plan_qty / hours;
            }
            lineKanbanData.time_start_actual = activeWorkOrder.TimeStartActual;
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
        public int GID { get; set; }
        public int DID { get; set; }
        public int Lamp { get; set; } // 1.红灯   2.黄灯  3.绿灯
    }

    public class KanbanLineData
    {
        public string line_code { get; set; }
        public int plan_qty { get; set; }
        public DateTime? time_start_actual { get; set; }
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
        public int hour { get; set; }
        public int qty_plan { get; set; }
        public int qty_good { get; set; }
        public int qty_bad { get; set; }
    }
}