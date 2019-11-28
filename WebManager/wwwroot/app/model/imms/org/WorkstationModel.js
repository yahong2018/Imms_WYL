Ext.define("app.model.imms.org.WorkstationModel", {
    extend: "app.model.imms.org.OrganizationModel",
    fields: [
        { name: "rfidControllerId", type: "int" },
        { name: "rfidTerminatorId", type: "int" },
        { name: "wocgCode", type: "string" },
        { name: "rfidTemplateIndex", type: "int" },
        { name: "autoReportCount", type: "int" },
    ]
});