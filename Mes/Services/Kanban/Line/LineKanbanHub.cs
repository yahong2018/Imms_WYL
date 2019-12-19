using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.SignalR;

namespace Imms.Mes.Services.Kanban.Line
{
    public class LineKanbanHub : Hub
    {
        private DataService _DataService = null;
        public bool Terminated { get; set; }
        private System.Threading.Thread _DieCheckThread;
        private List<WebSocketClient> _ClientList = new List<WebSocketClient>();

        public LineKanbanHub(DataService dataService)
        {
            this._DataService = dataService;
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Claim claim = this.GetClaim(JwtClaimTypes.Id);
            if (claim == null)
            {
                return base.OnDisconnectedAsync(exception);
            }

            string userId = claim.Value;
            GlobalConstants.DefaultLogger.Info(string.Format("ConnectionId:{0},UserId:{1}已断开.", Context.ConnectionId, userId));
            lock (this)
            {
                foreach (WebSocketClient client in this._ClientList)
                {
                    if (client.UserId == userId)
                    {
                        client.Shutdown();
                        GlobalConstants.DefaultLogger.Info(client.LineNo + "的看板服务已停止.");

                        break;
                    }
                }
            }
            return base.OnDisconnectedAsync(exception);
        }

        private Claim GetClaim(string type){
            foreach(Claim claim in Context.User.Claims){
                if(claim.Type == type){
                    return claim;
                }
            }
            return null;
        }

        public void RegisterWebClient(string lineNo)
        {
            Claim claim = this.GetClaim(JwtClaimTypes.Id);
            if (claim == null)
            {             
                this.Context.Abort();   
                return;
            }
            string userId = claim.Value;
            GlobalConstants.DefaultLogger.Info(string.Format("ConnectionId:{0},LineNo:{1},UserId:{2}已连接.", Context.ConnectionId, lineNo, userId));

            lock (this)
            {
                WebSocketClient client = new WebSocketClient();

                client = new WebSocketClient();
                client.ConnectionId = Context.ConnectionId;
                client.LineNo = lineNo;
                client.UserId = userId;
                client.Hub = this;
                client.DataService = this._DataService;

                this._ClientList.Add(client);
                client.Config();
                client.Startup();
            }
        }

        public void Stop()
        {
            lock (this)
            {
                foreach (WebSocketClient client in this._ClientList)
                {
                    client.Shutdown();
                }

                this._ClientList.Clear();
            }
        }

        public void Start()
        {
            if (_DieCheckThread == null)
            {
                _DieCheckThread = new System.Threading.Thread(() =>
                    {
                        while (!Terminated)
                        {
                            try
                            {
                                lock (this)
                                {
                                    for (int i = 0; i < this._ClientList.Count; i++)
                                    {
                                        WebSocketClient client = this._ClientList[i];
                                        if (client.Terminated)
                                        {
                                            this._ClientList.RemoveAt(i);
                                            i--;
                                        }
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                GlobalConstants.DefaultLogger.Error(e.Message);
                                GlobalConstants.DefaultLogger.Debug(e.StackTrace);
                            }

                            Thread.Sleep(1000 * 60 * 5); // 5分钟检查一次
                        }
                    });
                _DieCheckThread.Start();
            }
        }
    }

    public class WebSocketClient : BaseService
    {
        public string ConnectionId { get; set; }
        public string UserId { get; set; }
        public string LineNo { get; set; }
        public LineKanbanHub Hub { get; set; }
        public DataService DataService { get; set; }

        public override bool Config()
        {
            base.Config();
            this.ThreadIntervals = 1000;
            return true;
        }

        protected override void DoInternalThreadProc()
        {
            KanbanLineData data = this.DataService.GetLineData(this.LineNo);
            if (data != null)
            {
                try
                {
                    Hub.Clients.User(this.UserId).SendAsync("OnServerData", data);
                }
                catch (Exception e)
                {
                    GlobalConstants.DefaultLogger.Error("WebSocket出现错误：" + e.Message);
                    GlobalConstants.DefaultLogger.Trace(e.StackTrace);

                    this.Terminated = true;
                }
            }
        }
    }
}