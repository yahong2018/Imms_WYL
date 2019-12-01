Ext.define("app.view.imms.org.workline.WorklineDetailForm", {
    extend: "app.ux.TrackableFormPanel",
    xtype: "imms_org_workline_WorklineDetailForm",
    requires: ["app.model.imms.org.WorklineModel"],
    width: 400,
    bodyPadding: 5,
    items: [
         {
            name: "orgCode",
            xtype: "textfield",
            fieldLabel: "产线代码",
            allowBlank: false,
            maxLength: 20,
            enforceMaxLength: true,
            width: 250
        }, {
            name: "orgName",
            xtype: "textfield",
            fieldLabel: "产线名称",
            allowBlank: false,
            maxLength: 50,
            enforceMaxLength: true,
            width: 380
        },        
        {
            name:"gid",
            xtype:"textfield",
            fieldLabel:"组号",
            width:380,
        },
        {
            name: "did",
            xtype: "textfield",
            fieldLabel: "机号",
            width: 380,
        }         
    ]
});