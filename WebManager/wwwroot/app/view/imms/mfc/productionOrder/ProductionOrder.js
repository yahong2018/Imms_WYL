Ext.define("app.view.imms.mfc.productionOrder.ProductionOrder",{
    extend:"app.ux.dbgrid.DbGrid",
    xtype:"app_view_imms_mfc_productionOrder_ProductionOrder",
    requires:["app.model.imms.mfc.ProductionOrderModel","app.store.imms.mfc.ProductionOrderStore"],
    uses:["app.view.imms.mfc.productionOrder.ProductionOrderDetailForm"],

    columns:[
        { dataIndex: "orderNo", text: '计划单号', width: 150 },
        { dataIndex: "orderStatus", text: '状态', width: 150 },
        { dataIndex: "productionCode", text: '产品编号', width: 150 },
        { dataIndex: "productionName", text: '产品名称', width: 150 },

        // { dataIndex: "workshopCode", text: '车间编号', width: 150 },
        // { dataIndex: "workshopName", text: '车间名称', width: 150 },

        { dataIndex: "planDate", text: '计划生产日期', width: 150 },
        { dataIndex: "qtyPlanned", text: '计划数量', width: 150 },
        { dataIndex: "qtyGood", text: '完工数', width: 150 },
        { dataIndex: "qtyBad", text: '次品数', width: 150 },                
    ],

    constructor: function (config) {
        var configBase = {
            store: Ext.create({ xtype: 'imms_mfc_ProductionOrderStore' }),
            detailFormClass: 'imms_mfc_productionOrder_ProductionOrderDetailForm',
            detailWindowTitle: '生产计划',
        };
        Ext.applyIf(config, configBase);

        this.callParent(arguments);
    }
});