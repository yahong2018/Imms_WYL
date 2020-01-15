Ext.define("app.view.imms.mfc.workorder.WorkorderImportForm", {
    extend: "Ext.window.Window",
    xtype: "app_view_imms_mfc_workorder_WorkorderImportForm",
    width: 600,
    items: [
        {
            xtype: "form",
            layout: 'anchor',
            defaults: {
                anchor: '100%',
            },
            margin: 5,
            items: [

                {
                    xtype: "label",
                    html: "<p >所上传文件必须符合格式，<a href='resources/templates/工单导入模板.xlsx' target='_blank'>单击下载模板</a>，根据模板填写以减少错误。</p>"
                },
                {
                    xtype: 'fileuploadfield',
                    fieldLabel: "待导入文件",
                    name: "excelFile",
                },
                {
                    xtype: "container",
                    layout: "hbox",
                    items: [
                        {
                            xtype: "textfield",
                            fieldLabel: "数据开始行",
                            value: 3,
                            name: "rowStart",
                            allowBlank: false
                        },
                        {
                            xtype: "textfield",
                            fieldLabel: "数据结束行",
                            value: 4,
                            name: "rowEnd",
                            allowBlank: false
                        }
                    ]
                },
                {
                    xtype: "container",
                    layout: "hbox",
                    items: [
                        {
                            xtype: "textfield",
                            fieldLabel: "错误处理",
                            value: 0,
                            name: "errorHandle",
                            allowBlank: false
                        },
                        {
                            xtype: "label",
                            text: "0.不导入   1.忽略错误"
                        }
                    ]
                }
            ]
        }
    ],
    closeAction: 'destroy',
    modal: true,
    maximizable: true,
    minimizable: true,
    title: "工单导入",
    buttons: [
        {
            text: "取消",
            handler: function () {
                this.up('app_view_imms_mfc_workorder_WorkorderImportForm').close();
            }
        },
        {
            text: "导入",
            handler: function () {
                var win = this.up('app_view_imms_mfc_workorder_WorkorderImportForm');
                var form = win.down('form');
                if (!form.down('fileuploadfield').value) {
                    Ext.Msg.alert("系统提示", "请先选择需要上传的文件!");
                    return;
                }
                var rowStart = form.down("[name='rowStart']").value;
                var rowEnd = form.down("[name='rowEnd']").value;
                if (rowStart == "" || isNaN(rowStart)
                    || rowEnd == "" || isNaN(rowEnd)
                ) {
                    Ext.Msg.alert("系统提示", "请设置正确的[数据开始行]和[数据结束行]!");
                    return;
                }

                form.submit({
                    url: "api/imms/mfc/workorder/import",
                    success: function (form, action) {
                        var result = Ext.decode(action.response.responseText);
                        Ext.Msg.alert("系统提示", result.data);
                        if (result.success) {                            
                            win.close();
                        }
                    }
                });
            }
        }
    ]
});