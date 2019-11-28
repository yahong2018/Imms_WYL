Ext.define("app.view.imms.mfc.simulator.LineYZ", {
    extend: "Ext.container.Container",
    xtype: "app_view_imms_mfc_simulator_LineYZ",
    requires: ["app.view.imms.mfc.simulator.RfidTerminator"],
    margin: 5,
    items: [
        {
            xtype: "panel",
            layout: "hbox",
            margin:5,
            items: [
                {
                    xtype: "imms_mfc_simulator_RfidTerminator",
                    workstationName: "压铸1号线",
                    GID: 3,
                    DID: 1,
                },
                {
                    xtype: "imms_mfc_simulator_RfidTerminator",
                    workstationName: "压铸2号线",
                    GID: 3,
                    DID: 2,
                },
            ]
        },
        {
            xtype: "panel",
            layout: "hbox",
            margin:5,
            items: [
                {
                    xtype: "imms_mfc_simulator_RfidTerminator",
                    workstationName: "压铸3号线",
                    GID: 3,
                    DID: 3
                },
                {
                    xtype: "imms_mfc_simulator_RfidTerminator",
                    workstationName: "压铸4号线",
                    GID: 3,
                    DID: 4
                }]
        },
        {
            xtype: "panel",
            layout: "hbox",
            margin:5,
            items: [
                {
                    xtype: "imms_mfc_simulator_RfidTerminator",
                    workstationName: "压铸5号线",
                    GID: 3,
                    DID: 5
                },
                {
                    xtype: "imms_mfc_simulator_RfidTerminator",
                    workstationName: "压铸6号线",
                    GID: 3,
                    DID: 6
                }]
        },
        {
            xtype: "imms_mfc_simulator_RfidTerminator",
            workstationName: "压铸7号线",
            margin:"5 0 0 5",
            GID: 3,
            DID: 7
        }

    ]
});