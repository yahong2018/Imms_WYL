using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Imms.Mes.Services
{
    public static class WebSocketExtensions
    {
        public static IApplicationBuilder MapWebSocketManager(this IApplicationBuilder app, PathString path, WebSocketHandler handler)
        {
            return app.Map(path, (_app) => _app.UseMiddleware<WebSocketManagerMiddleware>(handler));
        }

        public static IServiceCollection AddWebSocketManager(this IServiceCollection services)
        {
            services.AddTransient<WebSocketConnectionManager>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly asm in assemblies)
            {
                foreach (var type in asm.ExportedTypes)
                {
                    if (type.GetTypeInfo().BaseType == typeof(WebSocketHandler))
                    {
                        services.AddSingleton(type);
                    }
                }
            }

            return services;
        }
    }

    public class WebSocketConnectionManager
    {
        private ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();

        public int GetCount()
        {
            return _sockets.Count;
        }

        public WebSocket GetSocketById(string id)
        {
            return _sockets.FirstOrDefault(p => p.Key == id).Value;
        }

        public ConcurrentDictionary<string, WebSocket> GetAll()
        {
            return _sockets;
        }
        public WebSocket GetWebSocket(string key)
        {
            WebSocket _socket;
            _sockets.TryGetValue(key, out _socket);
            return _socket;
        }

        public string GetId(WebSocket socket)
        {
            try
            {
                return _sockets.FirstOrDefault(p => p.Value == socket).Key;
            }
            catch
            {
                return null;
            }
        }
        public void AddSocket(WebSocket socket, string key)
        {
            if (GetWebSocket(key) != null)
            {
                _sockets.TryRemove(key, out WebSocket destoryWebsocket);
            }
            _sockets.TryAdd(key, socket);
        }

        public async Task RemoveSocket(string id)
        {
            try
            {
                WebSocket socket;
                _sockets.TryRemove(id, out socket);
                await socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
            }
            catch (Exception e)
            {
                GlobalConstants.DefaultLogger.Error("Socket移除出现异常：" + e.Message);
                GlobalConstants.DefaultLogger.Error(e.StackTrace);
            }
        }

        public async Task CloseSocket(WebSocket socket)
        {
            await socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
        }

        public string CreateConnectionId()
        {
            return Guid.NewGuid().ToString();
        }
    }

    public class WebSocketManagerMiddleware
    {
        private readonly RequestDelegate _next;
        private WebSocketHandler _webSocketHandler { get; set; }

        public WebSocketManagerMiddleware(RequestDelegate next,
                                          WebSocketHandler webSocketHandler)
        {
            _next = next;
            _webSocketHandler = webSocketHandler;
        }

        public async Task Invoke(Microsoft.AspNetCore.Http.HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
                return;

            var socket = await context.WebSockets.AcceptWebSocketAsync();
            string ip = context.Connection.RemoteIpAddress.MapToIPv4().ToString();
            GlobalConstants.DefaultLogger.Info("IP:" + ip + "已连接上服务器");
            _webSocketHandler.OnConnected(socket);


            await Receive(socket, async (result, buffer) =>
            {
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    string content = System.Text.Encoding.UTF8.GetString(buffer, 0, result.Count);
                    GlobalConstants.DefaultLogger.Info("IP:" + ip + "收到数据：" + content);
                    
                    await _webSocketHandler.ReceiveAsync(socket, result, buffer);
                    return;
                }

                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    GlobalConstants.DefaultLogger.Info("IP:" + ip + "已断开");
                    await _webSocketHandler.OnDisconnected(socket);
                    return;
                }
            });
        }

        private async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            try
            {
                var buffer = new byte[1024 * 4];
                while (socket.State == WebSocketState.Open)
                {
                    var result = await socket.ReceiveAsync(buffer: new ArraySegment<byte>(buffer), cancellationToken: CancellationToken.None);
                    handleMessage(result, buffer);
                }
            }
            catch (Exception ex)
            {
                GlobalConstants.DefaultLogger.Error("数据接收失败：" + ex.Message);
                GlobalConstants.DefaultLogger.Error(ex.StackTrace);

                handleMessage(new WebSocketReceiveResult(0, WebSocketMessageType.Close, false), null);
            }
        }
    }

    public abstract class WebSocketHandler
    {
        public WebSocketConnectionManager WebSocketConnectionManager { get; set; }

        public WebSocketHandler(WebSocketConnectionManager webSocketConnectionManager)
        {
            WebSocketConnectionManager = webSocketConnectionManager;
        }

        public virtual void OnConnected(WebSocket socket)
        {
            string id = WebSocketConnectionManager.CreateConnectionId();
            WebSocketConnectionManager.AddSocket(socket, id);
        }

        public virtual async Task OnDisconnected(WebSocket socket)
        {
            String id = WebSocketConnectionManager.GetId(socket);
            GlobalConstants.DefaultLogger.Info("Socket:" + id + "断开");
            await WebSocketConnectionManager.RemoveSocket(id);
        }

        public async Task SendMessageAsync(WebSocket socket, string message)
        {
            if (socket.State != WebSocketState.Open)
            {
                return;
            }
            var bytes = Encoding.UTF8.GetBytes(message);
            await socket.SendAsync(buffer: new ArraySegment<byte>(array: bytes, offset: 0, count: bytes.Length), messageType: WebSocketMessageType.Text, endOfMessage: true, cancellationToken: CancellationToken.None);
        }

        public async Task SendMessageAsync(string socketId, string message)
        {
            try
            {
                await SendMessageAsync(WebSocketConnectionManager.GetSocketById(socketId), message);
            }
            catch (Exception e)
            {
                GlobalConstants.DefaultLogger.Error("数据发送失败：" + e.Message);
                GlobalConstants.DefaultLogger.Error(e.StackTrace);
            }
        }

        public async Task SendMessageToAllAsync(string message)
        {
            foreach (var pair in WebSocketConnectionManager.GetAll())
            {
                if (pair.Value.State == WebSocketState.Open)
                    await SendMessageAsync(pair.Value, message);
            }
        }
        /// <summary>
        /// 获取一些连接
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public IEnumerable<WebSocket> GetSomeWebsocket(string[] keys)
        {
            foreach (var key in keys)
            {
                yield return WebSocketConnectionManager.GetWebSocket(key);
            }
        }

        /// <summary>
        /// 给一堆人发消息
        /// </summary>
        /// <param name="webSockets"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendMessageToSome(WebSocket[] webSockets, string message)
        {
            foreach (WebSocket socket in webSockets)
            {
                await SendMessageAsync(socket, message);
            }
        }
        public abstract Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer);
    }
}