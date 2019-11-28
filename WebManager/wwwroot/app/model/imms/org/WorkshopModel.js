Ext.define("app.model.imms.org.WorkshopModel", {
    extend: 'app.model.imms.org.OrganizationModel',
    fields: [
        { name: "operationIndex", type: "int" },
        { name: "prevOperationIndex", type: "int" },
        { name: "workshopType", type: "int" }
    ]
});