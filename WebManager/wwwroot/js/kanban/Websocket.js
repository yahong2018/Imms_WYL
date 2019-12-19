var ws = new WebSocket(url);
ws.onclose = function () {
    reconnect()
};
ws.onerror = function () {
    reconnect()
};

// 重连
function reconnect(url) {
    setTimeout(function () {     //没连接上会一直重连，设置延迟避免请求过多
        createWebSocket(url);
    }, 2000);
}

// 实例websocket
function createWebSocket(url) {
    try {
        if ('WebSocket' in window) {
            ws = new WebSocket(url);
        } else if ('MozWebSocket' in window) {
            ws = new MozWebSocket(url);
        } else {
            _alert("当前浏览器不支持websocket协议,建议使用现代浏览器", 3000)
        }
        initEventHandle();
    } catch (e) {
        reconnect(url);
    }
}

// 初始化事件函数
function initEventHandle() {
    ws.onclose = function () {
        reconnect(wsUrl);
    };
    ws.onerror = function (err) {
        reconnect(wsUrl);
    };
}