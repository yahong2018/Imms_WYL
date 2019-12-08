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
        var targetTable = this.getViewModel().get("target.tableName");
        if (!form.isValid()) {
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

                    var radioGroup = view.down('app_ux_excel_cards_File').down('worksheets');
                    radioGroup.removeAll();
                    for (var i = 0; i < message.worksheets.length; i++) {
                        var item = Ext.create({ xtype: 'radiofield', boxLabel: message.worksheets[i], inputValue: i });
                        radioGroup.add(item);
                    }

                    viewModel.set('session.sessionId', message.sessionId);
                    viewModel.set('session.worksheets', message.worksheets);
                }
            });
        }
    },

    verifyWorksheet: function () {
        var radioGroup = this.getView().down('app_ux_excel_cards_File').down('worksheets');
        var value = radioGroup.getValue();
        this.getViewModel().set("session.activeSheet", value);

        return value;
    },

    verifyFieldsSettings: function () {
        return true;
    },

    verifyRowSettings: function () {
        return true;
    }
});