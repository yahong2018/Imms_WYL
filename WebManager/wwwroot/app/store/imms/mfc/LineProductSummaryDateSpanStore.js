Ext.define("app.store.imms.mfc.LineProductSummaryDateSpanStore", {
    extend: "app.store.BaseStore",
    alias: "widget.app_store_imms_mfc_LineProductSummaryDateSpanStore",
    model: "app.model.imms.mfc.LineProductSummaryDateSpanModel",
    dao: {
        deleteUrl: 'imms/mfc/lineProductSummaryDateSpan/delete',
        insertUrl: 'imms/mfc/lineProductSummaryDateSpan/create',
        updateUrl: 'imms/mfc/lineProductSummaryDateSpan/update',
        selectUrl: 'imms/mfc/lineProductSummaryDateSpan/getAll',
    }
});