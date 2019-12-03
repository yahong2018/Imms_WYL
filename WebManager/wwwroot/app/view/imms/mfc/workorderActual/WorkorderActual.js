Ext.define("app.view.imms.mfc.workorderActual.WorkorderActual", {
    extend: "app.ux.dbgrid.DbGrid",
    xtype: "app_view_imms_mfc_workorderActual_WorkorderActual",
    requires: ["app.model.imms.mfc.WorkorderActualModel", "app.store.imms.mfc.WorkorderActualStore"],
    columns: [
        { dataIndex: "workorderNo", text: "工单号",width:100 },
        { dataIndex: "partNo", text: "产品编号", width: 150 },
        { dataIndex: "workstationCode", text: "工位编号", width: 150 },
        { dataIndex: "qty", text: "数量" },
        {
            dataIndex: "recordType", text: "记录类型", renderer: function (v) {
                if (v == 0) {
                    return "0.完工"
                }
                return "1.不良";
            }
        },
        { dataIndex: "defectCode", text: "品质代码" },
        { dataIndex: "reportTime", text: "汇报时间", width: 200 }
    ],

    constructor: function (config) {
        var configBase = {
            store: Ext.create({ xtype: 'imms_mfc_WorkorderActualStore' }),
            detailFormClass: '',
            detailWindowTitle: '',
        };
        Ext.applyIf(config, configBase);

        this.callParent(arguments);
    },
});