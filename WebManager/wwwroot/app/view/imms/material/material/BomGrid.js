Ext.define("app.view.imms.material.material.BomGrid", {
    extend: "app.ux.dbgrid.DbGrid",
    xtype: "app_view_imms_material_material_BomGrid",
    requires: ["app.model.imms.material.BomModel", "app.store.imms.material.BomStore"],
    uses: ["app.view.imms.material.material.BomDetailForm"],
    columns: [
        { dataIndex: "componentCode", text: '子件编码', width: 150 },
        { dataIndex: "componentName", text: '子件名称', width: 250 },
        { dataIndex: "materialQty", text: '父件用量', width: 80 },
        { dataIndex: "componentQty", text: '子件用量', width: 80 },
        { dataIndex: "effectDate", text: '生效日期', width: 120 },
    ],
    hideDefeaultPagebar: true,
    hideSearchBar: true,
    constructor: function (config) {
        var configBase = {
            store: Ext.create({ xtype: 'app_store_imms_material_BomStore', autoLoad: false }),
            detailFormClass: 'app_view_imms_material_material_BomDetailForm',
            detailWindowTitle: 'BOM',
        }
        Ext.applyIf(config, configBase);

        this.callParent(arguments);
    },
    beforeInsert: function (config) {
        var materialGrid = this.up("app_view_imms_material_material_Material").down("app_view_imms_material_material_MaterialGrid");
        if (!materialGrid.getSelectedRecord()) {
            return false;
        }
    }
});