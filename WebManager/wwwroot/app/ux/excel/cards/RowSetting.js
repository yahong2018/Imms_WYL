Ext.define("app.ux.excel.cards.RowSetting", {
    extend: "Ext.panel.Panel",
    alias: "widget.app_ux_excel_cards_RowSetting",
    layout: 'border',    
    items: [
        {
            region:"north",
            xtype: "panel",
            height:50,
            layout: "hbox",      
            defaults: {
                style: 'font-size:16px',
                fieldStyle: 'font-size:16px',
            },                  
            items: [
                {
                    xtype: "label",
                    html: "数据范围：",
                    margin: "8 0 0 0"
                }, {
                    xtype: "textfield",
                    name: "row_start_index",
                    width: 100
                }, {
                    xtype: "label",
                    html: "~",
                    margin: "8 5 0 5"
                }, {
                    xtype: "textfield",
                    name: "row_end_index",
                    width: 100
                }, {
                    xtype: "button",
                    text: "确定",
                    width: 100,
                    margin: "0 0 0 10"
                }
            ]
        },
        {
            region:"center",
            xtype: "grid",            
            border: true,            
            layout:"fit",
            columns: [
                { xtype: 'rownumberer', width: 30 },
                { dataIndex: "field_1", text: "字段1", width: 100, menuDisabled: true, sortable: false, },
                { dataIndex: "field_2", text: "字段2", width: 100, menuDisabled: true, sortable: false, },
                { dataIndex: "field_3", text: "字段3", width: 100, menuDisabled: true, sortable: false, },
                { dataIndex: "field_4", text: "字段4", width: 100, menuDisabled: true, sortable: false, },
                { dataIndex: "field_5", text: "字段5", width: 100, menuDisabled: true, sortable: false, },
                { dataIndex: "field_6", text: "字段6", width: 100, menuDisabled: true, sortable: false, }
            ],
        },
    ]
});