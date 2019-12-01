Ext.define("app.view.imms.org.workline.Workline", {
    extend: "app.ux.dbgrid.DbGrid",
    xtype: "org_workline_Workline",
    requires: ["app.model.imms.org.WorklineModel", "app.store.imms.org.WorklineStore"],
    uses: ["app.view.imms.org.workline.WorklineDetailForm"],
    hideDefeaultPagebar: true,
    hideSearchBar: true,

    columns: [
        { dataIndex: "orgCode", text: "车间代码", width: 120 },
        { dataIndex: "orgName", text: "车间名称", width: 200 },      
        { dataIndex: "gid", text: "组号", width: 120 },
        { dataIndex: "did", text: "机号", width: 120 },              
    ],
    constructor: function (config) {
        var configBase = {
            detailFormClass: 'imms_org_workline_WorklineDetailForm',
            detailWindowTitle: '产线',
            store: Ext.create({
                xtype: 'imms_org_WorklineStore', grid: this, listeners: {
                    load: function () {
                        if (this.getCount() > 0 && !this.grid.dataProcessed) {
                            this.grid.dataProcessed = true;
                            this.grid.getSelectionModel().select(0);
                        }
                    }
                }
            })
        }
        Ext.applyIf(config, configBase);

        this.callParent(arguments);
    },

    listeners: {
        beforeselect: 'gridSelectionChanged',
    }
});