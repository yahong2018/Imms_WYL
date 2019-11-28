Ext.define("app.view.imms.mfc.rfidCard.RfidCard", {
    extend: "app.ux.dbgrid.DbGrid",
    xtype: "app_view_imms_mfc_rfidCard_RfidCard",
    requires: ["app.model.imms.mfc.RfidCardModel", "app.store.imms.mfc.RfidCardStore",
        "app.model.imms.material.MaterialModel", "app.store.imms.material.MaterialStore",
        "app.model.imms.org.WorkshopModel", "app.store.imms.org.WorkshopStore",],
    uses: ["app.view.imms.mfc.rfidCard.RfidCardDetailForm", "app.view.imms.mfc.rfidCard.ExcelImportWindow"
    ],
    columns: [
        { dataIndex: "kanbanNo", text: '看板编号', width: 100 },
        { dataIndex: "rfidNo", text: 'RFID卡号', width: 100 },
        {
            dataIndex: "cardType", text: '看板类型', width: 100, renderer: function (v) {
                var labels = ["", "", "2.工程内看板", "3.外发看板"];

                return labels[v];
            }
        },
        { dataIndex: "towerNo", text: "外发塔编号", width: 100 },
        {
            dataIndex: "cardStatus", text: '看板状态', width: 120, renderer: function (v) {
                var lables = ["0.未使用", "1.已派发", "2.已退回", "3.已绑定", "4", "5", "6", "7", "8", "9", "10.已报工",
                    "11", "12", "13", "14", "15", "16", "17", "18", "19", "20.已移库(外发)",
                    "21", "22", "23", "24", "25", "26", "27", "28", "29", "30.已回厂",
                    "31", "32", "33", "34", "35", "36", "37", "38", "39", "40.已回厂投入"
                ];
                if (v == 255) {
                    return "255.已作废";
                }
                return lables[v];
            }
        },
        { dataIndex: "workshopCode", text: '车间编号', width: 100 },
        { dataIndex: "workshopName", text: '车间名称', width: 100 },
        { dataIndex: "productionCode", text: '产品编号', width: 150 },
        { dataIndex: "productionName", text: '产品名称', width: 150 },
        { dataIndex: "issueQty", text: '派发数量', width: 100 },
        { dataIndex: "stockQty", text: '完工数量', width: 100 },
    ],
    // additionToolbarItems: [
    //     '-',
    //     // {
    //     //     text: '打印条码', privilege: "PRINT", handler: function () {
    //     //         var grid = this.up("app_view_imms_mfc_rfidCard_RfidCard");
    //     //         var records = grid.getSelectionModel().getSelection();
    //     //         if (records.length == 0) {
    //     //             Ext.Msg.alert("系统提示", "请先选定需要打印的Rfid卡！");
    //     //             return;
    //     //         };
    //     //         var idList = [];
    //     //         for (var i = 0; i < records.length; i++) {
    //     //             idList.push(records[i].get("recordId"));
    //     //         }
    //     //         var strIdList = idList.join(",");
    //     //         var encodedStr = Ext.util.Base64.encode(strIdList);
    //     //         window.open("api/imms/mfc/rfidCard/printBarCode?idList=" + encodedStr, "_blank");
    //     //     }
    //     // },
    //     {
    //         text: "看板导入", privilege: "ExcelImport", handler: function () {
    //             var win = Ext.create({ xtype: "imms_mfc_rfidCard_ExcelImportWindow" });
    //             win.store = this.up("app_view_imms_mfc_rfidCard_RfidCard").store;
    //             win.show();
    //         }
    //     }
    // ],
    constructor: function (config) {
        var configBase = {
            store: Ext.create({ xtype: 'imms_mfc_RfidCardStore' }),
            detailFormClass: 'imms_mfc_rfidCard_RfidCardDetailForm',
            detailWindowTitle: '看板RFID卡',
        };
        Ext.applyIf(config, configBase);

        this.callParent(arguments);
    }
});