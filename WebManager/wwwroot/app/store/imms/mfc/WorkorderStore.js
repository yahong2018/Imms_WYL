Ext.define("app.store.imms.mfc.WorkorderStore",{
    extend:"app.store.BaseStore",
    alias: 'widget.imms_mfc_WorkorderStore',
    model: "app.model.imms.mfc.WorkorderModel",
    dao: {
        deleteUrl: 'imms/mfc/workorder/delete',
        insertUrl: 'imms/mfc/workorder/create',
        updateUrl: 'imms/mfc/workorder/update',
        selectUrl: 'imms/mfc/workorder/getAll',
    }
});