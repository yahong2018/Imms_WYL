Ext.define("app.view.imms.timesheet.Workshift", {
    extend: "app.ux.dbgrid.DbGrid",
    xtype: "app_view_imms_timesheet_Workshift",
    requires: ["app.model.imms.timesheet.WorkshiftModel", "app.model.imms.timesheet.WorkshiftSpanModel",
        "app.store.imms.timesheet.WorkshiftStore", "app.store.imms.timesheet.WorkshiftStore",
    ],
    uses: ["app.view.imms.timesheet.WorkshiftDetailForm"],
    hideDefeaultPagebar: true,
    hideSearchBar: true,
    columns: [
        { dataIndex: "shiftCode", text: "班次代码" },
        { dataIndex: "shiftName", text: "班次名称" },
        {
            dataIndex: "shiftStatus", text: "是否停用", renderer: function (v) {
                if (v == 0) {
                    return "0.启用";
                }
                return "1.停用"
            }
        },
    ],
    constructor:function (config) {
        var configBase = {
            detailFormClass: 'app_view_imms_timesheet_WorkshiftDetailForm',
            detailWindowTitle: '班次',
            store: Ext.create({
                xtype: 'imms_timesheet_WorkshiftStore', grid: this, listeners: {
                    load: function () {
                        if (this.getCount() > 0 && !this.grid.dataProcessed) {
                            this.grid.dataProcessed = true;
                            this.grid.getSelectionModel().select(0);
                        }
                    }
                } })
        }
        Ext.applyIf(config, configBase);

        this.callParent(arguments);        
    },
    listeners: {
        beforeselect: function (model, selected, index) {
            var grid = this.up('app_view_timesheet_TimeSheet').down('app_view_imms_timesheet_WorkshiftSpan');
            grid.getStore().parent = selected;
            grid.getStore().getAllByParent();
        },
    }
});