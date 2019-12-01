Ext.define("app.view.imms.org.workline.Workline", {
    extend: "app.ux.dbgrid.DbGrid",
    xtype: "org_workline_Workline",    
    uses: ["app.view.imms.org.workline.WorklineDetailForm"],
    hideDefeaultPagebar: true,
    hideSearchBar: true,

    columns: [
        { dataIndex: "orgCode", text: "产线编号", width: 120 },
        { dataIndex: "orgName", text: "产线名称", width: 200 },      
        { dataIndex: "gid", text: "组号", width: 120 },
        { dataIndex: "did", text: "机号", width: 120 },              
    ],
    additionToolbarItems: [
        '-',
        { text: '产线板', handler: 'openLineKanban', privilege: "KANBAN_LINE" },
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
        beforeselect: 'worklineGridSelectionChanged',
    }
});