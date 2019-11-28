Ext.define("app.view.admin.systemParameter.SystemParameterDetailForm", {
    extend: "Ext.form.Panel",
    xtype: "admin_systemParameter_SystemParameterDetailForm",
    bodyPadding: 5,
    width:500,
    items: [
        {
            xtype:"hidden",
            name:"recordId"
        },
        {
            xtype: "textfield",
            name: "parameterClassCode",
            fieldLabel:"类别代码",
            readOnly:true,
            width:300
        }, {
            xtype: "textfield",
            name: "parameterClassName",
            fieldLabel: "参数类别",
            readOnly: true,
            width:300,
        }, {
            xtype: "textfield",
            name: "parameterCode",
            fieldLabel: "参数代码",
            readOnly: true,
            width: 300,
        }, {
            xtype: "textfield",
            name: "parameterName",
            fieldLabel: "参数含义",
            readOnly: true,
            width: 300,
        }, {
            xtype: "textarea",
            name: "parameterValue",
            fieldLabel: "参数值",
            width:"100%"
        }
    ]
});