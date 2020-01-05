Ext.define("app.view.imms.mfc.workstationProductSummary.WorkstationProductSummary", {
    extend: "app.ux.dbgrid.DbGrid",
    xtype: "app_view_imms_mfc_workstationProductSummary_WorkstationProductSummary",
    requires: ["app.model.imms.mfc.WorkstationProductSummaryModel", "app.store.imms.mfc.WorkstationProductSummaryStore"],
    columns: [
        { dataIndex: "orderNo", text: "订单号" },
        { dataIndex: "partNo", text: "Part No" },
        { dataIndex: "lineNo", text: "产线" },
        { dataIndex: "workstationCode", text: "工位" },
        { dataIndex: "qtyGood", text: "良品数" },
        { dataIndex: "qtyBad", text: "不良数" },
    ],
    constructor: function (config) {
        var configBase = {
            store: Ext.create({ xtype: 'app_store_imms_mfc_WorkstationProductSummaryStore' }),
            detailFormClass: '',
            detailWindowTitle: '',
        };
        Ext.applyIf(config, configBase);

        this.callParent(arguments);
    },
});