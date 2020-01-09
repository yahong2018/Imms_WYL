Ext.define("app.view.imms.mfc.lineProductSummaryDateSpan.LineProductSummaryDateSpan", {
    extend: "app.ux.dbgrid.DbGrid",
    xtype: "app_view_imms_mfc_lineProductSummaryDateSpan_LineProductSummaryDateSpan",
    requires: ["app.model.imms.mfc.LineProductSummaryDateSpanModel", "app.store.imms.mfc.LineProductSummaryDateSpanStore"],
    columns: [
        { dataIndex: "lineNo", text: "产线" },
        { dataIndex: "workorderNo", text: "订单" },
        { dataIndex: "partNo", text: "Part No", width: 250 },
        { dataIndex: "uph", text: "UPH" },
        { dataIndex: "productDate", text: "日期" },
        { dataIndex: "spanName", text: "时间" },
        { dataIndex: "qtyGood", text: "良品", align: 'right' },
        { dataIndex: "qtyBad", text: "不良", align: 'right' },
        { dataIndex: "qtyTotal", text: "产出", align: 'right' },
        {
            dataIndex: "otd", text: "生产达成率(%)", width: 150, align: 'right', renderer: function (v) {
                if (v > 98) {
                    return v.toFixed(1);
                }
                return '<span style="color:red">' + v.toFixed(1) + '</span>';
            }
        },
        {
            dataIndex: "fail", text: "不良率(%)", align: 'right', renderer: function (v) {
                if (v < 3) {
                    return v.toFixed(1);
                }
                return '<span style="color:red">' + v.toFixed(1) + '</span>';
            }
        },
    ],
    constructor: function (config) {
        var configBase = {
            store: Ext.create({ xtype: 'app_store_imms_mfc_LineProductSummaryDateSpanStore' }),
            //   detailFormClass: 'imms_mfc_workorder_WorkorderDetailForm',
            //   detailWindowTitle: '生产计划',
        };
        Ext.applyIf(config, configBase);

        this.callParent(arguments);
    }
});