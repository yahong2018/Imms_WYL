Ext.define('app.model.admin.SystemMenuTreeModel', {
    extend: 'Ext.data.TreeModel',
    fields: [
        { name: 'text', mapping: 'programName' },
        { name: "programId", mapping: "recordId" },
        { name:"expanded",calculate:function(item){return true;}},
        {
            name: ' leaf', calculate: function (item) {
                if (item.id == 'root') {
                    return false;
                }
                item.leaf = (item.children == null || item.children.length == 0);

                return true;
            }
        },
        {
            name: 'glyph', convert: function (value) {
                return parseInt(value);
            }
        },
    ]
});