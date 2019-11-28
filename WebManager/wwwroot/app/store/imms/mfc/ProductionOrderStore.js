Ext.define("app.store.imms.mfc.ProductionOrderStore",{
    extend:"app.store.BaseStore",
    alias: 'widget.imms_mfc_ProductionOrderStore',
    model: "app.model.imms.mfc.ProductionOrderModel",
    dao: {
        deleteUrl: 'imms/mfc/productionOrder/delete',
        insertUrl: 'imms/mfc/productionOrder/create',
        updateUrl: 'imms/mfc/productionOrder/update',
        selectUrl: 'imms/mfc/productionOrder/getAll',
    }
});