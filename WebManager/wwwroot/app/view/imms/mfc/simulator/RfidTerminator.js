Ext.define("app.view.imms.mfc.simulator.RfidTerminator", {
    extend: "Ext.panel.Panel",
    xtype: "imms_mfc_simulator_RfidTerminator",
    width: 450,
    border: 1,
    layout: "hbox",
    bodyPadding: 5,
    workstationName: "",
    GID: 0,
    DID: 0,
    items: [
        {
            xtype: "textarea",
            name: "screen",
            height: "100%",
            flex: 0.6
        }, {
            xtype: "container",
            flex: 0.4,
            items: [
                {
                    xtype: "fieldcontainer",
                    layout: "hbox",
                    items: [
                        {
                            xtype: "textfield",
                            name: "cardNo",
                            width: 100,
                            flex: 0.8
                        }, {
                            xtype: "button",
                            text: "刷卡",
                            width: 50,
                            listeners: {
                                click: function () {
                                    var panel = this.up("imms_mfc_simulator_RfidTerminator");
                                    var cardNo = panel.down("[name='cardNo']");
                                    var createTime = Ext.util.Format.date(new Date(), "Y-m-d H:i:s");
                                    
                                    var reportItem ={
                                        isNewData:1,
                                        GID:panel.GID,
                                        DID:panel.DID,
                                        isOffLineData:0,
                                        dataType:1,
                                        dataGatherTime: createTime,
                                        dataMakeTime: createTime,
                                        strPara1:cardNo.value
                                    };

                                    app.ux.Utils.ajaxRequest({
                                        url: "api/imms/mfc/productionOrderProgress/reportProgress?dc=" + new Date().getTime(),
                                        method:"POST",
                                        jsonData:reportItem,
                                        successCallback:function(result, response, opts){
                                            panel.down("[name='screen']").setValue(result.data);
                                        }
                                    });
                                }
                            }
                        }
                    ]
                },
                {
                    xtype: "fieldcontainer",
                    layout: "hbox",
                    items: [
                        {
                            xtype: "textfield",
                            name: "keyboard",
                            width: 100,
                            flex: 0.8
                        }, {
                            xtype: "button",
                            text: "按键",
                            width: 50,
                            listeners: {
                                click: function () {
                                    var panel = this.up("imms_mfc_simulator_RfidTerminator");
                                    var keyboard = panel.down("[name='keyboard']");
                                    var createTime = Ext.util.Format.date(new Date(),"Y-m-d H:i:s");

                                    var reportItem = {
                                        isNewData: 1,
                                        GID: panel.GID,
                                        DID: panel.DID,
                                        isOffLineData: 0,
                                        dataType: 3,
                                        dataGatherTime: createTime,
                                        dataMakeTime: createTime,
                                        strPara1: keyboard.value
                                    };

                                    app.ux.Utils.ajaxRequest({
                                        url: "api/imms/mfc/productionOrderProgress/reportProgress?dc=" + new Date().getTime(),
                                        method: "POST",
                                        jsonData: reportItem,
                                        successCallback: function (result, response, opts) {
                                            panel.down("[name='screen']").setValue(result.data);
                                        }
                                    });
                                }
                            }
                        }
                    ]
                },
                // {
                //     xtype: "fieldcontainer",
                //     layout: "hbox",
                //     items: [
                //         {
                //             xtype: "textfield",
                //             name: "lightIR",
                //             width: 100,
                //             flex: 0.8
                //         }, {
                //             xtype: "button",
                //             text: "光电",
                //             width: 50,
                //             listeners: {
                //                 click: function () {
                //                     var panel = this.up("imms_mfc_simulator_RfidTerminator");
                //                     var lightIR = panel.down("[name='lightIR']");

                //                     alert("lightIR:" + lightIR.value);
                //                 }
                //             }
                //         }
                //     ]
                // }
            ]
        }
    ],
    initComponent: function () {
        this.title = this.workstationName + "(组号:" + this.GID + ",机号:" + this.DID + ")";

        this.callParent(arguments);
    }
});