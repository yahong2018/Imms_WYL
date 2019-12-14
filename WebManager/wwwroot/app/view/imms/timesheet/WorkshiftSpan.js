Ext.define("app.view.imms.timesheet.WorkshiftSpan", {
    extend: "app.ux.dbgrid.DbGrid",
    xtype: "app_view_imms_timesheet_WorkshiftSpan",
    requires: ["app.model.imms.timesheet.WorkshiftSpanModel", "app.store.imms.timesheet.WorkshiftSpanStore"],
    uses: ["app.view.imms.timesheet.WorkshiftSpanDetailForm"],
    hideDefeaultPagebar: true,
    hideSearchBar: true,
    columns: [
        { dataIndex: "seq", text: "顺序" },
        { dataIndex: "timeBegin", text: "开始时间" },
        { dataIndex: "timeEnd", text: "结束时间" },
        {
            dataIndex: "isBreak", text: "是否休息", renderer: function (v) {
                if (v == 0) {
                    return "0.工作";
                }
                return "1.休息";
            }
        },
        {
            dataIndex: "isShowOnKanban", text: "是否隐藏", renderer: function (v) {
                if (v == 0) {
                    return "0.显示";
                }
                return "1.隐藏";
            }
        },
    ],
    constructor: function (config) {
        var configBase = {
            detailFormClass: 'app_view_imms_timesheet_WorkshiftSpanDetailForm',
            detailWindowTitle: '时间段',
            store: Ext.create({ xtype: 'imms_timesheet_WorkshiftSpanStore' })
        }
        Ext.applyIf(config, configBase);

        this.callParent(arguments);
    }
});