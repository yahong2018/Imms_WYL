Ext.define("app.view.imms.org.operator.Operator", {
    extend: "app.ux.dbgrid.DbGrid",
    xtype: "app_view_imms_org_operator_Operator",
    requires: ["app.model.imms.org.OperatorModel", "app.store.imms.org.OperatorStore",
    "app.model.imms.org.WorkshopModel","app.store.imms.org.WorkshopStore"],
    uses: ["app.view.imms.org.operator.OperatorDetailForm"],
    columns: [
        { dataIndex: "orgCode", text: "产线", width: 100 },
        { dataIndex: "employeeId", text: "工号", width: 100 },
        { dataIndex: "employeeName", text: "姓名", width: 100 },
        { dataIndex: "title", text: "职位", width: 100 },        
    ],

    constructor:function(config){
        var configBase = {
            detailFormClass: 'imms_org_operator_OperatorDetailForm',
            detailWindowTitle: '员工管理',
            store: Ext.create({ xtype: 'imms_org_OperatorStore'})
        }
        Ext.applyIf(config, configBase);

        this.callParent(arguments);
    }
});