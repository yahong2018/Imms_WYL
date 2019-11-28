"use strict";

var dom = document.getElementById("charts");
var myChart = echarts.init(dom);
var app = {};
var option = {
    textStyle: {
        color: 'white'
    },
    title: {
        text: '生产效率',
        left: 'center',
        textStyle: {
            color: 'red',
            fontSize: 32,
        }
    },
    tooltip: {
        trigger: 'axis'
    },
    legend: {
        data: [{ name: '良品' }, { name: '不良品' }],
        left: 'right',
        top: '10%',
        icon: 'rect',
        textStyle: {
            color: 'white'
        }
    },
    grid: {
        right: '1%',
        bottom: '8%',
    },
    xAxis: {
        type: 'category',
        boundaryGap: false,
        data: ['8:00', '9:00', '10:00', '11:00', '12:00', '13:00', '14:00', '15:00', '16:00', '17:00', '18:00', '19:00', '20:00'],
        splitLine: {
            show: true,
            lineStyle: {
                type: 'dotted'
            }
        }
    },
    yAxis: {
        type: 'value',
        min: 0,
        max: 160,
        interval: 20,
    },
    series: [
        {
            name: '良品',
            type: 'line',
            itemStyle: {
                color: 'green',
            },
            lineStyle: {
                type: 'dashed'
            },
            data: [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]
        },
        {
            name: '不良品',
            type: 'line',
            itemStyle: {
                color: 'red',
            },
            lineStyle: {
                type: 'dashed'
            },
            data: [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]
        }
    ]
};

if (option && typeof option === "object") {
    myChart.setOption(option, true);
}

function dateFtt(fmt, date) { //author: meizz 
    var o = {
        "M+": date.getMonth() + 1,     //月份 
        "d+": date.getDate(),     //日 
        "h+": date.getHours(),     //小时 
        "m+": date.getMinutes(),     //分 
        "s+": date.getSeconds(),     //秒 
        "q+": Math.floor((date.getMonth() + 3) / 3), //季度 
        "S": date.getMilliseconds()    //毫秒 
    };
    if (/(y+)/.test(fmt))
        fmt = fmt.replace(RegExp.$1, (date.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
        if (new RegExp("(" + k + ")").test(fmt))
            fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
    return fmt;
}

setInterval(function () {
    var current_time = document.getElementById('current_time');
    var now = new Date();
    var text = dateFtt("yyyy年MM月dd日 hh:mm:ss", now)
    current_time.innerText = text;
}, 1000);

var total_plan, total_actual, total_good, total_bad;
total_plan = total_actual = total_good = total_bad = 0;

var server_data = {
    line_code: "A301-1",
    line_summary_data: {
        production_code: "AL666-ACC-02R",
        production_name: "",
        production_order_no: "",
        uph: 30,
        person_qty: 6
    },
    line_detail_data: [
        { hour: 8, index: -1, qty_plan: 0, qty_good: 0, qty_bad: 0 },
        { hour: 9, index: 0, qty_plan: 100, qty_good: 0, qty_bad: 0 },
        { hour: 10, index: 1, qty_plan: 100, qty_good: 0, qty_bad: 0 },
        { hour: 11, index: 2, qty_plan: 100, qty_good: 0, qty_bad: 0 },
        { hour: 12, index: 3, qty_plan: 100, qty_good: 0, qty_bad: 0 },
        { hour: 13, index: 4, qty_plan: 100, qty_good: 0, qty_bad: 0 },
        { hour: 14, index: 5, qty_plan: 100, qty_good: 0, qty_bad: 0 },
        { hour: 15, index: 6, qty_plan: 100, qty_good: 0, qty_bad: 0 },
        { hour: 16, index: 7, qty_plan: 100, qty_good: 0, qty_bad: 0 },
        { hour: 17, index: 8, qty_plan: 100, qty_good: 0, qty_bad: 0 },
        { hour: 18, index: 9, qty_plan: 100, qty_good: 0, qty_bad: 0 },
        { hour: 19, index: 10, qty_plan: 100, qty_good: 0, qty_bad: 0 },
        { hour: 20, index: 11, qty_plan: 100, qty_good: 0, qty_bad: 0 },
    ]
};


function fill_line_code() {
    document.getElementById("line_id").innerText = "Cell Line:" + server_data.line_code;
}

function fill_line_summary() {
    var table = document.getElementById("line-summary");
    var tr = table.rows[0];

    var summary_data = server_data.line_summary_data || {};
    tr.cells[1].innerText = summary_data.production_code;
    tr.cells[3].innerText = summary_data.production_name;
    tr.cells[5].innerText = summary_data.production_order_no;
    tr.cells[7].innerText = summary_data.uph + "(PCS)";
    tr.cells[9].innerText = summary_data.person_qty + "(人)";
}

function fill_detail_data(config) {
    var detail_items = server_data.line_detail_data || [];
    var table = document.getElementById("line-details");

    var fill_row = function (row, data) {
        row.cells[1].innerText = data.qty_plan;
        row.cells[2].innerText = data.qty_good;
        row.cells[3].innerText = data.qty_bad;
        row.cells[4].innerText = data.sub_total;
        row.cells[5].innerText = data.percentOfPass;
        row.cells[6].innerText = data.percentOfProducton;
    };
    total_plan = total_actual = total_good = total_bad = 0;

    var hour = new Date().getHours() + 1;
    for (var i = 0; i < detail_items.length; i++) {        
        var item = detail_items[i];
        var index = item.index;
        if (hour >= item.hour) {
            config.good_items.push(item.qty_good);
            config.bad_items.push(item.qty_bad);
        }

        if (index < 0 || index > table.rows.length) {
            continue;
        }

        var row = table.rows[index + 1];
        var sub_total = item.qty_good + item.qty_bad;
        var percentOfPass = 0;
        if (sub_total != 0) {
            percentOfPass = (item.qty_good / sub_total) * 100;
        }
        var percentOfProducton = 0;
        if (item.qty_plan != 0) {
            percentOfProducton = (sub_total / item.qty_plan) * 100;
        }
        fill_row(row, {
            qty_plan: item.qty_plan,
            qty_good: item.qty_good,
            qty_bad: item.qty_bad,
            sub_total: sub_total,
            percentOfPass: percentOfPass.toFixed(2),
            percentOfProducton: percentOfProducton.toFixed(2)
        });

        total_good += item.qty_good;
        total_bad += item.qty_bad;
        total_plan += item.qty_plan;
        total_actual += sub_total;
    }
    var row_summary = table.rows[table.rows.length - 1];
    fill_row(row_summary, {
        qty_plan: total_plan + "(件)",
        qty_good: total_good + "(件)",
        qty_bad: total_bad + "(件)",
        sub_total: total_actual + "(件)",
        percentOfPass: (total_actual == 0 ? 0 : (((total_good / total_actual) * 100).toFixed(2))) + "%",
        percentOfProducton: (total_plan == 0 ? 0 : (((total_actual / total_plan) * 100).toFixed(2))) + "%"
    });
}

function fill_detail_summary() {
    var table = document.getElementById("detail-summary");
    var now = new Date();
    var hour = now.getHours() + 1;
    var detail_items = server_data.line_detail_data || [];
    var item = {};
    for (var i = 0; i < detail_items.length; i++) {
        if (detail_items[i].hour == hour) {
            item = detail_items[i];
            break;
        }
    }

    var text = "当前时间<br/>" + (hour -1) + ":00" + "~" + hour + ":00";
    var row_current = table.rows[1]; 
    row_current.cells[0].innerHTML = text;
    row_current.cells[1].innerText = item.qty_plan;

    var qty_actual = item.qty_good + item.qty_bad;
    row_current.cells[2].innerText = qty_actual;
    row_current.cells[3].innerText = (((item.qty_plan == 0) ? 0 : qty_actual / item.qty_plan) * 100).toFixed(2) + "%";
    row_current.cells[4].innerText = item.qty_bad;
    row_current.cells[5].innerText = (((qty_actual == 0) ? 0 : item.qty_bad / qty_actual) * 100).toFixed(2) + "%";

    var month = now.toDateString().split(" ")[1];
    var day = now.getDate();
    var year = now.getFullYear();

    text = "当天统计<br/>" + day + "-" + month + " " + year;
    var row_total = table.rows[2];
    row_total.cells[0].innerHTML = text;
    row_total.cells[1].innerText = total_plan;
    row_total.cells[2].innerText = total_actual;
    row_total.cells[3].innerText = (((total_plan == 0) ? 0 : (total_actual / total_plan)) * 100).toFixed(2) + "%";
    row_total.cells[4].innerText = total_bad;
    row_total.cells[5].innerText = (((total_actual == 0) ? 0 : (total_bad / total_actual)) * 100).toFixed(2) + "%";
}

var connection = new signalR.HubConnectionBuilder().withUrl("/kanbanHub/realtime").build();

connection.on("PushRealtimeData", function (realtimeItem) {
    server_data = realtimeItem;
    var good_items = [];
    var bad_items = [];

    fill_line_code();
    fill_line_summary();
    fill_detail_data({ good_items: good_items, bad_items: bad_items });
    fill_detail_summary();
  
    var options = {
        series: [
            {
                name: '良品',
                data: good_items
            },
            {
                name: '不良品',
                data: bad_items
            }
        ]
    };

    myChart.setOption(options);
});

function start_connection() {
    connection.start().then(function () {
        connection.invoke("RegisterRealtimeClient");
        console.log("connected");
    }).catch(function () {
        setTimeout(start_connection, 5000);
    });
};

connection.onclose(function () {
    start_connection();
});

start_connection();

fill_line_code();
fill_line_summary();
fill_detail_data({ good_items: [], bad_items: [] });
fill_detail_summary();

