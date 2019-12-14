Ext.define("app.store.imms.timesheet.WorkshiftStore",{
    extend:"app.store.BaseStore",
    model:"app.model.imms.timesheet.WorkshiftModel",
    alias:"widget.imms_timesheet_WorkshiftStore",
    dao:{
        deleteUrl: 'imms/timesheet/workshift/delete',
        insertUrl: 'imms/timesheet/workshift/create',
        updateUrl: 'imms/timesheet/workshift/update',
        selectUrl: 'imms/timesheet/workshift/getAll',
    }
})