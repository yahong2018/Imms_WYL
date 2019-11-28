Ext.define("app.model.imms.org.OperatorModel", {
    extend: "app.model.TrackableModel",
    fields: [
        { name: "orgName", type: "string" },
        { name: "orgId", type: "int" },
        { name: "orgCode", type: "string" },
        { name: "employeeId", type: "string" },
        { name: "employeeName", type: "string" },
        { name: "employeeCardNo", type: "string" },
    ]
});