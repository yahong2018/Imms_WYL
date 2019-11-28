Ext.define("app.view.imms.mfc.rfidCard.RfidCardDetailForm", {
    extend: "app.ux.TrackableFormPanel",
    xtype: "imms_mfc_rfidCard_RfidCardDetailForm",
    padding: 5,
    width: 600,
    layout: "anchor",
    defaults: {
        layout: "anchor",
        anchor: "100%",
    },
    workshopStore: Ext.create({ xtype: 'imms_org_WorkshopStore', autoLoad: true, pageSize: 0 }),
    productionStore: Ext.create({ xtype: 'app_store_imms_material_MaterialStore', autoLoad: true, pageSize: 0 }),
    items: [
        { name: "productionId", xtype: "hidden" },
        { name: "workshopId", xtype: "hidden" },
        {
            name: "kanbanNo",
            xtype: "textfield",
            fieldLabel: "看板编号",
            allowBlank: false,
        },
        {
            name: "rfidNo",
            xtype: "textfield",
            fieldLabel: "RFID卡号",
            allowBlank: false,
        },
        {
            xtype: "container",
            layout: "hbox",
            margin: '0 0 3 ',
            items: [
                {
                    name: "cardType",
                    xtype: "textfield",
                    fieldLabel: "看板类型",
                    allowBlank: false,
                },
                {
                    xtype: "label",
                    text: "2.工程内看板  3.外发看板",
                    flex: 0.8,
                    margin: '8 0 5 5', 
                }
            ]
        },
        {
            name: "towerNo",
            xtype: "textfield",
            fieldLabel: "塔编号",
            allowBlank: true,
        },
        {
            xtype: "container",
            layout: "hbox",
            margin: '0 0 3 ',
            items: [
                {
                    name: "cardStatus",
                    xtype: "textfield",
                    fieldLabel: "看板状态",
                    allowBlank: false,
                },
                {
                    xtype: "label",
                    text: "0.未使用 1.已派发 2.已退回 3.已绑定 20.已移库(外发) 30.已回厂 40.已回厂投入 255.已作废",
                    flex: 0.8,
                    margin: '0 0 0 5', 
                }
            ]
        },
        {
            xtype: "container",
            layout: "hbox",
            margin: '0 0 3 ',
            items: [
                {
                    name: "productionCode", xtype: "textfield", fieldLabel: "产品", allowBlank: false, listeners: {
                        change: function (self, newValue, oldValue, eOpts) {
                            var form = this.up("imms_mfc_rfidCard_RfidCardDetailForm");
                            var record = form.productionStore.findRecord("materialCode", newValue, 0, false, false, true);
                            if (record != null) {
                                form.down("[name='productionId']").setValue(record.get("recordId"));
                                form.down("[name='productionName']").setValue(record.get("materialName"));
                            }
                        }
                    }
                },
                { name: "productionName", xtype: "textfield", margin: '0 20 0 5', allowBlank: false, flex: 0.8, readOnly: true },
            ]
        },
        {
            xtype: "container",
            layout: "hbox",
            items: [
                {
                    name: "workshopCode", xtype: "textfield", fieldLabel: "部门", allowBlank: false,
                    listeners: {
                        change: function (self, newValue, oldValue, eOpts) {
                            var form = this.up("imms_mfc_rfidCard_RfidCardDetailForm");
                            var record = form.workshopStore.findRecord("orgCode", newValue, 0, false, false, true);
                            if (record != null) {
                                form.down("[name='workshopId']").setValue(record.get("recordId"));
                                form.down("[name='workshopName']").setValue(record.get("orgName"));
                            }
                        }
                    }
                },
                { name: "workshopName", xtype: "textfield", flex: 0.8, margin: '0 20 5 5', allowBlank: false, readOnly: true },
            ]
        },
        {
            xtype: "container",
            layout: "hbox",
            items: [
                {
                    name: "issueQty",
                    xtype: "textfield",
                    fieldLabel: "派发数量",
                    allowBlank: false,
                },
                {
                    name: "stockQty",
                    xtype: "textfield",
                    fieldLabel: "完工数量",
                    allowBlank: false,
                    margin: '0 0 0 20',
                }
            ]
        }
    ],
    beforePost :function(config){         
        if (config.record.towerNo == "" && config.record.cardType == "3") {
            Ext.Msg.alert("系统提示","外发看板的【塔编号】必须输入!");
            return false;
        }
        return true;
    }
});