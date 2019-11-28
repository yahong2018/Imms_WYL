Ext.define('app.model.imms.material.MaterialModel', {
    extend: 'app.model.TrackableModel',
    fields: [
        { name: "materialCode", type: "string" },
        { name: "materialName", type: "string" },
        { name: "AutoFinishedProgress", type: "int" },
        { name: "description", type: "string" },
    ]
});