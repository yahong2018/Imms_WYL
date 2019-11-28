Ext.define("app.view.imms.mfc.productionMoving.ProductionMoving", {
    extend: "app.ux.dbgrid.DbGrid",
    xtype: "app_view_imms_mfc_productionMoving_ProductionMoving",
    requires: [
        "app.model.imms.mfc.ProductionMovingModel", "app.store.imms.mfc.ProductionMovingStore",
        "app.model.imms.mfc.RfidCardModel", "app.store.imms.mfc.RfidCardStore",
        "app.model.imms.org.WorkstationModel", "app.store.imms.org.WorkstationStore",
        "app.model.imms.org.WorkshopModel", "app.store.imms.org.WorkshopStore",
        "app.model.imms.material.MaterialModel", "app.store.imms.material.MaterialStore",
        "app.model.imms.org.OperatorModel", "app.store.imms.org.OperatorStore",
        "app.model.imms.mfc.ProductionOrderModel", "app.store.imms.mfc.ProductionOrderStore"
    ],
    uses: ["app.view.imms.mfc.productionMoving.ProductionMovingDetailForm"],
    columns: [
        // { dataIndex: "productionOrderNo", text: "计划单号" },
        { dataIndex: "recordId", text: "业务流水" },
        { dataIndex: "productionCode", text: "产品编号" },
        { dataIndex: "productionName", text: "产品名称",width:150 },

        { dataIndex: "workshopCode", text: "接收车间编号" },
        { dataIndex: "workshopName", text: "接收车间名称" },

        { dataIndex: "workshopCodeFrom", text: "原车间编号" },
        { dataIndex: "workshopNameFrom", text: "原车间名称" },

        { dataIndex: "qty", text: "接收数量" },
        { dataIndex: "employeeId", text: "接收人工号" },
        { dataIndex: "employeeName", text: "接收人姓名" },
        { dataIndex: "timeOfOrigin", text: "接收时间",width:150 },

        { dataIndex: "rfidNo", text: "RFID卡号" },
        { dataIndex: "rfidTerminatorId", text: "工位机号" },
        { dataIndex: "rfidControllerGroupId", text: "控制器号" },        
    ],

    constructor: function (config) {
        var configBase = {
            store: Ext.create({ xtype: 'imms_mfc_ProductionMovingStore' }),
            detailFormClass: 'imms_mfc_productionMoving_ProductionMovingDetailForm',
            detailWindowTitle: '移库记录',
        };
        Ext.applyIf(config, configBase);

        this.callParent(arguments);
    }
});