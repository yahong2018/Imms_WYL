Ext.define('app.model.EntityTreeModel', {
    extend: 'Ext.data.TreeModel',    
    uses:["app.model.EmptyGenerator"],
    identifier:'empty',
    fields: [
        { name: "recordId", type: "string", unique: true },
    ],
    idProperty: 'recordId'
});