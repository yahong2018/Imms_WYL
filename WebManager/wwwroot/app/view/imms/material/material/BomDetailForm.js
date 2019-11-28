Ext.define("app.view.imms.material.material.BomDetailForm", {
    extend: "app.ux.TrackableFormPanel",
    xtype: "app_view_imms_material_material_BomDetailForm",
    padding: 10,
    width: 520,
    materialStore: Ext.create({ xtype: "app_store_imms_material_MaterialStore", pageSize: 0 }),
    items: [
        {
            xtype: "hidden",
            name: "bomNo",
        },
        {
            xtype: "hidden",
            name: "bomStatus",
        }, {
            xytpe: "hidden",
            name: "bomType",
        },
        {
            xtype: "hidden",
            name: "materialId"
        },
        {
            xtype: "hidden",
            name: "componentId"
        },
        { name: "materialCode", fieldLabel: "父件编码", xtype: "textfield", maxLength: 20, allowBlank: false, enforceMaxLength: true, width: 250, readOnly: true, },
        { name: "materialName", fieldLabel: "父件名称", xtype: "textfield", maxLength: 50, allowBlank: false, enforceMaxLength: true, width: 500, readOnly: true, },
        { name: "materialQty", fieldLabel: "父件用量", xtype: "textfield", width: 200, allowBlank: false, enforceMaxLength: true, },
        {
            name: "componentCode", fieldLabel: "组品编码", xtype: "textfield", maxLength: 20, allowBlank: false, enforceMaxLength: true, width: 250, listeners: {
                change: function (self, newValue, oldValue, eOpts) {
                    var form = this.up("app_view_imms_material_material_BomDetailForm");
                    var record = form.materialStore.findRecord("materialCode", newValue, 0, false, false, true);
                    if (record != null) {
                        form.down("[name='componentId']").setValue(record.get("recordId"));
                        form.down("[name='componentName']").setValue(record.get("materialName"));
                    }
                }
            }
        },
        { name: "componentName", fieldLabel: "组品名称", xtype: "textfield", maxLength: 50, allowBlank: false, enforceMaxLength: true, width: 500, readOnly: true },
        { name: "componentQty", fieldLabel: "组件用量", xtype: "textfield", width: 200, allowBlank: false, enforceMaxLength: true },
        { name: "effectDate", fieldLabel: "生效日期", xtype: "textfield", width: 200, allowBlank: false, enforceMaxLength: true },
    ],
    onRecordLoad: function (config) {
        debugger;
        if (config.dataMode == app.ux.data.DataMode.INSERT && config.seq == app.ux.data.DataOperationSeq.BEFORE) {
            var materialGrid = config.grid.up("app_view_imms_material_material_Material").down("app_view_imms_material_material_MaterialGrid");
            var material = materialGrid.getSelectedRecord();
            if (!material) {
                return false;
            }

            var record = config.record;

            record.set("materialId", material.get("recordId"));
            record.set("materialCode", material.get("materialCode"));
            record.set("materialName", material.get("materialName"));
        }
    }
});