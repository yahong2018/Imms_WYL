Ext.define("app.view.imms.mfc.simulator.LineCJG", {
    extend: "Ext.container.Container",
    xtype: "app_view_imms_mfc_simulator_LineCJG",
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
                    workstationName: "粗加工1号线",
                    GID: 3,
                    DID: 8,
                },
                {
                    xtype: "imms_mfc_simulator_RfidTerminator",
                    workstationName: "粗加工2号线",
                    GID: 3,
                    DID: 9,
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
                    workstationName: "粗加工3号线",
                    GID: 3,
                    DID: 10
                },
                {
                    xtype: "imms_mfc_simulator_RfidTerminator",
                    workstationName: "粗加工4号线",
                    GID: 3,
                    DID: 11
                }]
        },        

    ]
});