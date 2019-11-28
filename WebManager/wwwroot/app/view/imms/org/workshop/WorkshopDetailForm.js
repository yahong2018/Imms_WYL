Ext.define("app.view.imms.org.workshop.WorkshopDetailForm", {
    extend: "app.ux.TrackableFormPanel",
    xtype: "imms_org_workshop_WorkshopDetailForm",
    requires: ["app.model.imms.org.WorkshopModel"],
    width: 600,
    bodyPadding: 5,
    defaults: {
        layout: "anchor",
        anchor: "100%",
    },
    items: [
         {
            name: "orgCode",
            xtype: "textfield",
            fieldLabel: "车间代码",
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
        },
        {
            xtype:"container",
            layout:"hbox",
            margin: "3 0 5 0",
            items:[
                {
                    xtype:"textfield",
                    fieldLabel:"车间类型",
                    name:"workshopType",
                    allowBlank:false,
                    width:150,
                },{
                    xtype:"label",
                    text:"1. 内部车间 3.外发前工程车间 4.外发车间 5.外发后工程车间",
                    flex:0.8,
                    margin:"8 0 3 3"
                }
            ]
        },
        {
            name:"operationIndex",
            xtype:"textfield",
            fieldLabel:"本工序",
            width:380,
        },
        {
            name: "prevOperationIndex",
            xtype: "textfield",
            fieldLabel: "上工序",
            width: 380,
        },
         {
            name: "description",
            xtype: "textarea",
            fieldLabel: "备注",
            width: 380
        }
    ]
});