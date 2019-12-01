Ext.define("app.view.imms.org.workshop.Workshop", {
    extend: "app.ux.dbgrid.DbGrid",
    xtype: "org_workshop_Workshop",
    uses: ["app.view.imms.org.workshop.WorkshopDetailForm"],
    hideDefeaultPagebar: true,
    hideSearchBar: true,

    columns: [
        { dataIndex: "orgCode", text: "车间编号", width: 120 },
        { dataIndex: "orgName", text: "车间名称", width: 200 },
    ],
    additionToolbarItems: [
        '-',
        { text: '工场板', handler: 'openWorkshopKanban', privilege: "KANBAN_WORKSHOP" },
    ],
    constructor: function (config) {
        var configBase = {
            detailFormClass: 'imms_org_workshop_WorkshopDetailForm',
            detailWindowTitle: '车间',
            store: Ext.create({
                xtype: 'imms_org_WorkshopStore', grid: this, listeners: {
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
        beforeselect: 'workshopGridSelectionChanged',
    }
});