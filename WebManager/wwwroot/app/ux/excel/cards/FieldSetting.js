Ext.define("app.ux.excel.cards.FieldSetting", {
    extend: "Ext.panel.Panel",
    alias: "widget.app_ux_excel_cards_FieldSetting",
    layout: 'border',
    items: [
        {
            xtype: "panel",
            region: "north",
            layout: "hbox",
            height: 50,
            defaults: {
                style: 'font-size:16px',
                fieldStyle: 'font-size:16px',
            },
            items: [
                {
                    xtype: "label",
                    text: "字段行：",
                    margin: "8 0 0 0"
                }, {
                    xtype: "textfield",
                    name: "field_row_index",
                    width: 100
                }, {
                    xtype: "label",
                    text: "列范围：",
                    margin: "8 0 0 20"
                }, {
                    xtype: "textfield",
                    name: "field_column_start_index",
                    width: 100
                }, {
                    xtype: "label",
                    text: "~",
                    margin: "8 5 0 5"
                }, {
                    xtype: "textfield",
                    name: "field_column_end_index",
                    width: 100
                }, {
                    xtype: "button",
                    text: "确定",
                    width: 100,
                    margin: "0 0 0 10",
                    handler: 'getExcelFields'
                }
            ]
        },
        {
            xtype: "panel",
            region: "center",
            layout: "border",
            items: [
                {
                    region: "north",
                    xtype: "panel",
                    items: [
                        {
                            xtype: "label",
                            text: "请设置字段对应关系：",
                            style: 'font-size:16px;font-weight:bolder;',
                            height: 35,
                        }
                    ],
                    height: 30
                }, {
                    region: "center",
                    xtype: "gridpanel",
                    border: true,
                    store: { xtype: 'array' },
                    selModel: {
                        type: 'cellmodel'
                    },
                    plugins: {
                        ptype: 'cellediting',
                        clicksToEdit: 1
                    },
                    columns: [
                        { dataIndex: "excelFieldCode", text: "Excel 字段", width: 300, menuDisabled: true, sortable: false, },
                        {
                            dataIndex: "systemFieldLabel", text: "系统字段", width: 300, menuDisabled: true, sortable: false,
                            editor: {
                                xtype: "combo",
                                typeAhead: true,
                                triggerAction: "all",                                  
                                store:[],                         
                                listeners: {
                                    change: function (self, newValue, oldValue, eOpts) {     
                                        debugger;                                  
                                        var record = self.getSelectedRecord();
                                        if (record == null) {
                                            return;
                                        }

                                        var gridRecord = self.up('gridpanel').getSelection()[0];
                                        gridRecord.set("systemFieldCode", record.get("field3"));
                                    }
                                }
                            }
                        },
                    ],
                },
            ]
        }
    ]
});