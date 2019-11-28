Ext.define("app.model.imms.org.OrganizationModel", {
    extend: 'app.model.EntityModel',      
    fields: [     
        { name: "orgCode",  type: "string" },
        { name: "organizationName", type: "string" },
        { name: "description",  type: "string" },

        { name: "parentId",  type: "int" },
        { name: "parentCode", type: "string" },
        { name: "parentName", type: "string" },
    ]
});