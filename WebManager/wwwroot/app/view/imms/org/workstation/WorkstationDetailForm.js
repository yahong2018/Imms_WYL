Ext.define("app.view.imms.org.workstation.WorkstationDetailForm", {
    extend: "app.ux.TrackableFormPanel",
    xtype: "imms_org_workstation_WorkstationDetailForm",

    width: 400,
    bodyPadding: 5,
    defaults: {
        labelWidth: 100
    },
    items: [
        {
            name: "parentId",
            xtype: "hidden"
        },{
            name: "parentCode",
            xtype: "hidden"
        },
        {
            name: "parentName",
            xtype: "hidden"
        },
         {
            name: "orgCode",
            xtype: "textfield",
            fieldLabel: "工位代码",
            allowBlank: false,
            maxLength: 20,
            enforceMaxLength: true,
            width: 250
        }, {
            name: "orgName",
            xtype: "textfield",
            fieldLabel: "工位名称",
            allowBlank: false,
            maxLength: 50,
            enforceMaxLength: true,
            width: 380
        }, {
            name: "wocgCode",
            xtype: "textfield",
            fieldLabel: "工作中心组",
            allowBlank: false,
            maxLength: 50,
            enforceMaxLength: true,
            width: 380
        },{
            name: "rfidControllerId",
            xtype: "textfield",
            fieldLabel: "Rfid控制器编号",
            allowBlank: false,
            maxLength: 3,
            enforceMaxLength: true,
            width: 180
        }, {
            name: "rfidTerminatorId",
            xtype: "textfield",
            fieldLabel: "Rfid工位机编号",
            allowBlank: false,
            maxLength: 3,
            enforceMaxLength: true,
            width: 180
        }, {
             name: "rfidTemplateIndex",
            xtype: "textfield",
            fieldLabel: "显示模板编号",
            allowBlank: false,
            maxLength: 3,
            enforceMaxLength: true,
            width: 180
        }, {
             name: "autoReportCount",
            xtype: "textfield",
            fieldLabel: "自动工序数",
            allowBlank: false,
            maxLength: 3,
            enforceMaxLength: true,
            width: 180
        },      
        {
            name: "description",
            xtype: "textarea",
            fieldLabel: "备注",
            width: 380
        }
    ],
    onRecordLoad:function(config){
        if (config.dataMode == app.ux.data.DataMode.INSERT && config.seq == app.ux.data.DataOperationSeq.BEFORE){
            var record = config.record;
            var grid = config.grid;
            
            record.set("parentId", grid.store.workshop.get("recordId"));
            record.set("parentCode", grid.store.workshop.get("orgCode"));
            record.set("parentName", grid.store.workshop.get("orgName"));
        }
    }
});