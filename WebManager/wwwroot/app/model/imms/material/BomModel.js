Ext.define('app.model.imms.material.BomModel', {
    extend: 'app.model.TrackableModel',
    fields: [
        { name: "bomNo", type: "string" },
        { name: "bomType", type: "int" },
        { name: "bomStatus", type: "int" },

        { name: "materialCode", type: "string" },
        { name: "materialName", type: "string" },
        { name: "componentCode", type: "string" },
        { name: "componentName", type: "string" },
        
        { name: "materialQty", type: "int" },
        { name: "componentQty", type: "int" },

        { name: "effectDate", type: "zhxhDate", dateFormat: "Y-m-d" },
    ]
});