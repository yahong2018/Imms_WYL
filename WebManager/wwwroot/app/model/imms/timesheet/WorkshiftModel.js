Ext.define("app.model.imms.timesheet.WorkshiftModel", {
    extend: "app.model.EntityModel",
    fields: [
        { name: "shiftCode", type: "string" },
        { name: "shiftName", type: "string" },
        { name: "shiftStatus", type: "int" },
    ]
});