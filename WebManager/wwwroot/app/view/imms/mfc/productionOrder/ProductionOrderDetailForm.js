Ext.define("app.view.imms.mfc.productionOrder.ProductionOrderDetailForm", {
    extend: "app.ux.TrackableFormPanel",
    xtype: "imms_mfc_productionOrder_ProductionOrderDetailForm",
    padding: 5,
    width: 400,
    defaults: {
        labelWidth: 100
    },

    items: [
        { name: "orderNo", xtype: "textfield", maxLength: 20, enforceMaxLength: true, allowBlank: false, fieldLabel: '计划单号', width: 350 },
        { name: "orderStatus", xtype: "textfield", fieldLabel: '状态', allowBlank: false, width: 350 },
        {
            name: "productionId", fieldLabel: '产品', width: 350, xtype: "combobox", valueField: "recordId", allowBlank: false,
            displayField: "materialName",
            store: Ext.create("app.store.imms.material.MaterialStore", { autoLoad: true }),
            listeners: {
                change: function (self, newValue, oldValue, eOpts) {
                    var record = self.getSelectedRecord();
                    var form = self.up("imms_mfc_productionOrder_ProductionOrderDetailForm");
                    var productionCode = form.down("[name='productionCode']");
                    var productionName = form.down("[name='productionName']");
                    productionCode.setValue(record.get("materialCode"));
                    productionName.setValue(record.get("materialName"));
                }
            }
        },
        { name: "productionCode", xtype: "hidden" },
        { name: "productionName", xtype: "hidden" },
        { name: "planDate", xtype: "textfield", fieldLabel: '计划生产日期', allowBlank: false, width: 350 },
        { name: "qtyPlanned", xtype: "textfield", fieldLabel: '计划数量', allowBlank: false, width: 350 },
        { name: "qtyGood", xtype: "textfield", fieldLabel: '完工数', allowBlank: false, width: 250 },
        { name: "qtyBad", xtype: "textfield", fieldLabel: '次品数', allowBlank: false, width: 250 },
    ],
    onRecordLoad: function (config) {
        if (config.seq == app.ux.data.DataOperationSeq.BEFORE && config.dataMode == app.ux.data.DataMode.INSERT) {
            config.record.data.planDate = Ext.Date.format(new Date(), 'Y-m-d');
        }
    }
});