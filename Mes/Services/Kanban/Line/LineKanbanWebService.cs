using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Imms.Mes.Services.Kanban.Line
{
    public class LineKanbanService : WebSocketHandler
    {
        private SortedList<string, KanbanClient> _KanbanList = new SortedList<string, KanbanClient>();
        public Imms.Mes.Services.Kanban.Line.DataService DataService { get; set; }

        public LineKanbanService(WebSocketConnectionManager webSocketConnectionManager, Imms.Mes.Services.Kanban.Line.DataService dataService) : base(webSocketConnectionManager)
        {
            this.DataService = dataService;
        }

        public override Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            string socketId = WebSocketConnectionManager.GetId(socket);
            string lineNo = System.Text.Encoding.UTF8.GetString(buffer, 0, result.Count);
            GlobalConstants.DefaultLogger.Info("SocketId:" + socketId + "连接的产线为:" + lineNo);

            KanbanClient kanban;
            lock (this)
            {
                if (_KanbanList.ContainsKey(socketId))
                {
                    kanban = _KanbanList[socketId];
                    if (kanban.Terminated)
                    {
                        kanban.Terminated = false;
                        kanban.Start();
                        GlobalConstants.DefaultLogger.Info(lineNo + "已重新启动数据推送服务");
                    }
                    return Task.Run(() => GlobalConstants.DefaultLogger.Info(lineNo + "已重新连接。"));
                }
            }

            kanban = new KanbanClient();
            kanban.Service = this;
            kanban.LineNo = lineNo;
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
                    KanbanClient kanban = _KanbanList[socketId];
                    kanban.Terminated = true;
                    _KanbanList.Remove(socketId);
                }
            }

            await base.OnDisconnected(socket);
        }
    }

    public class KanbanClient
    {
        public LineKanbanService Service { get; set; }
        public string LineNo { get; set; }
        public string SocketId { get; set; }
        public WebSocket Socket { get; set; }
        public bool Terminated { get; set; }

        public void Start()
        {
            GlobalConstants.DefaultLogger.Info("SocketId:" + this.SocketId + ", LineNo:" + this.LineNo + "的WebSocket开始推送数据");
            while (!this.Terminated)
            {
                string data = Service.DataService.GetLineDataString(this.LineNo);
                if (data != null)
                {
                    try
                    {
                        Service.SendMessageAsync(Socket, data).GetAwaiter();
                    }
                    catch (Exception e)
                    {
                        GlobalConstants.DefaultLogger.Error(this.LineNo + "发送数据失败：" + e.Message);
                        GlobalConstants.DefaultLogger.Debug(e.StackTrace);
                    }
                }

                Thread.Sleep(1000 * 1);
            }

            GlobalConstants.DefaultLogger.Info("SocketId:" + this.SocketId + ", LineNo:" + this.LineNo + "的WebSocket推送数据已停止");
        }
    }
}