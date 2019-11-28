Ext.define("app.view.imms.mfc.productionMoving.ProductionMovingDetailForm", {
    extend: "app.ux.TrackableFormPanel",
    xtype: "imms_mfc_productionMoving_ProductionMovingDetailForm",
    padding: 5,
    width: 600,
    layout: "anchor",
    defaults: {
        layout: "anchor",
        anchor: "100%",
    },
    operatorStore: Ext.create({ xtype: 'imms_org_OperatorStore', autoLoad: true, pageSize: 0 }),
    workshopStore: Ext.create({ xtype: 'imms_org_WorkshopStore', autoLoad: true, pageSize: 0 }),
    workstationStore: Ext.create({ xtype: 'imms_org_WorkstationStore', autoLoad: true, pageSize: 0 }),
    items: [
        { name: "workshopIdFrom", xtype: "hidden" },
        { name: "productionId", xtype: "hidden" },
        { name: "rfidCardId", xtype: "hidden" },
        { name: "operatorId", xtype: "hidden" },
        { name: "workstationId", xtype: "hidden" },
        { name: "workshopId", xtype: "hidden" },
        { name: "prevProgressRecordId", xtype: "hidden" },
        {
            xtype: "container",
            layout: "hbox",
            margin: '0 0 3 ',
            items: [
                { name: "productionCode", xtype: "textfield", fieldLabel: "交接产品", width: 200, allowBlank: false },
                { name: "productionName", xtype: "textfield", margin: '0 20 0 5', flex: 0.8, readOnly: true },
            ]
        },
        {
            xtype: "container",
            layout: "hbox",
            margin: '0 0 3 ',
            items: [
                {
                    name: "employeeId", xtype: "textfield", fieldLabel: "接收人", width: 200, allowBlank: false, listeners: {
                        change: function (self, newValue, oldValue, eOpts) {
                            var form = this.up("imms_mfc_productionMoving_ProductionMovingDetailForm");
                            var record = form.operatorStore.findRecord("employeeId", newValue, 0, false, false, true);
                            if (record != null) {
                                form.down("[name='operatorId']").setValue(record.get("recordId"));
                                form.down("[name='employeeName']").setValue(record.get("employeeName"));
                            }
                        }
                    }
                },
                { name: "employeeName", xtype: "textfield", margin: '0 20 0 5', flex: 0.8, readOnly: true },
            ]
        },
        {
            xtype: "container",
            layout: "hbox",
            margin: '0 0 3 ',
            items: [
                {
                    name: "workshopCode", xtype: "textfield", fieldLabel: "接收车间", width: 200, allowBlank: false, listeners: {
                        change: function (self, newValue, oldValue, eOpts) {
                            var form = this.up("imms_mfc_productionMoving_ProductionMovingDetailForm");
                            var record = form.workshopStore.findRecord("orgCode", newValue, 0, false, false, true);
                            if (record != null) {
                                form.down("[name='workshopId']").setValue(record.get("recordId"));
                                form.down("[name='workshopName']").setValue(record.get("orgName"));
                            }
                        }
                    }
                },
                { name: "workshopName", xtype: "textfield", margin: '0 20 0 5', flex: 0.8, readOnly: true },
            ]
        },
        {
            xtype: "container",
            layout: "hbox",
            margin: '0 0 3 ',
            items: [
                {
                    name: "workshopCodeFrom", xtype: "textfield", fieldLabel: "原车间", width: 200, allowBlank: false, listeners: {
                        change: function (self, newValue, oldValue, eOpts) {
                            var form = this.up("imms_mfc_productionMoving_ProductionMovingDetailForm");
                            var record = form.workstationStore.findRecord("orgCode", newValue, 0, false, false, true);
                            if (record != null) {
                                form.down("[name='workshopIdFrom']").setValue(record.get("recordId"));
                                form.down("[name='workshopNameFrom']").setValue(record.get("orgName"));
                            }
                        }
                    }
                },
                { name: "workshopNameFrom", xtype: "textfield", margin: '0 20 0 5', flex: 0.8, readOnly: true },
            ]
        },
        {
            xtype: "container",
            layout: "hbox",
            margin: '0 0 3 ',
            items: [
                { name: "rfidNo", xtype: "textfield", fieldLabel: "RFID号", },
                { name: "qty", xtype: "textfield", fieldLabel: "接收数量", margin: '0 0 0 20', allowBlank: false },
            ]
        },
        {
            xtype: "container",
            layout: "hbox",
            margin: '0 0 3 ',
            items: [
                { name: "timeOfOrigin", xtype: "textfield", fieldLabel: "接收时间", margin: '0 20 5 0', allowBlank: false },
                { xtype: "label" }
            ]
        },
        {
            xtype: "container",
            layout: "hbox",
            margin: '0 0 3 ',
            items: [
                { name: "rfidTerminatorId", xtype: "textfield", fieldLabel: "工位机号" },
                { name: "rfidControllerGroupId", xtype: "textfield", fieldLabel: "控制器号", margin: '0 0 0 20' },
            ]
        },
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