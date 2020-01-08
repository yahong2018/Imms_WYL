using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Imms.Mes.Data.Domain;
using Microsoft.EntityFrameworkCore;

namespace Imms.Mes.Services.Kanban.Workshop
{
    public class WorkshopKanbaData
    {
        public string WorkshopCode { get; set; }
        public string Customer { get; set; }
        public bool IsBreak { get; set; }
        public int WorkHours { get; set; }
        public string CurrentTime { get; set; }
        public LineSummaryData[] LineSummaryItems { get; set; }
    }

    public class LineSummaryData
    {
        public string LineNo { get; set; }
        public string PartNo { get; set; }
        public int DayTotalTarget { get; set; }//当日计划
        public int DayCurrentTarget { get; set; }//当日目标
        public int DayFinished { get; set; } //当日完工
        public int CurrentTarget { get; set; }  //本小时目标
        public int CurrentFinished { get; set; } //本小时完工
        public int CurrentBad { get; set; }//本小时不良
    }

    public class WorkshopDataService : BaseService
    {
        private DbContext _DbContext;
        private DateTime _LastDay = DateTime.Now;
        private QueryTrackingBehavior _OldBefahavior;
        private SortedList<string, List<Imms.Mes.Data.Domain.WorkshiftSpan>> _WorkshiftSpanList = new SortedList<string, List<Data.Domain.WorkshiftSpan>>();
        private List<Imms.Mes.Data.Domain.Workshop> _WorkshopList = new List<Imms.Mes.Data.Domain.Workshop>();
        private List<Imms.Mes.Data.Domain.Workline> _WorklineList = new List<Data.Domain.Workline>();
        private List<Data.Domain.Workorder> _ActiveWorkOrderList = new System.Collections.Generic.List<Data.Domain.Workorder>();
        private SortedList<string, WorkshopKanbaData> _WorkshopDataList = new SortedList<string, WorkshopKanbaData>();

        internal string GetData(string workshopCode)
        {
            lock (this)
            {
                if (_WorkshopDataList.ContainsKey(workshopCode))
                {
                    return _WorkshopDataList[workshopCode].ToJson();
                }
            }
            return null;
        }

        protected override void DoInternalThreadProc()
        {
            lock (this)
            {
                try
                {
                    if (this._LastDay.Day != DateTime.Now.Day)
                    {
                        this._WorkshopDataList.Clear();

                        this.RefreshOrgData();
                        this.RefreshWorkorder();

                        this._LastDay = DateTime.Now;
                    }
                    this.RefreshKanbanData();
                }
                catch (Exception e)
                {
                    GlobalConstants.DefaultLogger.Error(this.ServiceId + "处理数据出现异常:" + e.Message);
                    GlobalConstants.DefaultLogger.Error(e.StackTrace);
                }
            }
        }

        private void RefreshKanbanData()
        {
            DateTime currentTime = DateTime.Now;
            string currentTimeString = currentTime.ToString("yyyy年MM月dd日  HH:mm:ss");
            string today = currentTime.ToString("yyyy/MM/dd");
            foreach (Imms.Mes.Data.Domain.Workshop workshop in this._WorkshopList)
            {
                WorkshopKanbaData kanbanData = this._WorkshopDataList[workshop.OrgCode];
                kanbanData.CurrentTime = currentTimeString;

                Imms.Mes.Data.Domain.WorkshiftSpan lastSpan = this._WorkshiftSpanList[workshop.OrgCode]
                     .Where(x => DateTime.Parse(today + " " + x.TimeBegin) < currentTime)
                     .OrderByDescending(x => x.Seq)
                     .FirstOrDefault();
                kanbanData.IsBreak = (lastSpan == null || lastSpan.IsBreak == 1);
                if (kanbanData.IsBreak)
                {
                    continue;  //现在是休息时间
                }

                var lineNoArray = this._WorklineList.Where(x => x.ParentId == workshop.RecordId).Select(x => x.OrgCode);
                List<Data.Domain.Workorder> orders = _ActiveWorkOrderList.Where(x => lineNoArray.Contains(x.LineNo)).ToList();
                if (orders.Count == 0)
                {
                    kanbanData.Customer = "N/A";
                    kanbanData.LineSummaryItems = new LineSummaryData[] { };
                    continue;
                }

                kanbanData.Customer = orders.First().CustomerNo;
                kanbanData.LineSummaryItems = new LineSummaryData[orders.Count];

                int passedHours = this._WorkshiftSpanList[workshop.OrgCode].Where(x => DateTime.Parse(today + " " + x.TimeEnd) < currentTime && x.IsBreak == 0).Count();
                DateTime firstTime = DateTime.Parse(today + " " + lastSpan.TimeBegin);
                int secconds = (int)currentTime.Subtract(firstTime).TotalSeconds;
                for (int i = 0; i < orders.Count; i++)
                {
                    Data.Domain.Workorder order = orders[i];
                    LineSummaryData itemData = this.CreateLineSummayData(kanbanData, currentTime, lastSpan, passedHours, secconds, order);
                    kanbanData.LineSummaryItems[i] = itemData;
                }
            }
        }

        private LineSummaryData CreateLineSummayData(WorkshopKanbaData kanbanData, DateTime currentTime, WorkshiftSpan lastSpan, int passedHours, int secconds, Workorder order)
        {
            LineSummaryData itemData = new LineSummaryData();
            itemData.LineNo = order.LineNo;
            itemData.PartNo = order.PartNo;
            itemData.DayTotalTarget = order.UPH * kanbanData.WorkHours;
            itemData.DayCurrentTarget = passedHours * order.UPH;
            int currentTarget = (int)((order.UPH / 3600F) * secconds);
            itemData.DayCurrentTarget += currentTarget;
            itemData.CurrentTarget = currentTarget;

            int qtyGood = this._DbContext.Set<Imms.Mes.Data.Domain.LineProductSummaryDateSpan>().Where(x => x.ProductDate == currentTime.Date && x.LineNo == order.LineNo).Sum(x => x.QtyGood);
            itemData.DayFinished = qtyGood;

            Imms.Mes.Data.Domain.LineProductSummaryDateSpan productSummay = this._DbContext.Set<Imms.Mes.Data.Domain.LineProductSummaryDateSpan>()
                   .Where(x => x.SpanId == lastSpan.RecordId && x.LineNo == order.LineNo && x.ProductDate == currentTime.Date)
                   .SingleOrDefault();
            if (productSummay != null)
            {
                itemData.CurrentFinished = productSummay.QtyGood;
                itemData.CurrentBad = productSummay.QtyBad;
            }

            return itemData;
        }

        public void RefreshWorkorder()
        {
            GlobalConstants.DefaultLogger.Info(this.ServiceId + "开始刷新工单...");
            lock (this)
            {
                this._ActiveWorkOrderList.Clear();
                var orders = this._DbContext.Set<Data.Domain.Workorder>()
                         .Where(x => x.OrderStatus == Data.Domain.Workorder.WOKORDER_STATUS_STARTED)
                         .ToList();
                this._ActiveWorkOrderList.AddRange(orders);
            }
            GlobalConstants.DefaultLogger.Info(this.ServiceId + "工单刷新完毕");
        }

        public void RefreshOrgData()
        {
            GlobalConstants.DefaultLogger.Info(this.ServiceId + "开始刷新组织结构与工作班次");
            lock (this)
            {
                this._WorkshopList.Clear();
                this._WorklineList.Clear();
                this._WorkshopDataList.Clear();
                this._WorkshiftSpanList.Clear();

                this._WorkshopList.AddRange(this._DbContext.Set<Imms.Mes.Data.Domain.Workshop>().ToList());
                this._WorklineList.AddRange(this._DbContext.Set<Imms.Mes.Data.Domain.Workline>().ToList());

                foreach (Imms.Mes.Data.Domain.Workshop workshop in this._WorkshopList)
                {
                    WorkshopKanbaData data = new WorkshopKanbaData();
                    data.WorkshopCode = workshop.OrgCode;

                    this._WorkshopDataList.Add(workshop.OrgCode, data);

                    long shiftId = this._DbContext.Set<Imms.Mes.Data.Domain.Workshift>().Where(x => x.ShiftCode == workshop.WorkshiftCode).Select(x => x.RecordId).Single();
                    List<Imms.Mes.Data.Domain.WorkshiftSpan> spans = this._DbContext.Set<Imms.Mes.Data.Domain.WorkshiftSpan>().Where(x =>
                       x.WorkshiftId == shiftId).ToList();
                    this._WorkshiftSpanList.Add(workshop.OrgCode, spans);
                    data.WorkHours = spans.Where(x => x.IsBreak == 0).Count();
                }
            }
            GlobalConstants.DefaultLogger.Info(this.ServiceId + "组织结构与工作班次刷新完毕");
        }

        public override bool Config()
        {
            this.ServiceId = "WORKSHOP_KANBAN_DATA_SERVICE";
            base.Config();
            this.ThreadIntervals = 1000 * 1;
            this._DbContext = GlobalConstants.DbContextFactory.GetContext();

            this._OldBefahavior = this._DbContext.ChangeTracker.QueryTrackingBehavior;
            this._DbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            this.RefreshOrgData();
            this.RefreshWorkorder();

            return true;
        }
    }

    public class WorkshopWebService : WebSocketHandler
    {
        private WorkshopDataService _DataService;
        private SortedList<string, WorkshopKanbanClient> _KanbanList = new SortedList<string, WorkshopKanbanClient>();
        public WorkshopWebService(WebSocketConnectionManager webSocketConnectionManager, WorkshopDataService dataService) : base(webSocketConnectionManager)
        {
            this._DataService = dataService;
        }

        public override Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            string socketId = WebSocketConnectionManager.GetId(socket);
            string workshopCode = System.Text.Encoding.UTF8.GetString(buffer, 0, result.Count);
            GlobalConstants.DefaultLogger.Info("SocketId:" + socketId + "连接的工场为:" + workshopCode);

            WorkshopKanbanClient kanban;
            lock (this)
            {
                if (_KanbanList.ContainsKey(socketId))
                {
                    kanban = _KanbanList[socketId];
                    if (kanban.Terminated)
                    {
                        kanban.Terminated = false;
                        kanban.Start();
                        GlobalConstants.DefaultLogger.Info(workshopCode + "已重新启动数据推送服务");
                    }
                    return Task.Run(() => GlobalConstants.DefaultLogger.Info(workshopCode + "已重新连接。"));
                }
            }

            kanban = new WorkshopKanbanClient();
            kanban.WebService = this;
            kanban.DataService = this._DataService;
            kanban.WorkshopCode = workshopCode;
            kanban.Socket = socket;
            kanban.SocketId = socketId;
            lock (this)
            {
                _KanbanList.Add(socketId, kanban);
            }

            return Task.Run(() =>
            {
                kanban.Start();
            });
        }

        public override void OnConnected(WebSocket socket)
        {
            base.OnConnected(socket);
            string socketId = WebSocketConnectionManager.GetId(socket);
            GlobalConstants.DefaultLogger.Info("收到Socket连接:" + socketId);
        }

        public override async Task OnDisconnected(WebSocket socket)
        {
            string socketId = WebSocketConnectionManager.GetId(socket);
            lock (this)
            {
                if (_KanbanList.ContainsKey(socketId))
                {
                    WorkshopKanbanClient kanban = _KanbanList[socketId];
                    kanban.Terminated = true;
                    _KanbanList.Remove(socketId);
                }
            }

            await base.OnDisconnected(socket);
        }
    }


    public class WorkshopKanbanClient
    {
        public WorkshopWebService WebService { get; set; }
        public WorkshopDataService DataService { get; set; }
        public string WorkshopCode { get; set; }
        public string SocketId { get; set; }
        public WebSocket Socket { get; set; }
        public bool Terminated { get; set; }

        public void Start()
        {
            GlobalConstants.DefaultLogger.Info("SocketId:" + this.SocketId + ", WorkshopCode:" + this.WorkshopCode + "的WebSocket开始推送数据");
            while (!this.Terminated)
            {
                string data = this.DataService.GetData(this.WorkshopCode);
                if (data != null)
                {
                    try
                    {
                        WebService.SendMessageAsync(Socket, data).GetAwaiter();
                    }
                    catch (Exception e)
                    {
                        GlobalConstants.DefaultLogger.Error(this.WorkshopCode + "发送数据失败：" + e.Message);
                        GlobalConstants.DefaultLogger.Error(e.StackTrace);
                    }
                }

                Thread.Sleep(1000 * 1);
            }

            GlobalConstants.DefaultLogger.Info("SocketId:" + this.SocketId + ", WorkshopCode:" + this.WorkshopCode + "的WebSocket推送数据已停止");
        }
    }
}
