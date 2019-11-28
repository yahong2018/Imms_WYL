Ext.define("app.store.imms.mfc.ProductionMovingStore",{
    extend:"app.store.BaseStore",
    model:"app.model.imms.mfc.ProductionMovingModel",
    alias:"widget.imms_mfc_ProductionMovingStore",

    dao:{
        deleteUrl: 'imms/mfc/productionOrderMoving/delete',
        insertUrl: 'imms/mfc/productionOrderMoving/create',
        updateUrl: 'imms/mfc/productionOrderMoving/update',
        selectUrl: 'imms/mfc/productionOrderMoving/getAll',
    }

});