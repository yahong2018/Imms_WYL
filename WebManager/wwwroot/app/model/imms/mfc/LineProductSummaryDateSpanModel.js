Ext.define("app.model.imms.mfc.LineProductSummaryDateSpanModel", {
    extend: "app.model.EntityModel",
    requires: ["app.ux.ZhxhDate"],
    fields: [
        { name: "lineNo", type: "string" },
        { name: "workorderNo", type: "string" },
        { name: "partNo", type: "string" },
        { name: "productDate", type: "zhxhDate", dateFormat: "Y-m-d" },
        { name: "spanId", type: "int" },
        { name: "spanName", type: "string" },
        { name: "uph", type: "int" },
        { name: "qtyGood", type: "int" },
        { name: "qtyBad", type: "int" },
        { name: "qtyTotal", type: "int" },
        { name: "otd", type: "float" },
        { name: "Fail", type: "float" },
    ]
});