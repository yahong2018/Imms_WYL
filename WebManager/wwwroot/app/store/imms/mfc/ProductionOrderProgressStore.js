Ext.define("app.store.imms.mfc.ProductionOrderProgressStore",{
    extend:"app.store.BaseStore",
    model:"app.model.imms.mfc.ProductionOrderProgressModel",
    alias:"widget.imms_mfc_ProductionOrderProgressStore",

    dao: {
        deleteUrl: 'imms/mfc/productionOrderProgress/delete',
        insertUrl: 'imms/mfc/productionOrderProgress/create',
        updateUrl: 'imms/mfc/productionOrderProgress/update',
        selectUrl: 'imms/mfc/productionOrderProgress/getAll',
    }
});