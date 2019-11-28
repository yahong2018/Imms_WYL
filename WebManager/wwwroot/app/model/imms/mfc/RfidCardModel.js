Ext.define("app.model.imms.mfc.RfidCardModel", {
    extend: "app.model.TrackableModel",
    fields: [
        { name: "kanbanNo", type: "string" },
        { name: "rfidNo", type: "string" },
        { name: "cardType", type: "int" },
        { name: "cardStatus", type: "int" },

        { name: "productionId", type: "int" },
        { name: "productionCode", type: "string" },
        { name: "productionName", type: "string" },

        { name: "workShopId", type: "int" },
        { name: "workshopCode", type: "string" },
        { name: "workshopName", type: "string" },

        { name: "issueQty", type: "int" },
        { name: "stockQty", type: "int" },

        { name:"towerNo",type:"string"}
    ]
});