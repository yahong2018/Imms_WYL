Ext.define("app.view.imms.material.material.Material", {
    extend: "Ext.panel.Panel",
    xtype:"app_view_imms_material_material_Material",
    requires: ["app.model.imms.material.BomModel", "app.store.imms.material.BomStore",
        "app.store.imms.material.MaterialStore", "app.model.imms.material.MaterialModel",
        "app.view.imms.material.material.MaterialGrid", "app.view.imms.material.material.BomGrid",
        "app.view.imms.material.material.MaterialController"],
    layout: "border",
    controller: {
        type: "imms_material_material_MaterialController"
    },
    items: [
        {
            region: "west",
            width: 750,
            xtype: "app_view_imms_material_material_MaterialGrid"
        },
        {
            region: "center",            
            xtype: "app_view_imms_material_material_BomGrid"
        }
    ]
});