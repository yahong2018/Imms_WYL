Ext.define("app.model.imms.timesheet.WorkshiftSpanModel",{
    extend:"app.model.EntityModel",
    fields:[
        { name:"workshiftId",type:"int"},
        { name: "seq", type: "int" },
        { name: "timeBegin", type: "string" },
        { name: "timeEnd", type: "string" },
        { name: "isBreak", type: "int" },
        { name: "isShowOnKanban", type: "int" },
    ]
});