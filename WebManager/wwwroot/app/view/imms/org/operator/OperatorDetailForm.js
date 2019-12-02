Ext.define("app.view.imms.org.operator.OperatorDetailForm", {
    extend: "Ext.form.Panel",
    xtype: "imms_org_operator_OperatorDetailForm",
    width: 430,
    bodyPadding: 5,
    defaults: {
        labelWidth: 70
    },
    layout: "vbox",
    items: [
        {
            name: "recordId",
            xtype: "hidden"
        },
        {
            name: "pic",
            xtype: "hidden"
        },
        {
            xtype: "container",
            layout: "hbox",
            items: [
                {
                    xtype: "container",
                    items: [
                        {
                            name: "orgCode",
                            xtype: "textfield",
                            fieldLabel: "所属产线",
                            width: 250,
                            allowBlank: false
                        }, {
                            name: "empId",
                            xtype: "textfield",
                            fieldLabel: "工号",
                            allowBlank: false,
                            maxLength: 10,
                            enforceMaxLength: true,
                            width: 250
                        }, {
                            name: "empName",
                            xtype: "textfield",
                            fieldLabel: "姓名",
                            allowBlank: false,
                            maxLength: 20,
                            enforceMaxLength: true,
                            width: 250,
                        },
                        {
                            name: "title",
                            xtype: "textfield",
                            fieldLabel: "工作岗位",
                            allowBlank: false,
                            maxLength: 20,
                            enforceMaxLength: true,
                            width: 250,
                        },
                        {
                            name: "seq",
                            xtype: "textfield",
                            fieldLabel: "显示顺序",
                            allowBlank: false,
                            maxLength: 20,
                            enforceMaxLength: true,
                            width: 250,
                        }
                    ]
                },
                {
                    xtype: 'box',
                    width: 120,
                    height: 150,
                    margin: "5 10 5 10",
                    name: "disp_pic",
                    autoEl: {
                        tag: 'img',
                        src: 'upload/operators/W01/W01L01/1_C00001_张三_拉长.jpg'
                    }
                }
            ]
        }, {
            xtype: "filefield",
            fieldLabel: "照片文件",
            width: 400,
            labelWidth: 100
        }
    ],
    onRecordLoad: function (config) {
        if (config.seq == app.ux.data.DataOperationSeq.AFTER) {
            debugger;
            this.down("[name='disp_pic']").getEl().dom.src = config.record.get("pic");
        }
    }
});