Ext.define("app.view.imms.mfc.qualityCheck.QualityCheck", {
    extend: "app.ux.dbgrid.DbGrid",
    xtype: "app_view_imms_mfc_qualityCheck_QualityCheck",
    requires: [
        "app.model.imms.mfc.QualityCheckModel", "app.store.imms.mfc.QualityCheckStore",
        "app.model.imms.mfc.RfidCardModel", "app.store.imms.mfc.RfidCardStore",
        "app.model.imms.org.WorkstationModel", "app.store.imms.org.WorkstationStore",
        "app.model.imms.org.WorkshopModel", "app.store.imms.org.WorkshopStore",
        "app.model.imms.material.MaterialModel", "app.store.imms.material.MaterialStore",
        "app.model.imms.org.OperatorModel", "app.store.imms.org.OperatorStore",
        "app.model.imms.mfc.ProductionOrderModel", "app.store.imms.mfc.ProductionOrderStore",
        "app.model.imms.mfc.DefectModel", "app.store.imms.mfc.DefectStore"
    ],
    uses: ["app.view.imms.mfc.qualityCheck.QualityCheckDetailForm"],
    columns: [
        // { dataIndex: "productionOrderNo", text: "计划单号", width: 100 },
        { dataIndex: "recordId", text: "业务流水" },
        { dataIndex: "productionCode", text: "产品代码", width: 100 },
        { dataIndex: "productionName", text: "产品名称", width: 200 },
        { dataIndex: "qty", text: "数量", width: 100 },
        { dataIndex: "timeOfOrigin", text: "时间", width: 100 },
        { dataIndex: "timeOfOriginWork", text: "工作日", width: 100 },
        { dataIndex: "shiftId", text: "班次", width: 100,renderer:function(value){
            if(value==0){
                return '白班';
            }
            return '夜班';
        } },

        { dataIndex: "defectCode", text: "品质代码", width: 100 },
        { dataIndex: "defectName", text: "品质描述", width: 200 },

        { dataIndex: "workshopCode", text: "车间代码", width: 100 },
        { dataIndex: "workshopName", text: "车间名称", width: 150 },
        { dataIndex: "wocgCode", text: "工作中心组", width: 150 },
    ],
    constructor: function (config) {
        var configBase = {
            detailFormClass: 'imms_mfc_qualityCheck_QualityCheckDetailForm',
            detailWindowTitle: '品质记录',
            store: Ext.create({ xtype: 'imms_mfc_QualityCheckStore' })
        }
        Ext.applyIf(config, configBase);

        this.callParent(arguments);
    }
});