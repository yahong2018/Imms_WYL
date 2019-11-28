Ext.define("app.view.imms.rpt.rptProductionOrderProgress.RptProductionOrderProgress", {
    extend: "Ext.panel.Panel",
    xtype: "app_view_imms_rpt_rptProductionOrderProgress_RptProductionOrderProgress",
    requires: ["app.view.imms.rpt.rptProductionOrderProgress.RptProductionOrderProgressGrid"],
    layout:"fit",
    items:[
        {
            xtype: "app_view_imms_rpt_rptProductionOrderProgress_RptProductionOrderProgressGrid" 
        }
    ]
})