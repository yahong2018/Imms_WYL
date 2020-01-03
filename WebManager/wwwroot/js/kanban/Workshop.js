"use strict";

var server_data = {
    isBreak: false,
    workshopCode: "A3",
    currentTime: "2020年01月02日 23:42",
    customer: "威雅利",
    lineSummaryItems: [
        { lineNo: "A201-1", partNo: "AF98", dayTotalTarget: 1735, dayCurrentTarget: 763, dayFinished: 715, currentTarget: 25, currentFinished: 23, currentBad: 0 },
        { lineNo: "A201-1", partNo: "AF98", dayTotalTarget: 1735, dayCurrentTarget: 763, dayFinished: 715, currentTarget: 25, currentFinished: 23, currentBad: 0 },
        { lineNo: "A201-1", partNo: "AF98", dayTotalTarget: 1735, dayCurrentTarget: 763, dayFinished: 715, currentTarget: 25, currentFinished: 23, currentBad: 0 },
        { lineNo: "A201-1", partNo: "AF98", dayTotalTarget: 1735, dayCurrentTarget: 763, dayFinished: 715, currentTarget: 25, currentFinished: 23, currentBad: 0 },
        { lineNo: "A201-1", partNo: "AF98", dayTotalTarget: 1735, dayCurrentTarget: 763, dayFinished: 715, currentTarget: 25, currentFinished: 23, currentBad: 0 },
        { lineNo: "A201-1", partNo: "AF98", dayTotalTarget: 1735, dayCurrentTarget: 763, dayFinished: 715, currentTarget: 25, currentFinished: 23, currentBad: 0 }
    ]
};

function on_server_data() {
    if (server_data.is_break) {
        return;
    }

    var workshopCode_1 = document.getElementById("workshop_1");
    var workshopCode_2 = document.getElementById("workshop_2");
    var workshopCode_3 = document.getElementById("workshop_3");
    var customer = document.getElementById("customer");
    var current_time_label = document.getElementById("current_time");
    var data_table = document.getElementById("data_table");
    var avg_rate_label = document.getElementById("avg_rate");

    workshopCode_1.innerText = server_data.workshopCode;
    workshopCode_2.innerText = server_data.workshopCode;
    workshopCode_3.innerText = server_data.workshopCode;
    customer.innerText = server_data.customer;
    current_time_label.innerText = server_data.currentTime;

    var row_count = data_table.rows.length;
    var line_count = server_data.lineSummaryItems.length;
    var i = 1;
    var j = 0;
    var total_target = 0;
    var total_finished = 0;
    for (; i < row_count; i++ , j++) {
        var line_data = (j < line_count) ? server_data.lineSummaryItems[j] : null;
        var row = data_table.rows[i];

        row.cells[0].innerText = (j < line_count) ? line_data.lineNo : "/";
        row.cells[1].innerText = (j < line_count) ? line_data.partNo : "/";
        row.cells[2].innerText = (j < line_count) ? line_data.dayTotalTarget : "/";
        row.cells[3].innerText = (j < line_count) ? line_data.dayCurrentTarget : "/";
        row.cells[4].innerText = (j < line_count) ? line_data.dayFinished : "/";

        var current_rate = 0;
        if ((j < line_count) && (line_data.dayCurrentTarget != 0)) {
            current_rate = ((line_data.dayFinished / line_data.dayCurrentTarget) * 100).toFixed(1);
        }
        row.cells[5].innerText = (j < line_count) ?
            line_data.dayCurrentTarget != 0 ? (current_rate + "%") : "0.0%"
            : "/";
        if (j < line_count) {
            if (current_rate > 100) {
                row.cells[5].style.backgroundColor = 'blue';
            } else if (current_rate >= 98) {
                row.cells[5].style.backgroundColor = 'green';
            } else {
                row.cells[5].style.backgroundColor = 'red';
            }
        }
        row.cells[6].innerText = (j < line_count) ? line_data.currentTarget : "/";
        row.cells[7].innerText = (j < line_count) ? line_data.currentFinished : "/";
        row.cells[8].innerText = (j < line_count) ? line_data.currentBad : "/";

        if (line_data!=null) {
            total_target += line_data.dayCurrentTarget;
            total_finished += line_data.dayFinished;
        }
    }

    var avg_rate = 0;
    if (total_target != 0) {
        avg_rate = ((total_finished / total_target) * 100).toFixed(1);
    }
    
    avg_rate_label.innerText = avg_rate + "%";
    if (avg_rate > 100) {
        avg_rate_label.style.backgroundColor = 'blue';
    } else if (avg_rate >= 98) {
        avg_rate_label.style.backgroundColor = 'green';
    } else {
        avg_rate_label.style.backgroundColor = 'red';
    }
}

function getQueryVariable(variable) {
    var query = window.location.search.substring(1);
    var vars = query.split("&");
    for (var i = 0; i < vars.length; i++) {
        var pair = vars[i].split("=");
        if (pair[0] == variable) { return pair[1]; }
    }
    return (false);
}
var workshopCode = getQueryVariable("workshopCode");
var WebSocketProxy = {
    open: function () {
        try {
            var scheme = document.location.protocol === "https:" ? "wss" : "ws";
            var port = document.location.port ? (":" + document.location.port) : "";
            var url = scheme + "://" + document.location.hostname + port + "/workshop";
            var websocket = new WebSocket(url);

            this.websocket = websocket;
            var me = this;
            websocket.onopen = function () {
                console.log("connected,before login...");
                me.send(workshopCode);
                console.log("after logined");
            };
            websocket.onerror = function (evt) {
                console.log("error");
                me.reconnect();
            };
            websocket.onclose = function () {
                console.log("closed.");
                me.reconnect();
            };
            websocket.onmessage = function (evt) {
                try {
                    server_data = JSON.parse(evt.data);
                    on_server_data();
                } catch (e) {
                    console.error(e);
                }
            };
        } catch (e) {
        }
    },
    reconnect: function () {
        console.log("begin reconnect ...");
        setTimeout(this.open(), 1000 * 10);
    },
    close: function () {
        this.websocket.close();
        this.websocket = null;
    },
    send: function (msg) {
        this.websocket.send(msg);
    }
};

debugger;
WebSocketProxy.open();

