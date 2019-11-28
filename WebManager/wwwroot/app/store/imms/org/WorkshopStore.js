Ext.define("app.store.imms.org.WorkshopStore",{
    extend:"app.store.BaseStore",
    model: 'app.model.imms.org.WorkshopModel',
    alias:'widget.imms_org_WorkshopStore',
     
    dao:{
        deleteUrl: 'imms/org/workshop/delete',
        insertUrl: 'imms/org/workshop/create',
        updateUrl: 'imms/org/workshop/update',
        selectUrl: 'imms/org/workshop/getAll',
    }
});