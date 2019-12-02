Ext.define("app.ux.excel.ImporterViewModel", {
    extend: "Ext.app.ViewModel",
    alias: "viewmodel.excelImportViewModel",
    data: {
        target: {
            tableName:"mes_workorder",
            template: "upload/excel/templates/mes_workorder.xlsx",
        },
        navigator: {
            activeIndex: 0,
            pages: [
                "文件选择",               
                "字段设置",
                "数据设置",
                "完成导入"
            ]
        }
    }
});