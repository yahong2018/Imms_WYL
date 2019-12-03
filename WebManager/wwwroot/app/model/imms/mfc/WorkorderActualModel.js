Ext.define("app.model.imms.mfc.WorkorderActualModel", {
    extend: "app.model.EntityModel",
    requires: ["app.ux.ZhxhDate"],
    fields: [
        { name: "workorderNo", type: "string" },
        { name: "partNo", type: "string" },
        { name: "workstationCode", type: "string" },
        { name: "qty", type: "int" },
        { name: "recordType", type: "int" },
        { name: "defectCode", type: "string" },
        { name: "reportTime", type: "zhxhDate", dateFormat: "Y-m-d H:i:s" },
    ]
});