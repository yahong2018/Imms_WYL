Ext.define("app.view.imms.mfc.workorder.Workorder", {
    extend: "app.ux.dbgrid.DbGrid",
    xtype: "app_view_imms_mfc_Workorder_Workorder",
    requires: ["app.model.imms.mfc.WorkorderModel", "app.store.imms.mfc.WorkorderStore", "app.view.imms.mfc.workorder.WorkorderController"],
    uses: ["app.view.imms.mfc.workorder.WorkorderDetailForm"],

    controller: "imms_mfc_workorder_WorkorderController",
    columns: [
        { dataIndex: "orderNo", text: '单号', width: 150 },
        {
            dataIndex: "orderStatus", text: '状态', width: 150, renderer: function (v) {
                if (v == 0) {
                    return "0.已计划";
                } else if (v == 1) {
                    return "1.已开工";
                } else if (v == 254) {
                    return "254.已取消";
                } else if (v == 255) {
                    return "255.已完工";
                }
            }
        },
        { dataIndex: "lineNo", text: '产线', width: 100 },
        { dataIndex: "partNo", text: '产品编号', width: 150 },
        { dataIndex: "customerNo", text: '客户', width: 150 },
        { dataIndex: "qtyReq", text: '计划数量', width: 80 },
        { dataIndex: "timeStartPlan", text: '计划开工', width: 150 },
        { dataIndex: "timeEndPlan", text: '计划完工', width: 150 },
        { dataIndex: "timeStartActual", text: '实际开工', width: 150 },
        { dataIndex: "timeEndActual", text: '实际完工', width: 150 },
        { dataIndex: "qtyGood", text: '良品数', width: 80 },
        { dataIndex: "qtyBad", text: '不良数', width: 80 },
    ],

    additionToolbarItems: [
        '-',
        { text: '开工', handler: 'startOrder', privilege: "START" },
        '-',
        { text: '导入', handler: 'importOrder', privilege: "IMPORT" },
    ],

    constructor: function (config) {
        var configBase = {
            store: Ext.create({ xtype: 'imms_mfc_WorkorderStore' }),
            detailFormClass: 'imms_mfc_workorder_WorkorderDetailForm',
            detailWindowTitle: '生产计划',
        };
        Ext.applyIf(config, configBase);

        this.callParent(arguments);
    },
});