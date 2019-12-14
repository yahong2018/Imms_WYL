Ext.define("app.store.imms.timesheet.WorkshiftSpanStore", {
    extend: "app.store.BaseStore",
    model: "app.model.imms.timesheet.WorkshiftSpanModel",
    alias: "widget.imms_timesheet_WorkshiftSpanStore",
    dao: {
        deleteUrl: 'imms/timesheet/workshiftSpan/delete',
        insertUrl: 'imms/timesheet/workshiftSpan/create',
        updateUrl: 'imms/timesheet/workshiftSpan/update',
        selectUrl: 'imms/timesheet/workshiftSpan/getAll',
    },
    getAllByParent:function(){
        if(this.parent == null){
            return;
        }

        var filter = {
            L: "workshiftId",
            O: "=",
            R: this.parent.get("recordId")
        };

        this.clearCustomFilter();
        this.addCustomFilter(filter);
        this.buildFilterUrl();

        this.load();        
    }
})