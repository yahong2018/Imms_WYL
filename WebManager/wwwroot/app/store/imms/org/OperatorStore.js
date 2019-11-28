Ext.define("app.store.imms.org.OperatorStore", {
    extend: "app.store.BaseStore",
    model: 'app.model.imms.org.OperatorModel',
    alias: 'widget.imms_org_OperatorStore',

    dao: {
        deleteUrl: 'imms/org/operator/delete',
        insertUrl: 'imms/org/operator/create',
        updateUrl: 'imms/org/operator/update',
        selectUrl: 'imms/org/operator/getAll',
    }
});