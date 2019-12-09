Ext.define("app.ux.excel.ImporterController", {
    extend: "Ext.app.ViewController",
    alias: "controller.excelImporterController",

    cancelButtonClick: function () {
        this.getView().close();
    },

    prevButtonClick: function () {
        this.doCardNavigation(-1);
    },

    nextButtonClick: function () {
        var viewModel = this.getViewModel();
        var activeIndex = viewModel.get("navigator.activeIndex");
        if (activeIndex == 3) {
            this.doImport();
            return;
        }
        if (activeIndex == 0 && !this.verifyWorksheet()) {
            return;
        }
        if (activeIndex == 1 && !this.verifyFieldsSettings()) {
            return;
        }
        if (activeIndex == 2 && !this.verifyRowSettings()) {
            return;
        }

        this.doCardNavigation(1);
    },

    doCardNavigation: function (incr) {
        var viewModel = this.getViewModel();
        var window = this.getView();
        var cardForm = window.down("form");

        var activeIndex = viewModel.get("navigator.activeIndex") + incr;
        viewModel.set("navigator.activeIndex", activeIndex);
        cardForm.getLayout().setActiveItem(activeIndex);
        window.setTitle(viewModel.get("navigator.pages")[activeIndex]);

        window.down('[buttonId="btnPrev"]').setDisabled(activeIndex == 0);
        if (activeIndex == 3) {
            window.down('[buttonId="btnNext"]').setText("完成");
        } else {
            window.down('[buttonId="btnNext"]').setText("下一步");
        }
    },

    doImport: function () {
        this.getView().close();
    },

    uploadFile: function () {
        var view = this.getView();
        var viewModel = this.getViewModel();
        var form = view.down('app_ux_excel_cards_File').down('form');
        if (!form.down('fileuploadfield').value) {
            Ext.Msg.alert("系统提示","请先选择需要上传的文件!");
            return;
        }
        var targetTable = this.getViewModel().get("target.tableName");

        form.submit({
            url: "api/misc/excel/startImport?target=" + targetTable,
            success: function (form, action) {
                var message = Ext.decode(action.response.responseText);

                //    {
                //        sessionId:123456,
                //        worksheets:[
                //            "sheet1",
                //            "sheet2",
                //            "sheet3"
                //        ]
                //    }

                var radioGroup = view.down('app_ux_excel_cards_File').down('radiogroup');
                radioGroup.removeAll();
                viewModel.set('session', message.data);

                for (var i = 0; i < message.data.worksheets.length; i++) {
                    var item = Ext.create({ xtype: 'radiofield', boxLabel: message.data.worksheets[i], inputValue: i });
                    radioGroup.add(item);
                }
            }
        });
    },

    verifyWorksheet: function () {
        var view = this.getView();
        var viewModel = this.getViewModel();        
        var radioGroup = view.down('app_ux_excel_cards_File').down('radiogroup');    
        var value = radioGroup.getValue().worksheets;
        if (isNaN(value)){
            Ext.Msg.alert("系统提示", "请选择需要导入的Worksheet !");
            return false;
        }
        viewModel.set("session.activeSheet", value);

        return true;
    },

    getExcelFields:function(){
        debugger;
        
        var view = this.getView();
        var fieldPanel = view.down("app_ux_excel_cards_FieldSetting");
        
        var rowIndex = fieldPanel.down("[name='field_row_index']").value;
        var columnStartIndex = fieldPanel.down("[name='field_column_start_index']").value;
        var columnEndIndex = fieldPanel.down("[name='field_column_end_index']").value;

        if(isNaN(rowIndex) || isNaN(columnStartIndex) || isNaN(columnEndIndex)){
            Ext.Msg.alert("系统提示","请设置正确的[字段行]和[列范围]!");

            return;
        }

        var session = viewModel.get("session");
        session.fieldRowIndex = rowIndex;
        session.columnStartIndex = columnStartIndex;
        session.columnEndIndex = columnEndIndex;
        
        app.ux.Utils.ajaxRequest({
            url:"api/misc/excel/getExcelFields",
            method:"POST",
            jsonData:session,
            successCallback: function (result, response, opts){
                debugger;
                viewModel.set('session', result);
            }
        });
    },

    verifyFieldsSettings: function () {
        return true;
    },

    verifyRowSettings: function () {
        return true;
    }
});