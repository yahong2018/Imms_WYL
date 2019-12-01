Ext.define("app.view.imms.org.operator.Operator", {
    extend: "app.ux.dbgrid.DbGrid",
    xtype: "app_view_imms_org_operator_Operator",
    requires: ["app.model.imms.org.OperatorModel", "app.store.imms.org.OperatorStore"],
    uses: ["app.view.imms.org.operator.OperatorDetailForm"],
    columns: [
        { dataIndex: "orgCode", text: "产线", width: 100 },
        { dataIndex: "empId", text: "工号", width: 100 },
        { dataIndex: "empName", text: "姓名", width: 150 },
        { dataIndex: "title", text: "工作岗位", width: 150 },
        { dataIndex: "seq", text: "显示顺序", width: 100 },        
    ],

    constructor: function (config) {
        var configBase = {
            detailFormClass: 'imms_org_operator_OperatorDetailForm',
            detailWindowTitle: '员工管理',
            store: Ext.create({ xtype: 'imms_org_OperatorStore' })
        }
        Ext.applyIf(config, configBase);

        this.callParent(arguments);
    }
});