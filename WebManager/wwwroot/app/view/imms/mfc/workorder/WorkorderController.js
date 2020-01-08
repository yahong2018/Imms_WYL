Ext.define('app.view.imms.mfc.workorder.WorkorderController', {
    extend: 'Ext.app.ViewController',
    alias: 'controller.imms_mfc_workorder_WorkorderController',
    // uses: ["app.ux.excel.Importer"],
    startOrder: function () {
        var url = 'api/imms/mfc/workorder/start'
        var grid = this.getView()
        var record = grid.getSelectedRecord()
        if (record == null) {
            Ext.Msg.alert('系统提示', '请先选定一条记录!')
            return
        }
        app.ux.Utils.ajaxRequest({
            url: url,
            method: 'POST',
            jsonData: record.data,
            successCallback: function (result, response, opts) {
                grid.store.load({
                    callback: function (records, operation, success) {
                        Ext.toast({
                            html: '工单已开工',
                            title: '系统提示',
                            width: 200,
                            align: 't'
                        });

                        var button = grid.down("[buttonId='BTN_START']");
                        button.setDisabled(true);

                        button = grid.down("[btnName='btnDelete']");
                        button.setDisabled(true);
                    }
                });
            }
        })
    },
    // importOrder: function () {
    //     var importer = Ext.create({ xtype: "app_ux_excel_impoter" });
    //     importer.show();
    // },

    gridSelectionChanged: function (model, selected, index) {
        if (selected.length == 0) {
            return
        }
        var record = selected[0]
        var status = record.get('orderStatus')

        //只有已计划订单才可以启用
        var button = this.getView().down("[buttonId='BTN_START']")
        button.setDisabled(status != 0);

        //只有已计划订单才可以删除
        button = this.getView().down("[btnName='btnDelete']");
        button.setDisabled(status != 0);
    }
})
