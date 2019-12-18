using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Imms.Mes.Services.Kanban.Line
{
    public class LineKanbanHub : Hub
    {
        private DataService _DataService = null;
        public bool Terminated { get; set; }
        private System.Threading.Thread _DieCheckThread;
        private SortedList<string, WebSocketClient> _ConnectedIdList = new SortedList<string, WebSocketClient>();

        public LineKanbanHub(DataService dataService)
        {
            this._DataService = dataService;
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            string id = Context.ConnectionId;
            lock (this)
            {
                if (this._ConnectedIdList.ContainsKey(id))
                {
                    WebSocketClient client = this._ConnectedIdList[id];
                    GlobalConstants.DefaultLogger.Info(client.LineNo + "的看板已断开");
                    client.Shutdown();

                    this._ConnectedIdList.Remove(id);
                }
            }
            return base.OnDisconnectedAsync(exception);
        }

        public void RegisterWebClient(string lineNo)
        {
            GlobalConstants.DefaultLogger.Info("收到来自" + lineNo + "的看板请求连接...");
            string id = Context.ConnectionId;

            lock (this)
            {
                WebSocketClient client;
                if (!this._ConnectedIdList.ContainsKey(id))
                {
                    client = new WebSocketClient();
                    client.ConnectionId = id;
                    client.LineNo = lineNo;
                    this._ConnectedIdList.Add(id, client);
                }
                else
                {
                    client = this._ConnectedIdList[id];
                }

                client.Proxy = this.Clients.Client(id);
                client.DataService = this._DataService;
                if (client.Status == ServiceStatus.Stopped)
                {
                    client.Config();
                    client.Startup();
                }
            }
        }

        public void Stop()
        {
            lock (this)
            {
                foreach (WebSocketClient client in this._ConnectedIdList.Values)
                {
                    client.Shutdown();
                }

                this._ConnectedIdList.Clear();
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
                                    for (int i = 0; i < this._ConnectedIdList.Count; i++)
                                    {
                                        string id = this._ConnectedIdList.Keys[i];
                                        WebSocketClient client = this._ConnectedIdList[id];
                                        if (client.Terminated)
                                        {
                                            this._ConnectedIdList.Remove(id);
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
        public string LineNo { get; set; }
        public IClientProxy Proxy { get; set; }
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
                    Proxy.SendAsync("OnServerData", data);
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