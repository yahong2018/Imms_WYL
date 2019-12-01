Ext.define("app.store.imms.org.OrgStore", {
    extend: "app.store.BaseStore",
    parent: null,
    getAllByParent: function () {
        if (this.parent == null) {
            return;
        }

        var filter = {
            L: "parentId",
            O: "=",
            R: this.parent.get("recordId")
        };

        this.clearCustomFilter();
        this.addCustomFilter(filter);
        this.buildFilterUrl();

        this.load();
    }
});

Ext.define("app.store.imms.org.WorkshopStore", {
    extend: "app.store.imms.org.OrgStore",
    model: 'app.model.imms.org.WorkshopModel',
    alias: 'widget.imms_org_WorkshopStore',

    dao: {
        deleteUrl: 'imms/org/workshop/delete',
        insertUrl: 'imms/org/workshop/create',
        updateUrl: 'imms/org/workshop/update',
        selectUrl: 'imms/org/workshop/getAll',
    }
});


Ext.define("app.store.imms.org.WorklineStore", {
    extend: "app.store.imms.org.OrgStore",
    model: 'app.model.imms.org.WorklineModel',
    alias: 'widget.imms_org_WorklineStore',

    dao: {
        deleteUrl: 'imms/org/workline/delete',
        insertUrl: 'imms/org/workline/create',
        updateUrl: 'imms/org/workline/update',
        selectUrl: 'imms/org/workline/getAll',
    }
});


Ext.define("app.store.imms.org.WorkstationStore", {
    extend: "app.store.imms.org.OrgStore",
    model: "app.model.imms.org.WorkstationModel",
    alias: 'widget.imms_org_WorkstationStore',

    dao: {
        deleteUrl: 'imms/org/workstation/delete',
        insertUrl: 'imms/org/workstation/create',
        updateUrl: 'imms/org/workstation/update',
        selectUrl: 'imms/org/workstation/getAll',
    }
});