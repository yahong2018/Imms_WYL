Ext.define("app.view.imms.org.OrganizationController", {
    extend: 'Ext.app.ViewController',
    alias: "controller.imms_org_OrganizationController",

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

    openLineKanban: function () {        
        var grid = this.getView().down('org_workline_Workline');
        var record = grid.getSelectedRecord();
        if (record == null) {
            Ext.Msg.alert("系统提示", "请先选择1条需要监控的产线！");
            return;
        }
        window.open('kanban/line?lineNo=' + record.get("lineCode"), '_blank', 'fullscreen=1');
    },

    openWorkshopKanban: function () {
        var grid = this.getView().down('org_workshop_Workshop');
        var record = grid.getSelectedRecord();
        if (record == null) {
            Ext.Msg.alert("系统提示", "请先选择一个需要监控的工场！");
            return;
        }
        window.open('kanban/workshop?workshopCode=' + record.get("orgCode"), '_blank', 'fullscreen=1');
    },

    openPlantKanban: function () {
        window.open('kanban/factory', '_blank', 'fullscreen=1');
    }
});