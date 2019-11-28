Ext.define("app.store.admin.SystemParameterStore",{
    extend:"app.store.BaseStore",
    model:"app.model.admin.SystemParameterModel",
    alias:"widget.app_store_admin_SystemParameterStore",
    dao:{
        deleteUrl: 'admin/systemParameter/delete',
        insertUrl: 'admin/systemParameter/create',
        updateUrl: 'admin/systemParameter/update',
        selectUrl: 'admin/systemParameter/getAll',
    }
});