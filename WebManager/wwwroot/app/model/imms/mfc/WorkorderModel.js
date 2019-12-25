Ext.define("app.model.imms.mfc.WorkorderModel", {
    extend: "app.model.EntityModel",
    requires: ["app.ux.ZhxhDate"],
    fields: [
        { name: "orderNo", type: "string" },
        { name: "orderStatus", type: "int" },
        { name: "lineNo", type: "string" },
        { name: "customerNo", type: "string" },
        { name: "partNo", type: "string" },
        { name: "partName", type: "string" },
        { name: "qtyReq", type: "int" },
        { name: "uph", type: "int" },
        { name: "workerCount", type: "int" },
        { name: "qtyGood", type: "int" },
        { name: "qtyBad", type: "int" },
        { name: "timeStartPlan", type: "zhxhDate", dateFormat: "Y-m-d H:i" },
        { name: "timeEndPlan", type: "zhxhDate", dateFormat: "Y-m-d H:i" },
        { name: "timeStartActual", type: "zhxhDate", dateFormat: "Y-m-d H:i" },
        { name: "timeEndActual", type: "zhxhDate", dateFormat: "Y-m-d H:i" },
    ]
});