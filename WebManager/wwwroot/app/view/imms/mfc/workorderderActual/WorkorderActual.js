Ext.define("app.view.imms.mfc.productionOrderProgress.ProductionOrderProgress", {
    extend: "app.ux.dbgrid.DbGrid",
    xtype: "app_view_imms_mfc_productionOrderProgress_ProductionOrderProgress",
    requires: ["app.model.imms.mfc.ProductionOrderProgressModel", "app.store.imms.mfc.ProductionOrderProgressStore",
        "app.model.imms.mfc.RfidCardModel", "app.store.imms.mfc.RfidCardStore",
        "app.model.imms.org.WorkstationModel", "app.store.imms.org.WorkstationStore",
        "app.model.imms.org.WorkshopModel", "app.store.imms.org.WorkshopStore",
        "app.model.imms.material.MaterialModel", "app.store.imms.material.MaterialStore",
        "app.model.imms.org.OperatorModel", "app.store.imms.org.OperatorStore",
        "app.model.imms.mfc.ProductionOrderModel", "app.store.imms.mfc.ProductionOrderStore"
    ],
    uses: ["app.view.imms.mfc.productionOrderProgress.ProductionOrderProgressDetailForm"],
    columns: [
        // { dataIndex: "productionOrderNo", text: "计划订单",width:150 },
        { dataIndex: "recordId", text: "业务流水" },
        { dataIndex: "workshopCode", text: "车间编码" },
        { dataIndex: "workshopName", text: "车间名称" },

        { dataIndex: "workstationCode", text: "工位编码" },
        { dataIndex: "workstationName", text: "工位名称" },
        { dataIndex: "wocgCode", text: "工作中心组" },        

        { dataIndex: "productionCode", text: "产品编码" ,width:150},
        { dataIndex: "productionName", text: "产品名称", width: 200},
        { dataIndex: "timeOfOrigin", text: '时间', width: 150 },
        { dataIndex: "qty", text: "数量" },

        { dataIndex: "employeeId", text: "工号" },
        { dataIndex: "employeeName", text: "姓名" },

        { dataIndex: "rfidTerminatorId", text: "工位机" },
        { dataIndex: "rfidControllerId", text: "控制器" },
        { dataIndex: "rfidCardNo", text: "卡号" },
        { dataIndex: "reportType", text: "汇报类型" },
    ],

    constructor: function (config) {
        var configBase = {
            store: Ext.create({ xtype: 'imms_mfc_ProductionOrderProgressStore' }),
            detailFormClass: 'imms_mfc_productionOrderProgress_ProductionOrderProgressDetailForm',
            detailWindowTitle: '生产报工',
        };
        Ext.applyIf(config, configBase);

        this.callParent(arguments);
    }
});