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
        private SortedList<string, DataItem> DataList = new SortedList<string, DataItem>();
        public bool Terminated { get; set; }
        private System.Threading.Thread dataPushThread;
        private DbContext dbContext;
        private string greenLight = "2|0|1|230|0E0100120000220000310000000074";
        private string redLight = "2|0|1|230|0E0100110000220000320000000074";

        public LineKanbanHub()
        {
            this.dbContext = GlobalConstants.DbContextFactory.GetContext();
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

                                   Thread.Sleep(1000 * 10); // 10秒钟发布1次数据
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

                    if (DataList[client.LineNo].line_detail_data == null)
                    {
                        continue;
                    }
                    int hour = Now.Hour;
                    List<LineProductSummaryDateSpan> hourList = this.GetLineProductSummaryDateSpan(client.LineNo, hour, false);
                    if (hourList.Count == 0)
                    {
                        continue;
                    }

                    if (hour > 20)
                    {
                        hour = 20;
                    }
                    if (hour < 8)
                    {
                        hour = 8;
                    }
                    DetailItem detailItem = DataList[client.LineNo].line_detail_data.Where(x => x.hour == hour + 1).Single();
                    // detailItem.qty_plan = hourList.Sum(x => x.QtyPlan);
                    detailItem.qty_good = hourList.Sum(x => x.QtyGood);
                    detailItem.qty_bad = hourList.Sum(x => x.QtyBad);

                    int planQty = (int)(Now.Minute * 1.66);
                    bool toggleLight = false;                    
                    int curLamp = 0;
                    if (detailItem.qty_bad > 0 || (hour > 8 && detailItem.qty_good < planQty))
                    {
                        //如果出现了品质问题或者进度落后，则亮红灯
                        if (client.Lamp != 1)
                        {
                            toggleLight = true;
                            curLamp = 1;                            
                        }
                    }
                    else
                    {
                        //否则亮绿灯
                        if (client.Lamp != 3)
                        {
                            toggleLight = true;
                            curLamp = 3;                            
                        }
                    }

                    if(toggleLight){
                        this.LigthLamp(client,curLamp);
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
            //TODO:插入数据库
            string sql = "insert into DeviceCmdList()"
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
            DataItem theItem;
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
                }
                if (lineNo == null)
                {
                    lineNo = "";
                }
                if (!DataList.ContainsKey(lineNo))
                {
                    theItem = new DataItem();
                    DataList.Add(lineNo, theItem);
                    theItem.line_code = lineNo;
                }
                else
                {
                    theItem = DataList[lineNo];
                }
                this.FillInitedItem(theItem, lineNo);
            }

            this.Clients.Client(id).SendAsync("OnServerData", theItem);
        }

        private void FillInitedItem(DataItem dataItem, string lineNo)
        {
            int[] hours = new int[] { 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
            this.FillWorkorder(dataItem, lineNo);

            dataItem.line_detail_data = new DetailItem[hours.Length];
            DateTime curDate = DateTime.Now;
            List<LineProductSummaryDateSpan> allList = this.GetLineProductSummaryDateSpan(lineNo, curDate.Hour, true);
            for (int i = 0; i < hours.Length; i++)
            {
                DetailItem item = new DetailItem();
                dataItem.line_detail_data[i] = item;
                item.index = i - 1;
                item.qty_plan = 100;
                if (hours[i] == 20) //如果是20:00
                {
                    var dbItem = allList.Where(x => x.SpanId >= 20);
                    item.qty_good = dbItem.Select(x => x.QtyGood).Sum();
                    item.qty_bad = dbItem.Select(x => x.QtyBad).Sum();
                }
                else if (hours[i] == 8)
                {
                    var dbItem = allList.Where(x => x.SpanId < 8);
                    item.qty_good = dbItem.Select(x => x.QtyGood).Sum();
                    item.qty_bad = dbItem.Select(x => x.QtyBad).Sum();
                }
                else
                {
                    item.qty_good = allList.Where(x => x.SpanId == hours[i] - 1).Select(x => x.QtyGood).Sum();
                    item.qty_bad = allList.Where(x => x.SpanId == hours[i] - 1).Select(x => x.QtyBad).Sum();
                }
                item.hour = hours[i];
            }
        }

        private List<LineProductSummaryDateSpan> GetLineProductSummaryDateSpan(string lineNo, int hour, bool all = false)
        {
            DateTime now = DateTime.Now;
            DateTime productDate = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            lock (this)
            {
                if (all)
                {
                    return dbContext.Set<LineProductSummaryDateSpan>().Where(x => x.LineNo == lineNo && x.ProductDate == productDate).ToList();
                }

                var lineNoParam = new SqlParameter("lineNo", lineNo);
                var productDateParam = new SqlParameter("productDate", productDate);
                var spanIdParam = new SqlParameter("spanId", hour);
                string sql = "select * from mes_line_product_summary_datespan where line_no = @lineNo and product_date = @productDate and span_id=@spanId";
                if (hour >= 20)
                {
                    sql = "select * from mes_line_product_summary_datespan where line_no = @lineNo and product_date = @productDate and span_id >=20";
                }
                else if (hour < 8)
                {
                    sql = "select * from mes_line_product_summary_datespan where line_no = @lineNo and product_date = @productDate and span_id <8";
                }

                List<LineProductSummaryDateSpan> result = new List<LineProductSummaryDateSpan>();
                using (RelationalDataReader reader = dbContext.Database.ExecuteSqlQuery(sql, lineNoParam, productDateParam, spanIdParam))
                {
                    while (reader.DbDataReader.Read())
                    {
                        LineProductSummaryDateSpan item = new LineProductSummaryDateSpan();
                        item.LineNo = reader.DbDataReader["line_no"].ToString();
                        item.PartNo = reader.DbDataReader["part_no"].ToString();
                        item.ProductDate = (DateTime)reader.DbDataReader["product_date"];
                        item.QtyBad = (int)reader.DbDataReader["qty_bad"];
                        item.QtyGood = (int)reader.DbDataReader["qty_good"];
                        item.QtyPlan = (int)reader.DbDataReader["qty_plan"];
                        item.SpanId = (int)reader.DbDataReader["span_id"];

                        result.Add(item);
                    }
                }

                return result;
            }
        }

        private void FillWorkorder(DataItem dataItem, string lineNo)
        {
            dataItem.line_code = lineNo;
            Workorder activeWorkOrder = this.GetLineActiveWorkorder(lineNo);
            if (activeWorkOrder == null)
            {
                return;
            }
            dataItem.line_summary_data = new SummaryItem();
            dataItem.line_summary_data.production_code = activeWorkOrder.PartNo;
            dataItem.line_summary_data.production_name = activeWorkOrder.PartNo;
            dataItem.line_summary_data.production_order_no = activeWorkOrder.OrderNo;
            dataItem.line_summary_data.uph = 30;
            dataItem.line_summary_data.person_qty = this.GetLineOperatorCount(lineNo);
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

                return this.dbContext.Set<Workorder>().Where(x => x.OrderNo == orderNo).FirstOrDefault();
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

    public class DataItem
    {
        public string line_code { get; set; }
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
        public int index { get; set; }
        public int qty_plan { get; set; }
        public int qty_good { get; set; }
        public int qty_bad { get; set; }
    }
}