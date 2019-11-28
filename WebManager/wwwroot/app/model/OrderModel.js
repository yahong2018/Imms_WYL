Ext.define("app.model.OrderModel", {
    extend: "app.model.TrackableModel",
    fields: [
        { name: "orderNo", type: "string" },
        { name: "orderStatus", type: "int" },

        
    ]
});