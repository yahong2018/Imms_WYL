Ext.define("app.view.imms.org.Organization", {
    extend: 'Ext.panel.Panel',
    xtype: 'app_view_imms_org_Organization',
    requires: ["app.model.imms.org.OrgModel", "app.store.imms.org.OrgStore", "app.view.imms.org.OrganizationController",
        "app.view.imms.org.workshop.Workshop", "app.view.imms.org.workline.Workline", "app.view.imms.org.workstation.Workstation"
    ],
    controller: {
        type: "imms_org_OrganizationController"
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
                    width: 400,
                    xtype: "org_workshop_Workshop"
                }, {
                    region: 'center',
                    layout: "border",
                    items: [
                        {
                            region: "north",
                            xtype: 'org_workline_Workline',
                            height: 300
                        }, {
                            region: "center",
                            xtype: 'org_workstation_Workstation'
                        }
                    ]
                }
            ]
        }
    ]
});