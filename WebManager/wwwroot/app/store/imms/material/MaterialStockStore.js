Ext.define("app.store.imms.material.MaterialStockStore",{
    extend:"app.store.BaseStore",
    alias:"widget.app_store_imms_material_MaterialStockStore",
    model:"app.model.imms.material.MaterialStockModel",
    dao:{
        selectUrl: 'imms/material/materialStock/getAll',
    }
})