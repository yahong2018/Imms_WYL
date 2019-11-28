Ext.define("app.store.imms.material.BomStore", {
    extend: "app.store.BaseStore",
    model: 'app.model.imms.material.BomModel',
    alias: 'widget.app_store_imms_material_BomStore',

    dao: {
        deleteUrl: 'imms/material/bom/delete',
        insertUrl: 'imms/material/bom/create',
        updateUrl: 'imms/material/bom/update',
        selectUrl: 'imms/material/bom/getAll',
    },

    getBom: function (material) {
        if (material == null) {
            return;
        }

        var materialCode = material.get("materialCode");
        this.getProxy().url = "api/imms/material/bom/getBomByMaterialCode?materialCode=" + materialCode;

        this.load();
    }
});