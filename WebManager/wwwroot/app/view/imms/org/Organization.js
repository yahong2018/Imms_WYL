Ext.define("app.view.imms.org.Organization", {
    extend: 'Ext.panel.Panel',
    xtype: 'app_view_imms_org_Organization',
    requires: ["app.model.imms.org.WorklineModel","app.model.imms.org.WorkstationModel",
        "app.view.imms.org.workline.Workline", "app.view.imms.org.workstation.Workstation","app.view.imms.org.OrganizationController"
    ],   
    controller:{
        type:"imms_org_OrganizationController"
    },
    layout: 'fit',
    items: [
        {
            xtype: 'panel',
            frame: false,
            layout: 'border',
            items: [
                {
                    region: 'west',
                    xtype: 'org_workline_Workline',
                    width: 400,
                }, {
                    region: 'center',
                    xtype: 'org_workstation_Workstation'
                }
            ]
        }
    ]
});