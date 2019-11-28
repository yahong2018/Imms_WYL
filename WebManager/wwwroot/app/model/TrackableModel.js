Ext.define('app.model.TrackableModel', {
    extend: 'app.model.EntityModel',
    requires: ["app.ux.ZhxhDate"],    
    fields: [
        { name: "createById",  type: "int" },
        { name: "createByCode", type: "string" },
        { name: "createByName", type: "string" },
        { name: "createTime", type: 'zhxhDate', dateFormat: 'Y-m-d H:i:s' },

        { name: "updateById",  type: "int" },
        { name: "updateByCode", type: "string" },
        { name: "updateByName", type: "string" },
        { name: "updateTime", type: 'zhxhDate', dateFormat: 'Y-m-d H:i:s' },

        { name: "optFlag",  type: "int" },        
    ],
});