Ext.define('app.view.admin.systemParameter.SystemParameter', {
    extend: 'app.ux.dbgrid.DbGrid',
    xtype: 'app_view_admin_systemParameter_SystemParameter',
    requires:["app.model.admin.SystemParameterModel","app.store.admin.SystemParameterStore"],
    uses:["app.view.admin.systemParameter.SystemParameterDetailForm"],
    hideInsert:true,
    hideDelete:true,
    columns:[
        { dataIndex:"parameterClassCode",text:"类别代码",width:100},
        { dataIndex:"parameterClassName",text:"参数类别",width:180},
        { dataIndex: "parameterCode", text: "参数代码", width: 200 },
        { dataIndex: "parameterName", text: "参数含义", width: 200 },
        { dataIndex: "parameterValue", text: "参数值",flex:1,minWidth:300},
    ],
    additionToolbarItems: [
        '-',
        {
            text: '立即同步', privilege: "SYNC_WITH_ERP_WDB", handler: function () {
                app.ux.Utils.ajaxRequest({
                    url: "api/admin/systemParameter/sync_wdb",
                    method: "GET",
                    successCallback: function (result, response, opts) {
                        debugger;
                        alert(result);
                    }
                })
            }
        },
    ],

    constructor:function(config){
        var configBase = {
            store: Ext.create({ xtype: 'app_store_admin_SystemParameterStore' }),
            detailFormClass: 'admin_systemParameter_SystemParameterDetailForm',
            detailWindowTitle: '系统参数'
        }
        Ext.applyIf(config, configBase);

        this.callParent(arguments);
    }
        
});