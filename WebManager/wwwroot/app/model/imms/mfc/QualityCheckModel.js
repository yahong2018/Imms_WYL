Ext.define("app.model.imms.mfc.QualityCheckModel", {
    extend: "app.model.TrackableModel",
    fields: [
        { name: "productionOrderId", type: "int" },
        { name: "productionOrderNo", type: "string" },

        { name: "productionId", type: "int" },
        { name: "productionCode", type: "string" },
        { name: "productionName", type: "string" },

        { name: "workshopId", type: "int" },
        { name: "workshopCode", type: "string" },
        { name: "workshopName", type: "string" },
        { name: "timeOfOrigin", type: "zhxhDate", dateFormat: "Y-m-d H:i:s" },
        { name: "timeOfOriginWork", type: "zhxhDate", dateFormat: "Y-m-d" },
        { name: "shiftId", type: "int"},
        { name: "qty", type: "int" },
        
        { name: "defectId", type: "int" },
        { name: "defectCode", type: "string" },
        { name: "defectName", type: "string" },        
    ]
});