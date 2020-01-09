Ext.define("app.view.imms.mfc.workorder.Workorder", {
    extend: "app.ux.dbgrid.DbGrid",
    xtype: "app_view_imms_mfc_workorder_Workorder",
    requires: ["app.model.imms.mfc.WorkorderModel",
        "app.store.imms.mfc.WorkorderStore",
        "app.view.imms.mfc.workorder.WorkorderController",
        "app.view.imms.mfc.workorder.WorkorderColumns"],
    uses: ["app.view.imms.mfc.workorder.WorkorderDetailForm"],

    controller: "imms_mfc_workorder_WorkorderController",
    columns: Ext.create("app.view.imms.mfc.workorder.WorkorderColumns").ColumnItems,

    additionToolbarItems: [
        '-',
        { text: '开工', handler: 'startOrder', privilege: "START", btnName: "BTN_START" },
        { text: '完工', handler: 'completeOrder', privilege: "COMPLETE", btnName: "BTN_COMPLETE" },
        // '-',
        // { text: '导入', handler: 'importOrder', privilege: "IMPORT" },
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
    listeners: {
        selectionchange: 'gridSelectionChanged'
    }
});