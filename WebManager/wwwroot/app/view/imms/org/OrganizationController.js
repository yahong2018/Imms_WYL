Ext.define("app.view.imms.org.OrganizationController",{
    extend: 'Ext.app.ViewController',
    alias:"controller.imms_org_OrganizationController",
    
    worklineGridSelectionChanged: function (model, selected, index) {
        var grid = this.getView().down('org_workstation_Workstation');
        grid.getStore().parent = selected;
        grid.getStore().getAllByParent();       
    },

    workshopGridSelectionChanged: function (model, selected, index) {
        var grid = this.getView().down('org_workline_Workline');
        grid.getStore().parent = selected;
        grid.getStore().getAllByParent();
    },    
});