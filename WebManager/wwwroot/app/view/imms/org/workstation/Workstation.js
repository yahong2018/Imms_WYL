Ext.define("app.view.imms.org.workstation.Workstation", {
    extend: "app.ux.dbgrid.DbGrid",
    xtype: "org_workstation_Workstation",
    requires: ["app.model.imms.org.WorkstationModel", "app.store.imms.org.WorkstationStore", "app.model.imms.org.WorkshopModel"],
    uses: ["app.view.imms.org.workstation.WorkstationDetailForm"],

    hideDefeaultPagebar: true,
    hideSearchBar: true,
    
    columns: [
        { dataIndex: "orgCode", text: "工位代码", width: 100 },
        { dataIndex: "orgName", text: "工位名称", width: 200 },
        { dataIndex: "wocgCode", text: "工作中心组", width: 200 },
        { dataIndex: "rfidTemplateIndex", text: "显示模板编号", width: 150 },
        { dataIndex: "rfidControllerId", text: "Rfid控制器编号", width: 150 },
        { dataIndex: "rfidTerminatorId", text: "Rfid工位机编号", width: 150 },
        { dataIndex: "autoReportCount", text: "自动工序数", width: 150 },
        { dataIndex: "description", text: "备注", flex: 1 }
    ],
    constructor: function (config) {       
        var configBase = {
            store: Ext.create({ xtype: 'imms_org_WorkstationStore',autoLoad:false }),
            detailFormClass: 'imms_org_workstation_WorkstationDetailForm',
            detailWindowTitle: '工位管理'
        }
        Ext.applyIf(config, configBase);

        this.callParent(arguments);
    },
});