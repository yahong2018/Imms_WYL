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
    },

    verifyWorksheet: function () {
        return true;
    },
    verifyFieldsSettings: function () {
        return true;
    },
    verifyRowSettings:function(){
        return true;
    }
});