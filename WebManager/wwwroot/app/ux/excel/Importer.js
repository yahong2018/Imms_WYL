Ext.define("app.ux.excel.Importer", {
    extend: "Ext.window.Window",
    xtype: "app_ux_excel_impoter",
    requires: [
        'Ext.layout.container.Card', "app.ux.excel.ImporterViewModel", "app.ux.excel.ImporterController"
        , "app.ux.excel.cards.File",  "app.ux.excel.cards.FieldSetting",
        "app.ux.excel.cards.RowSetting", "app.ux.excel.cards.Complete"        
    ],
    viewModel: {
        type: "excelImportViewModel",
    },
    controller: {
        type: "excelImporterController"
    },
    closeAction: 'destroy',
    modal: true,
    maximizable: true,
    minimizable: true,    
    height:500,
    width:800,
    bodyPadding:10,
    layout:"fit",
    title:"文件选择",
    items: [
        {
            xtype: "form",
            layout: "card",
            items: [
                {
                    xtype: "app_ux_excel_cards_File"
                }, {
                    xtype: "app_ux_excel_cards_FieldSetting"
                },{
                    xtype: "app_ux_excel_cards_RowSetting"
                },{
                    xtype: "app_ux_excel_cards_Complete"
                }
            ]
        }
    ],
    buttons: [
        {
            text: "取消",
            handler: "cancelButtonClick"
        },
        "->",
        {
            text: "上一步",
            buttonId: "btnPrev",
            disabled:true,
            handler: "prevButtonClick"
        }, {
            text: "下一步",
            buttonId: "btnNext",
            handler: "nextButtonClick"
        }
    ],
    listeners:{
        afterrender:function(){
           // this.maximize(true);
        }
    }
});