Ext.define("app.model.imms.mfc.WorkstationProductSummaryModel", {
    extend: "app.model.EntityModel",
    fields: [
        { name: "orderNo", type: "string" },
        { name: "lineNo", type: "string" },
        { name: "partNo", type: "string" },
        { name: "workstationCode", type: "string" },
        { name: "qtyGood", type: "int" },
        { name: "qtyBad", type: "int" },
    ]
});