Ext.define("app.ux.excel.cards.File", {
    extend: "Ext.panel.Panel",
    alias: 'widget.app_ux_excel_cards_File',
    layout: 'anchor',
    defaults: {
        anchor: '100%',
        style: 'font-size:18px',
        fieldStyle: 'font-size:18px', 
    },
    margin: 5,
    items: [
        {
            xtype: "label",            
            bind: {
                html: "<p >请选择需要导入的Excel文件。<br/><br/>所上传文件必须符合格式，<a href='{target.template}' target='_blank'>单击下载模板</a>，根据模板填写以减少错误。</p>"
            }
        },
        {
            xtype: "container",
            layout: "hbox",
            items: [
                {
                    xtype: 'fileuploadfield',
                    hideLabel: true,
                    flex: 0.7
                }, {
                    xtype: "button",
                    text: "上传",
                    width: 100,
                    margin: '0 0 0 10',
                    handler: 'uploadFile'
                }
            ]
        }, {
            xtype: "label",            
            html: "<p>请选择需要导入的表格:</p>"
        }, {
            xtype: 'radiogroup',
            columns: 1,
            name: 'worksheets',
            items: [
                { boxLabel: 'Sheet1', inputValue: 1, checked: true },
                { boxLabel: 'Sheet2', inputValue: 2 },
                { boxLabel: 'Sheet3', inputValue: 3 }
            ]
        }
    ]
});