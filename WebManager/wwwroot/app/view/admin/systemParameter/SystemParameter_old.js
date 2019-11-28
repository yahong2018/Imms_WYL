Ext.define('app.view.admin.systemParameter.SystemParameter_old', {
    extend: 'Ext.form.FormPanel',
    xtype: 'admin_systemParameter_SystemParameter_old',
    bodyPadding: 5,
    frame: true,
    items: [
        {
            xtype: 'fieldset',
            title: "与ERP同步参数",
            width: 680,
            items: [                
                {
                    xtype: "container",
                    layout: "hbox",
                    items: [
                        {
                            xtype: "container",
                            width: 300,
                            items: [
                                {
                                    xtype: "textfield",
                                    name: "grant_type",
                                    fieldLabel: "grant_type",
                                },
                                {
                                    xtype: "textfield",
                                    name: "client_id",
                                    fieldLabel: "client_id",
                                }, {
                                    xtype: "textfield",
                                    name: "client_secret",
                                    fieldLabel: "client_secret",
                                }, {
                                    xtype: "textfield",
                                    name: "username",
                                    fieldLabel: "username",
                                }, {
                                    xtype: "textfield",
                                    name: "password",
                                    fieldLabel: "password",
                                }
                            ]
                        },
                        {
                            xtype: "container",
                            defaults: {
                                labelWidth: 150,
                            },
                            items: [
                                {
                                    xtype: "textfield",
                                    name: "account_id",
                                    fieldLabel: "账套Id",
                                },
                                {
                                    xtype: "textfield",
                                    name: "last_sync_id_progress",
                                    fieldLabel: "最后报工数据同步的Id",
                                }, {
                                    xtype: "textfield",
                                    name: "last_sync_id_progress_ww",
                                    fieldLabel: "最后委外数据同步的Id",
                                }, {
                                    xtype: "textfield",
                                    name: "last_sync_id_move",
                                    fieldLabel: "最后移库数据同步的Id",
                                }, {
                                    xtype: "textfield",
                                    name: "last_sync_id_qualitycheck",
                                    fieldLabel: "最后品质数据同步的Id",
                                }
                            ]
                        }
                    ]
                },
                {
                    xtype: "container",
                    defaults: {
                        labelWidth: 150,
                        width: 625,
                    },
                    items: [
                        {
                            xtype: "textfield",
                            name: "server_host",
                            fieldLabel: "服务器地址",                             
                        },
                        {
                            xtype: "textfield",
                            name: "login_url",
                            fieldLabel: "登录地址",
                            
                        }, {
                            xtype: "textfield",
                            name: "progress_report_url",
                            fieldLabel: "入库报工数据同步地址",
                            
                            
                        }, {
                            xtype: "textfield",
                            name: "moving_report_url",
                            fieldLabel: "移库数据同步地址",
                        }, {
                            xtype: "textfield",
                            name: "qualitycheck_report_url",
                            fieldLabel: "品质数据同步地址",
                        },
                    ]
                },
                {
                    xtype: "container",
                    layout: "hbox",
                    defaults: {
                        width: 80,
                    },
                    items: [
                        {
                            xtype: "button",
                            text: "立即同步",
                            handler: function () {
                                app.ux.Utils.ajaxRequest({
                                    url: "api/admin/systemParameter/sync_wdb",
                                    method: "GET",
                                    successCallback: function (result, response, opts) {
                                        debugger;
                                        alert(result);
                                    }
                                })
                            }
                        },
                        {
                            xtype: "button",
                            text: "保存参数",
                            margin: "0 0 5 75",
                            handler: function () {
                            }
                        },
                    ]
                },
            ]
        }
    ],
    listeners:{
        afterrender:function(){

        }
    }
});