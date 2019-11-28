Ext.define("app.store.imms.mfc.RfidCardStore", {
    extend: "app.store.BaseStore",
    alias: 'widget.imms_mfc_RfidCardStore',
    model: "app.model.imms.mfc.RfidCardModel",
    dao: {
        deleteUrl: 'imms/mfc/rfidCard/delete',
        insertUrl: 'imms/mfc/rfidCard/create',
        updateUrl: 'imms/mfc/rfidCard/update',
        selectUrl: 'imms/mfc/rfidCard/getAll',
    }
});