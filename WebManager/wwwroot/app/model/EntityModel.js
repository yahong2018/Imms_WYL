Ext.define('app.model.EntityModel', {
    extend: 'Ext.data.Model',    
    requires: ["app.model.EmptyGenerator"],
    identifier:'empty',
    fields: [
        { name: "recordId", type: "string", unique: true },
    ],
    idProperty: 'recordId'
});