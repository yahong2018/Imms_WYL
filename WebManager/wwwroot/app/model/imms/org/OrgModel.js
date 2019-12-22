Ext.define("app.model.imms.org.OrgModel", {
    extend: 'app.model.EntityModel',
    fields: [
        { name: "orgCode", type: "string" },
        { name: "orgName", type: "string" },
        { name: "parentId", type: "int" },
        { name: "gid", type: "int" },
        { name: "did", type: "int" },
    ]
});

Ext.define("app.model.imms.org.WorkshopModel", {
    extend: 'app.model.imms.org.OrgModel',
    field: [
        { name: "workshiftCode", type: "string" }
    ]
});

Ext.define("app.model.imms.org.WorklineModel", {
    extend: 'app.model.imms.org.OrgModel',
});

Ext.define("app.model.imms.org.WorkstationModel", {
    extend: 'app.model.imms.org.OrgModel',
    fields: [
        { name: "seq", type: "int" },
        { name: "defectReportMethod", type: "int" },
    ]
});