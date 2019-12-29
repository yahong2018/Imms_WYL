Ext.define('app.model.admin.SystemMenuTreeModel', {
    extend: 'Ext.data.TreeModel',
    fields: [
        { name: 'text', mapping: 'programName' },
        { name: "programId", mapping: "recordId" },        
        {
            name: 'glyph', convert: function (value) {
                return parseInt(value);
            }
        },
    ]
});