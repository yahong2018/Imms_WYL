Ext.define("app.view.imms.org.workstation.Workstation", {
    extend: "app.ux.dbgrid.DbGrid",
    xtype: "org_workstation_Workstation",
    requires: ["app.model.imms.org.WorkstationModel", "app.store.imms.org.WorkstationStore", "app.model.imms.org.WorkshopModel"],
    uses: ["app.view.imms.org.workstation.WorkstationDetailForm"],

    hideDefeaultPagebar: true,
    hideSearchBar: true,

    columns: [
        { dataIndex: "orgCode", text: "工位代码", width: 100 },
        { dataIndex: "orgName", text: "工位名称", width: 150 },
        { dataIndex: "gid", text: "组号", width: 120 },
        { dataIndex: "did", text: "机号", width: 120 },
        {
            dataIndex: "defectReportMethod", text: "不良汇报方式", width: 150, renderer: function (v) {
                if (v == 2) {
                    return "2.按键汇报";
                } else if (v == 9) {
                    return "9.光感汇报";
                }
            }
        },
        { dataIndex: "seq", text: "顺序", width: 150 },
    ],

    constructor: function (config) {
        var configBase = {
            store: Ext.create({ xtype: 'imms_org_WorkstationStore', autoLoad: false }),
            detailFormClass: 'imms_org_workstation_WorkstationDetailForm',
            detailWindowTitle: '工位管理'
        }
        Ext.applyIf(config, configBase);

        this.callParent(arguments);
    },
});