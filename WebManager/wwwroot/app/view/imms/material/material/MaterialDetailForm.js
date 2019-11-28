Ext.define("app.view.imms.material.material.MaterialDetailForm", {
    extend: "app.ux.TrackableFormPanel",
    xtype: "app_view_imms_material_material_MaterialDetailForm",
    padding: 10,
    width: 420,
    layout: "anchor",
    defaults: {
        layout: "anchor",
        anchor: "100%",
    },

    items: [
        {
            name: "materialCode",
            fieldLabel: "产品编码",
            xtype: "textfield",
            maxLength: 20,
            allowBlank: false,
            enforceMaxLength: true,
            width: 380,
        },
        {
            name: "materialName",
            fieldLabel: "产品名称",
            xtype: "textfield",
            maxLength: 50,
            allowBlank: false,
            enforceMaxLength: true,
            width: 380,
        },
        {
            xtype: "container",
            layout: "hbox",
            margin: '0 0 3 ',
            items: [
                { name: "autoFinishedProgress", xtype: "textfield", fieldLabel: "是否自动报工", width: 150, },
                { xtype: "label", margin: '8 20 0 5', flex: 0.8, text: "0.不自动   1.自动" },
            ]
        }
    ]
});