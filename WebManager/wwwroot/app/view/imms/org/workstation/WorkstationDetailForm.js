Ext.define("app.view.imms.org.workstation.WorkstationDetailForm", {
    extend: "Ext.form.Panel",
    xtype: "imms_org_workstation_WorkstationDetailForm",
    width: 400,
    bodyPadding: 5,
    layout: "anchor",
    defaults: {
        layout: "anchor",
        anchor: "100%",
    },
    items: [
        {
            name: "parentId",
            xtype: "hidden"
        },
        {
            name: "orgCode",
            xtype: "textfield",
            fieldLabel: "工位代码",
            allowBlank: false,
            maxLength: 20,
            enforceMaxLength: true,
            width: 250
        }, {
            name: "orgName",
            xtype: "textfield",
            fieldLabel: "工位名称",
            allowBlank: false,
            maxLength: 50,
            enforceMaxLength: true,
            width: 380
        }, {
            name: "gid",
            xtype: "textfield",
            fieldLabel: "组号",
            allowBlank: false,
            maxLength: 3,
            enforceMaxLength: true,
            width: 180
        }, {
            name: "did",
            xtype: "textfield",
            fieldLabel: "机号",
            allowBlank: false,
            maxLength: 3,
            enforceMaxLength: true,
            width: 180
        },
        {
            xtype: "container",
            layout: "hbox",
            margin: '0 0 3 ',
            items: [
                {
                    name: "defectReportType",
                    xtype: "textfield",
                    fieldLabel: "不良汇报方式",
                    allowBlank: false,
                    maxLength: 3,
                    enforceMaxLength: true,
                    width: 180
                }, { xtype: "label", text: "3.按键汇报  9.光电汇报" }
            ]
        },
        {
            name: "seq",
            xtype: "textfield",
            fieldLabel: "顺序",
            allowBlank: false,
            maxLength: 3,
            enforceMaxLength: true,
            width: 180
        }
    ],
    onRecordLoad: function (config) {
        if (config.dataMode == app.ux.data.DataMode.INSERT && config.seq == app.ux.data.DataOperationSeq.BEFORE) {
            var record = config.record;
            var grid = config.grid;

            record.set("parentId", grid.store.parent.get("recordId"));
        }
    }
});