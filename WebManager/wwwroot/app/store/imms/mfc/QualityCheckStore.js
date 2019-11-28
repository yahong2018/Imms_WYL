Ext.define("app.store.imms.mfc.QualityCheckStore", {
    extend: "app.store.BaseStore",
    alias: 'widget.imms_mfc_QualityCheckStore',
    model: "app.model.imms.mfc.QualityCheckModel",
    dao: {
        deleteUrl: 'imms/mfc/qualityCheck/delete',
        insertUrl: 'imms/mfc/qualityCheck/create',
        updateUrl: 'imms/mfc/qualityCheck/update',
        selectUrl: 'imms/mfc/qualityCheck/getAll',
    }
});