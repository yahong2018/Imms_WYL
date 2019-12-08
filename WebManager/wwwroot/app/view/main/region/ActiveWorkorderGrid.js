Ext.define("app.view.main.region.ActiveWorkorderGrid", {
    extend: "app.ux.dbgrid.DbGrid",
    xtype: "app_view_main_region_ActiveWorkorderGrid",
    requires: ["app.view.imms.mfc.workorder.WorkorderColumns", "app.ux.dbgrid.DbGrid",
        "app.model.imms.mfc.WorkorderModel", "app.store.imms.mfc.WorkorderStore",],
  //  hideDefeaultPagebar: true,
    hideDefaultToolbar: true,
    columns: Ext.create("app.view.imms.mfc.workorder.WorkorderColumns").ColumnItems,
    constructor: function (config) {
        var configBase = {
            store: Ext.create({ xtype: 'imms_mfc_WorkorderStore', autoLoad: false }),
        };
        Ext.applyIf(config, configBase);

        this.callParent(arguments);
    },

    listeners: {
        afterrender: function () {
            var filter = {
                L: "orderStatus",
                O: "=",
                R: 1
            };
            this.store.clearCustomFilter();
            this.store.addCustomFilter(filter);
            this.store.buildFilterUrl();
            this.store.load();
        }
    }
});