Ext.define("app.model.imms.org.OperatorModel", {
    extend: "app.model.EntityModel",
    fields: [
        { name: "orgCode", type: "string" },
        { name: "empId", type: "string" },
        { name: "empName", type: "string" },
        { name: "title", type: "string" },
        { name: "pic", type: "string" },
        { name: "seq", type: "int" },
    ]
});