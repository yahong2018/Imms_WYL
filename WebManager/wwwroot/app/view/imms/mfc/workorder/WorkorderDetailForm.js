Ext.define("app.view.imms.mfc.workorder.WorkorderDetailForm", {
    extend: "Ext.form.Panel",
    xtype: "imms_mfc_workorder_WorkorderDetailForm",
    requires: ["app.ux.DateTimeField"],
    padding: 5,
    width: 600,
    layout: "anchor",
    defaults: {
        layout: "anchor",
        anchor: "100%",
    },
    items: [
        {
            name: 'recordId',
            xtype: 'hidden',
        },
        { name: "orderNo", xtype: "textfield", maxLength: 20, enforceMaxLength: true, allowBlank: false, fieldLabel: '工单号', width: 350 },
        {
            xtype: "container",
            layout: "hbox",
            margin: '0 0 3 ',
            items: [
                { name: "orderStatus", xtype: "textfield", fieldLabel: '状态', allowBlank: false, width: 200,readOnly:true },
                { xtype: "label", margin: '8 20 5 15', text: "0.已计划  1.已开工   254.已取消   255.已完工" },
            ]
        },
        { name: "lineNo", xtype: "textfield", fieldLabel: '产线', width: 280, allowBlank: false, },
        {
            xtype: "container",
            layout: "hbox",
            margin: '0 0 3 ',
            items: [
                { name: "partNo", xtype: "textfield", fieldLabel: '产品编号', width: 280, allowBlank: false, },
                { name: "partName", xtype: "textfield", margin: '0 20 0 15', fieldLabel: '产品名称', width: 280, allowBlank: false, },
            ]
        },
        {
            xtype: "container",
            layout: "hbox",
            margin: '0 0 3 ',
            items: [
                { name: "customerNo", xtype: "textfield", fieldLabel: '客户', width: 280, allowBlank: false, },
                { name: "qtyReq", xtype: "textfield", margin: '0 20 0 15', fieldLabel: '计划数量', width: 280, allowBlank: false, },
            ]
        },
        {
            xtype: "container",
            layout: "hbox",
            margin: '0 0 3 ',
            items: [
                { name: "uph", xtype: "textfield", fieldLabel: 'UPH', width: 280, allowBlank: false, },
                { name: "workerCount", xtype: "textfield", margin: '0 20 0 15', fieldLabel: '人数', width: 280, allowBlank: false, },
            ]
        },
        {
            xtype: "container",
            layout: "hbox",
            margin: '0 0 3 ',
            items: [
                { name: "timeStartPlan", xtype: "datetimefield", fieldLabel: '计划开工', width: 280, allowBlank: false, format: 'Y-m-d H:i', },
                { name: "timeEndPlan", xtype: "datetimefield", margin: '0 20 0 15', fieldLabel: '计划完工', width: 280, allowBlank: false, format: 'Y-m-d H:i' },
            ]
        },
        {
            xtype: "container",
            layout: "hbox",
            margin: '0 0 3 ',
            items: [
                { name: "timeStartActual", xtype: "datetimefield", fieldLabel: '实际开工', width: 280, format: 'Y-m-d H:i', },
                { name: "timeEndActual", xtype: "datetimefield", margin: '0 20 0 15', fieldLabel: '实际完工', width: 280, format: 'Y-m-d H:i', },
            ]
        },
        {
            xtype: "container",
            layout: "hbox",
            margin: '0 0 3 ',
            items: [
                { name: "qtyGood", xtype: "textfield", fieldLabel: '良品数', width: 280, allowBlank: false, },
                { name: "qtyBad", xtype: "textfield", margin: '0 20 0 15', fieldLabel: '不良数', width: 280, allowBlank: false, },
            ]
        }
    ]
});