using Imms.Data;
using Imms.Mes.Data.Domain;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
namespace Imms.WebManager
{
    public class KanbanRealtimeHub : Hub
    {
        public static SortedList<string, WebSocketClient> realTimeConnectedIdList = new SortedList<string, WebSocketClient>();
        public static RealtimeItem demoRealtimeItem = new RealtimeItem();

        public KanbanRealtimeHub()
        {
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            string id = Context.ConnectionId;
            lock (KanbanRealtimeHub.realTimeConnectedIdList)
            {
                if (KanbanRealtimeHub.realTimeConnectedIdList.ContainsKey(id))
                {
                    KanbanRealtimeHub.realTimeConnectedIdList.Remove(id);
                }
            }
            return base.OnDisconnectedAsync(exception);
        }

        public void RegisterRealtimeClient()
        {
            string id = Context.ConnectionId;
            lock (KanbanRealtimeHub.realTimeConnectedIdList)
            {
                if (!KanbanRealtimeHub.realTimeConnectedIdList.ContainsKey(id))
                {
                    WebSocketClient client = new WebSocketClient();
                    client.ConnectionId = id;
                    client.Inited = false;

                    KanbanRealtimeHub.realTimeConnectedIdList.Add(id, client);
                }
            }

            this.Clients.Client(id).SendAsync("PushRealtimeData", demoRealtimeItem);
        }
    }

    public class WebSocketClient
    {
        public string ConnectionId { get; set; }
        public bool Inited { get; set; }
    }

    public class RealtimeDataPushTask
    {
        private readonly IHubContext<KanbanRealtimeHub> _hubContext;
        private System.Threading.Thread dataPushThread;
        private readonly Random random = new Random(100);
        private int lastRecordCount = 0;
        private int[] hours = new int[] { 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
        private DbContext dbContext;

        public bool Terminated { get; set; }

        public RealtimeDataPushTask(IHubContext<KanbanRealtimeHub> hubContext)
        {
            this._hubContext = hubContext;

            this.dbContext = GlobalConstants.DbContextFactory.GetContext();
            this.Terminated = false;
            this.InitDemoData();
        }

        private void InitDemoData()
        {
            KanbanRealtimeHub.demoRealtimeItem.line_code = "A301-2";
            KanbanRealtimeHub.demoRealtimeItem.line_summary_data = new SummaryItem();
            KanbanRealtimeHub.demoRealtimeItem.line_summary_data.production_code = "AL666-ACC-01M";
            KanbanRealtimeHub.demoRealtimeItem.line_summary_data.production_name = "测试品名";
            KanbanRealtimeHub.demoRealtimeItem.line_summary_data.production_order_no = "WO-20191002-001";
            KanbanRealtimeHub.demoRealtimeItem.line_summary_data.uph = 30;
            KanbanRealtimeHub.demoRealtimeItem.line_summary_data.person_qty = 6;

            KanbanRealtimeHub.demoRealtimeItem.line_detail_data = new DetailItem[this.hours.Length];
            int hour = DateTime.Now.Hour;
            List<LineProductSummaryDateSpan> allList = this.GetLineProductSummaryDateSpan(hour, true);

            for (int i = 0; i < this.hours.Length; i++)
            {
                DateTime date = DateTime.Now;

                DetailItem item = new DetailItem();
                KanbanRealtimeHub.demoRealtimeItem.line_detail_data[i] = item;
                item.index = i - 1;
                item.qty_plan = 100;
                if (date.Hour > this.hours[i] && date.Hour != this.hours[0])
                {
                    var dbItem = allList.Where(x => x.SpanId + 1 == this.hours[i]);
                    // item.qty_good = dbItem.Select(x => x.GoodQty).Sum();
                    // item.qty_bad = dbItem.Select(x => x.BadQty).Sum();
                }
                else
                {
                    item.qty_good = 0;
                    item.qty_bad = 0;
                }
                item.hour = this.hours[i];
            }
        }

        public void Start()
        {
            if (dataPushThread == null)
            {
                dataPushThread = new System.Threading.Thread(() =>
                           {
                               while (!Terminated)
                               {
                                   this.PushThreadHandler();
                                   Thread.Sleep(1000); // 1秒钟发布1次数据
                               }
                           });
                dataPushThread.Start();
            }
        }

        private void PushThreadHandler()
        {
            this.PushRealtimeData();
        }

        public void PushRealtimeData()
        {
            if (KanbanRealtimeHub.realTimeConnectedIdList.Count == 0)
            {
                return;
            }
            int hour = DateTime.Now.Hour;
            List<LineProductSummaryDateSpan> hourList = this.GetLineProductSummaryDateSpan(hour);
            if (lastRecordCount == hourList.Count)
            {
                return;
            }
            lastRecordCount = hourList.Count;
            DetailItem detailItem = KanbanRealtimeHub.demoRealtimeItem.line_detail_data.Where(x => x.hour == DateTime.Now.Hour + 1).Single();

            lock (KanbanRealtimeHub.realTimeConnectedIdList)
            {
                foreach (string id in KanbanRealtimeHub.realTimeConnectedIdList.Keys)
                {
                    _hubContext.Clients.Client(id).SendAsync("PushRealtimeData", KanbanRealtimeHub.demoRealtimeItem);
                }
            }
        }

        public List<LineProductSummaryDateSpan> GetLineProductSummaryDateSpan(int hour, bool all = false)
        {
            DateTime now = DateTime.Now;
            DateTime begin = new DateTime(now.Year, now.Month, now.Day, hour, 0, 0);
            DateTime end = begin.AddHours(1);
            if (all)
            {
                begin = new DateTime(now.Year, now.Month, now.Day);
                end = begin.AddDays(1);
            }
            List<LineProductSummaryDateSpan> result = dbContext.Set<LineProductSummaryDateSpan>().Where(x => x.ProductDate >= begin && x.ProductDate < end).ToList();
            return result;
        }
    }

    public class RealtimeItem
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