Ext.define("app.store.imms.mfc.WorkstationProductSummaryStore", {
    extend: "app.store.BaseStore",
    model: "app.model.imms.mfc.WorkstationProductSummaryModel",
    alias:"widget.app_store_imms_mfc_WorkstationProductSummaryStore",
    dao: {
        selectUrl: 'imms/mfc/workstationProductSummary/getAll',
    }
});