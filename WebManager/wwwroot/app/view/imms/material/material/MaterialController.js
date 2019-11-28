Ext.define("app.view.imms.material.material.MaterialController", {
    extend: 'Ext.app.ViewController',
    alias: "controller.imms_material_material_MaterialController",
    materialGridSelectionChanged: function (model, selected, index) {
        this.getView().down('app_view_imms_material_material_BomGrid').getStore().getBom(selected);        
    },
});