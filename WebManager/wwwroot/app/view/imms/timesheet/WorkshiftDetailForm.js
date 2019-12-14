Ext.define("app.view.imms.timesheet.WorkshiftDetailForm", {
    extend: "Ext.form.Panel",
    xtype: "app_view_imms_timesheet_WorkshiftDetailForm",
    width:400,
    bodyPadding:5,
    items: [
        {
            xtype: "hidden",
            name: "recordId"
        },
        {
            xtype: "textfield",
            name: "shiftCode",
            fieldLabel: "班次代码",
            allowBlank: false,
            width: 380
        }, {
            xtype: "textfield",
            name: "shiftName",
            fieldLabel: "班次名称",
            width: 380,
            allowBlank: false,
        },
        {
            xtype: "container",
            layout: "hbox",
            margin: '0 0 3 ',
            items: [
                {
                    xtype: "textfield",
                    name: "shiftStatus",
                    fieldLabel: "是否停用",
                    width: 250,
                    allowBlank: false,
                }, {
                    xtype: "label",
                    text: "0.启用  1.停用",
                    margin: '8 20 5 5', flex: 0.8, readOnly: true
                }
            ]
        }
    ]
});