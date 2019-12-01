Ext.define("app.view.imms.org.workshop.WorkshopDetailForm", {
    extend: "Ext.form.Panel",
    xtype: "imms_org_workshop_WorkshopDetailForm",
    requires: ["app.model.imms.org.WorkshopModel"],
    width: 400,
    bodyPadding: 5,
    items: [
        {
            xtype:"hidden",
            name:"recordId"
        },
        {
            name: "orgCode",
            xtype: "textfield",
            fieldLabel: "车间编号",
            allowBlank: false,
            maxLength: 20,
            enforceMaxLength: true,
            width: 250
        }, {
            name: "orgName",
            xtype: "textfield",
            fieldLabel: "车间名称",
            allowBlank: false,
            maxLength: 50,
            enforceMaxLength: true,
            width: 380
        }
    ]
});