Ext.define("app.view.imms.timesheet.WorkshiftSpanDetailForm", {
    extend: "Ext.form.Panel",
    xtype: "app_view_imms_timesheet_WorkshiftSpanDetailForm",
    width: 400,
    bodyPadding: 5,
    items: [
        {
            xtype: "hidden",
            name: "recordId"
        }, {
            xtype: "hidden",
            name: "workshiftId",
        },
        {
            xtype: "textfield",
            name: "seq",
            fieldLabel: "序号",
            allowBlank: false,
            width: 380
        },
        {
            xtype: "textfield",
            name: "timeBegin",
            fieldLabel: "开始时间",
            allowBlank: false,
            width: 380
        }, {
            xtype: "textfield",
            name: "timeEnd",
            fieldLabel: "结束时间",
            allowBlank: false,
            width: 380
        },
        {
            xtype: "container",
            layout: "hbox",
            margin: '0 0 3 ',
            items: [
                {
                    xtype: "textfield",
                    name: "isBreak",
                    allowBlank: false,
                    fieldLabel: "是否休息"
                },
                {
                    xtype: "label",
                    text: "0.工作  1.休息",
                    margin: '8 20 5 5', flex: 0.8, readOnly: true
                }
            ]
        },
        {
            xtype: "container",
            layout: "hbox",
            margin: '0 0 3 ',
            items: [
                {
                    xtype: "textfield",
                    name: "isShowOnKanban",
                    allowBlank: false,
                    fieldLabel: "是否隐藏"
                },
                {
                    xtype: "label",
                    text: "0.显示  1.隐藏",
                    margin: '8 20 5 5', flex: 0.8, readOnly: true
                }
            ]
        }, {
            xtype: "textfield",
            name: "delayTime",
            fieldLabel: "红灯延迟时间",
            allowBlank: false
        }
    ],
    onRecordLoad: function (config) {
        if (config.dataMode == app.ux.data.DataMode.INSERT && config.seq == app.ux.data.DataOperationSeq.BEFORE) {
            var record = config.record;
            var grid = config.grid;

            record.set("workshiftId", grid.store.parent.get("recordId"));
        }
    }
});