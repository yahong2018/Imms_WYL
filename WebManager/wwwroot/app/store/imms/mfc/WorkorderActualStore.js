Ext.define("app.store.imms.mfc.WorkorderActualStore", {
    extend: "app.store.BaseStore",
    alias: 'widget.imms_mfc_WorkorderActualStore',
    model: "app.model.imms.mfc.WorkorderActualModel",
    dao: {
        deleteUrl: 'imms/mfc/workorderActual/delete',
        insertUrl: 'imms/mfc/workorderActual/create',
        updateUrl: 'imms/mfc/workorderActual/update',
        selectUrl: 'imms/mfc/workorderActual/getAll',
    }
});