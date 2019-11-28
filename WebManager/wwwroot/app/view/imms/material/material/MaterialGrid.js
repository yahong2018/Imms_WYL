Ext.define("app.view.imms.material.material.MaterialGrid", {
    extend: "app.ux.dbgrid.DbGrid",
    xtype: "app_view_imms_material_material_MaterialGrid",

    requires: ["app.store.imms.material.MaterialStore", "app.model.imms.material.MaterialModel",
    ],
    uses: ["app.view.imms.material.material.MaterialDetailForm"],

    columns: [
        { dataIndex: "materialCode", text: '产品编码', width: 150 },
        { dataIndex: "materialName", text: '产品名称', flex: 1 },
        {
            dataIndex: "autoFinishedProgress", text: '是否自动报工', width: 100, renderer: function (v) {
                if (v == 1) {
                    return "1.是";
                }
                return "0.否";
            }
        }
    ],

    constructor: function (config) {
        var configBase = {
            store: Ext.create({
                xtype: 'app_store_imms_material_MaterialStore', grid: this, listeners: {
                    load: function () {
                        if (this.getCount() > 0 && !this.grid.dataProcessed) {
                            this.grid.dataProcessed = true;
                            this.grid.getSelectionModel().select(0);
                        }
                    }
                }
            }),
            detailFormClass: 'app_view_imms_material_material_MaterialDetailForm',
            detailWindowTitle: '物料',
        }
        Ext.applyIf(config, configBase);

        this.callParent(arguments);
    },
    listeners: {
        beforeselect: "materialGridSelectionChanged"
    }
});