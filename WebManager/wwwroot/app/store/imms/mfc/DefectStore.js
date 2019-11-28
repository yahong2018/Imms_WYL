Ext.define("app.store.imms.mfc.DefectStore",{
    extend:"app.store.BaseStore",
    model:"app.model.imms.mfc.DefectModel",
    alias: "widget.imms_mfc_DefectStore",

    dao: {
        deleteUrl: 'imms/mfc/defect/delete',
        insertUrl: 'imms/mfc/defect/create',
        updateUrl: 'imms/mfc/defect/update',
        selectUrl: 'imms/mfc/defect/getAll',
    }
});