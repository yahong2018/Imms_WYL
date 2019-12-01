Ext.define("app.view.imms.org.OrganizationController",{
    extend: 'Ext.app.ViewController',
    alias:"controller.imms_org_OrganizationController",
    gridSelectionChanged: function (model, selected, index) {
        this.getView().down('org_workstation_Workstation').getStore().parent = selected;
        this.getView().down('org_workstation_Workstation').getStore().getAllByParent();
    },
});