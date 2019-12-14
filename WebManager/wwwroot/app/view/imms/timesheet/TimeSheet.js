Ext.define("app.view.imms.timesheet.TimeSheet",{
    extend:"Ext.panel.Panel",
    xtype:"app_view_timesheet_TimeSheet",

    requires: ["app.view.imms.timesheet.Workshift","app.view.imms.timesheet.WorkshiftSpan"],

    layout:"border",
    items:[
        {
            region:"west",
            xtype:"app_view_imms_timesheet_Workshift",
            width:400,
        },{
            region:"center",
            xtype:"app_view_imms_timesheet_WorkshiftSpan"
        }
    ]
});