Ext.define("app.model.imms.material.MaterialStockModel", {
    extend: "app.model.TrackableModel",
    fields: [
        { name: "materialId", type: "int" },
        { name: "materialCode", type: "string" },
        { name: "materialName", type: "string" },

        { name: "storeId", type: "int" },
        { name: "storeCode", type: "string" },
        { name: "storeName", type: "string" },

        { name: "qtyStock", type: "int" },
        { name: "qtyMoveIn", type: "int" },
        { name: "qtyBackIn", type: "int" },
        { name: "qtyBackOut", type: "int" },
        { name: "qtyConsumeGood", type: "int" },
        { name: "qtyConsumeDefect", type: "int" },
        { name: "qtyGood", type: "int" },
        { name: "qtyDefect", type: "int" },
        { name: "qtyMoveOut", type: "int" },
    ]
});