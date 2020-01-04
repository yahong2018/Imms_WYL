Ext.define("app.ux.excel.ImporterViewModel", {
    extend: "Ext.app.ViewModel",
    alias: "viewmodel.excelImportViewModel",
    data: {
        target: {
            tableName: "mes_workorder",
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
        },
        session: {
            sessionId: "",
            activeSheet: -1,
            worksheets: ["sheet1", "sheet2", "sheet3"],
            fieldRowIndex: -1,
            columnStartIndex:-1,
            columnEndIndex:-1,
            fieldMappings: [
                {
                    systemFieldCode: "orderNo",
                    systemFieldLabel:"订单编号",
                    excelFieldCode: "JobNum",
                    columnIndex: 0
                },
                {
                    systemFieldCode: "production_no",
                    systemFieldLabel:"产品编码",
                    excelFieldCode: "PartNo",
                    columnIndex: 1
                },
            ]
        }
    }
});