Ext.define("app.model.imms.mfc.ProductSummaryModel", {
    extend: "Ext.data.Model",
    requires: ["app.ux.ZhxhDate"],
    fields: [
        { name: "productDate", type: "zhxhDate", dateFormat: 'Y-m-d' },
        { name: "productionId", type: "int" },
        { name: "productionCode", type: "string" },
        { name: "productionName", type: "string" },
        { name: "workshopId", type: "int" },
        { name: "workshopCode", type: "string" },
        { name: "workshopName", type: "string" },

        { name: "qtyGood_0", type: "int" },
        { name: "qtyDefect_0", type: "int" },
        {
            name: "qtyTotal_0", type: "int", calculate: function (data) {
                return data.qtyGood_0 + data.qtyDefect_0;
            }
        },
        {
            name: "rateGood_0", type: "number", calculate: function (data) {
                var good = data.qtyGood_0;
                if (good == 0) {
                    return 0;
                }

                return good / (data.qtyGood_0 + data.qtyDefect_0);
            }
        },
        {
            name: "rateDefect_0", type: "number", calculate: function (data) {
                var defect = data.qtyDefect_0;
                if (defect == 0) {
                    return 0;
                }

                return defect / (data.qtyGood_0 + data.qtyDefect_0);
            }
        },


        { name: "qtyGood_1", type: "int" },
        { name: "qtyDefect_1", type: "int" },
        {
            name: "qtyTotal_1", type: "int", calculate: function (data) {
                return data.qtyGood_1 + data.qtyDefect_1;
            }
        },
        {
            name: "rateGood_1", type: "number", calculate: function (data) {
                var good = data.qtyGood_1;
                if (good == 0) {
                    return 0;
                }

                return good / (data.qtyGood_1 + data.qtyDefect_1);
            }
        }, {
            name: "rateDefect_1", type: "number", calculate: function (data) {
                var defect = data.qtyDefect_1;
                if (defect == 0) {
                    return 0;
                }
                return defect / (data.qtyGood_1 + data.qtyDefect_1);
            }
        },

        {
            name: "qtyTotal", type: "int", calculate: function (data) {
                return data.qtyGood_1 + data.qtyDefect_1 + data.qtyGood_0 + data.qtyDefect_0;
            }
        },

        {
            name: "qtyGood", type: "int", calculate: function (data) {
                return data.qtyGood_1 + data.qtyGood_0;
            }
        },

        {
            name: "qtyDefect", type: "int", calculate: function (data) {
                return data.qtyDefect_1 + data.qtyDefect_0;
            }
        },
        {
            name: "rateDefect", type: "number", calculate: function (data) {
                var defect = data.qtyDefect_1 + data.qtyDefect_0;
                if (defect == 0) {
                    return 0;
                }

                return defect / (data.qtyGood_1 + data.qtyDefect_1 + data.qtyGood_0 + data.qtyDefect_0);
            }
        },
        {
            name: "rateGood", type: "number", calculate: function (data) {
                var good = data.qtyGood_1 + data.qtyGood_0;
                if (good == 0) {
                    return 0;
                }

                return good / (data.qtyGood_1 + data.qtyDefect_1 + data.qtyGood_0 + data.qtyDefect_0);
            }
        },
    ]
});