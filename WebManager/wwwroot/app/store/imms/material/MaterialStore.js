Ext.define("app.store.imms.material.MaterialStore",{
    extend:"app.store.BaseStore",
    model: 'app.model.imms.material.MaterialModel',
    alias:'widget.app_store_imms_material_MaterialStore',
     
    dao:{
        deleteUrl: 'imms/material/material/delete',
        insertUrl: 'imms/material/material/create',
        updateUrl: 'imms/material/material/update',
        selectUrl: 'imms/material/material/getAll',
    }
});