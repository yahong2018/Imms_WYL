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
            GlobalConstants.DefaultLogger.Info(lineNo);
            if (_KanbanList.ContainsKey(socketId))
            {
                return Task.Run(() => GlobalConstants.DefaultLogger.Info("空闲处理..."));
            }

            KanbanClient kanban = new KanbanClient();
            kanban.Service = this;
            kanban.LineNo = lineNo;
            kanban.Socket = socket;
            _KanbanList.Add(socketId, kanban);
            return Task.Run(() =>
            {
                kanban.Start();
            });
        }

        public override void OnConnected(WebSocket socket)
        {
            base.OnConnected(socket);
        }

        public override async Task OnDisconnected(WebSocket socket)
        {
            string socketId = WebSocketConnectionManager.GetId(socket);
            if (_KanbanList.ContainsKey(socketId))
            {
                KanbanClient kanban = _KanbanList[socketId];
                kanban.Terminated = true;
                _KanbanList.Remove(socketId);
            }

            await base.OnDisconnected(socket);
        }
    }

    public class KanbanClient
    {
        public LineKanbanService Service { get; set; }
        public string LineNo { get; set; }
        public WebSocket Socket { get; set; }
        public bool Terminated { get; set; }

        public void Start()
        {
            while (!this.Terminated)
            {
                KanbanLineData lineData = Service.DataService.GetLineData(this.LineNo);
                if (lineData != null)
                {
                    string lineDataMessage = lineData.ToJson();
                    Service.SendMessageAsync(Socket, lineDataMessage).GetAwaiter();
                }

                Thread.Sleep(1000 * 1);
            }
        }
    }
}