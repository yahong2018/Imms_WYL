Ext.define("app.view.imms.mfc.qualityCheck.QualityCheckDetailForm", {
    extend: "app.ux.TrackableFormPanel",
    xtype: "imms_mfc_qualityCheck_QualityCheckDetailForm",
    width: 650,
    padding: 5,
    layout: "anchor",
    defaults: {
        layout: "anchor",
        anchor: "100%",
    },
    defectStore: Ext.create({ xtype: "imms_mfc_DefectStore", autoLoad: true, pageSize: 0 }),
    operatorStore: Ext.create({ xtype: 'imms_org_OperatorStore', autoLoad: true, pageSize: 0 }),
    workshopStore: Ext.create({ xtype: 'imms_org_WorkshopStore', autoLoad: true, pageSize: 0 }),
    productionStore: Ext.create({ xtype: 'app_store_imms_material_MaterialStore', autoLoad: true, pageSize: 0 }),
    items: [
        { name: "defectId", xtype: "hidden" },
        { name: "productionId", xtype: "hidden" },
        { name: "workshopId", xtype: "hidden" },

        { name: "timeOfOriginWork", xtype: "textfield", fieldLabel: "工作日",  allowBlank: false },
        {
            xtype: "container",
            layout: "hbox",
            margin: '0 0 3 ',
            items: [                
                { name: "shiftId", xtype: "textfield", fieldLabel: "班次", margin: '0 20 5 0', allowBlank: false },
                { xtype:"label",text:"0.白班  1.夜班"},
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
                            debugger;
                            var form = this.up("imms_mfc_qualityCheck_QualityCheckDetailForm");
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
                            var form = this.up("imms_mfc_qualityCheck_QualityCheckDetailForm");
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
        { name: "wocgCode", xtype: "textfield", width: 200, fieldLabel: "工作中心组", allowBlank: false },
        {
            xtype: "container",
            layout: "hbox",
            margin: '0 0 3 ',
            items: [
                { name: "timeOfOrigin", xtype: "textfield", fieldLabel: "产生时间", margin: '0 20 5 0', allowBlank: false },
                { name: "qty", xtype: "textfield", fieldLabel: "缺陷数量", margin: '0 20 5 0', allowBlank: false },
            ]
        },       
        {
            xtype: "container",
            layout: "hbox",
            margin: '0 0 3 ',
            items: [
                {
                    name: "defectCode", xtype: "textfield", fieldLabel: "缺陷", margin: '0 20 5 0', allowBlank: false, listeners: {
                        change: function (self, newValue, oldValue, eOpts) {
                            var form = this.up("imms_mfc_qualityCheck_QualityCheckDetailForm");
                            var record = form.defectStore.findRecord("defectCode", newValue, 0, false, false, true);
                            if (record != null) {
                                form.down("[name='defectId']").setValue(record.get("recordId"));
                                form.down("[name='defectName']").setValue(record.get("defectName"));
                            } else {
                                form.down("[name='defectId']").setValue(-1);
                                form.down("[name='defectName']").setValue("");
                            }
                        }
                    }
                },
                { name: "defectName", xtype: "textfield", margin: '0 20 5 0', allowBlank: false, flex: 0.8 },
            ]
        }
    ],
    showHiddenItems: [
        {
            name: "recordId",
            xtype: "textfield",
            fieldLabel: "业务流水号",
            readOnly: true,
        }
    ]
});