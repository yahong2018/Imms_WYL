Ext.define("app.view.imms.mfc.rfidCard.ExcelImportWindow", {
    extend: 'Ext.window.Window',
    xtype: "imms_mfc_rfidCard_ExcelImportWindow",
    closeAction: "destroy",
    modal: true,
    width: 500,
    title: "Excel 导入",
    bodyPadding: 5,
    store:null,
    items: [{
        items: [
            {
                xtype: "form",
                items: [
                    {
                        xtype: "fileuploadfield",
                        fieldLabel: "Excel文件",
                        name: "excelFile",
                        width: '100%'
                    }
                ]
            }
        ]
    }],
    buttons: [
        '->'
        , {
            text: '上传',
            handler: function () {
                var me = this.up('imms_mfc_rfidCard_ExcelImportWindow');
                var formCmp = me.down('form');
                var form = formCmp.getForm();
                form.submit({
                    url: "api/imms/mfc/rfidCard/importExcel",
                    success: function (form, action) {
                        var store = me.store;
                        Ext.MessageBox.show({
                            title: '系统提示',
                            msg: '已成功导入',
                            buttons: Ext.MessageBox.OK,
                            icon: Ext.MessageBox.INFO,
                        });

                        me.store.load();
                    },
                    failure: function (form, action) {
                        debugger;

                        var message = Ext.decode(action.response.responseText.trim());
                        Ext.MessageBox.show({
                            title: '系统提示',
                            msg: message.data.message,
                            buttons: Ext.MessageBox.OK,
                            icon: Ext.MessageBox.ERROR,
                        });
                    }
                }
                );
            }
        }, {
            text: '取消',
            handler: function () {
                var me = this.up('imms_mfc_rfidCard_ExcelImportWindow');
                me.close();
                me.destroy();
            }
        }
    ]

});