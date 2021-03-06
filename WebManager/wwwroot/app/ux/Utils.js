Ext.define('app.ux.Utils', {
    uses: ['Ext.window.MessageBox', 'Ext.Ajax'],
    singleton: true,

    ajaxRequest: function (config) {
        var me = config;

        var handleFailure = function (response, opts) {
            if (!me.silence) {
                var message = response.responseText.trim().replace("\n", "<br>");
                Ext.MessageBox.show({
                    title: '系统提示',
                    msg: message,
                    buttons: Ext.MessageBox.OK,
                    icon: Ext.MessageBox.ERROR,
                    fn: function () {
                        if (me.failureCallback) {
                            me.failureCallback(arguments);
                        }
                    }
                });
            }
        };

        var configBase = {
            success: function (response, opts) {
                try {
                    var result = Ext.decode(response.responseText);
                    if (typeof result == "string") {
                        result = Ext.decode(result);
                    }
                    if (me.successCallback) {
                        me.successCallback(result, response, opts);
                    }
                } catch (e) {
                    if(me.failCallback){
                        me.failCallback(response,opts);
                    }else{
                        handleFailure(response, opts);
                    }                    
                }
            }, failure: function (response, opts) {
                if (me.failCallback) {
                    me.failCallback(response, opts);
                } else {
                    handleFailure(response, opts);
                }
            }
        }

        Ext.applyIf(config, configBase);

        Ext.Ajax.request(config);
    },

    verifySelection: function (grid, callback) {
        var record = grid.getSelectionModel().getSelection();
        if (!record || record.length == 0) {
            Ext.MessageBox.alert("系统提示", "请先选择一条待编辑记录！");
            return;
        }
        record = record[0];
        if (callback) {
            callback(record);
        }
    },
    applyPrivileges: function (config) {
        var privilegeList = app.ux.GlobalVars.currentLogin.privielges;
        var programId = config.programId;
        var model = config.model;
        var comp = config.component;
       // debugger;

        for (var i = 0; i < privilegeList.length; i++) {
            if (privilegeList[i].programId != programId) {
                continue;
            }
            var privilege = privilegeList[i];
            var actionButtonList = comp.query('[privilege="' + privilege.privilegeCode + '"]');
            if (actionButtonList) {
                for (var j = 0; j < actionButtonList.length; j++) {
                    var actionButton = actionButtonList[j];
                    actionButton.setDisabled(false);
                }
            }
        }
    },

    hasPrivilege: function (config) {
        var privilegeList = app.ux.GlobalVars.currentLogin.privielges;
        var privilegeCode = config.privilegeCode;
        var programId = config.programId;

        for (var i = 0; i < privilegeList.length; i++) {
            if (privilegeList[i].programId == programId && privilegeList[i].privilegeCode == privilegeCode) {
                return true;
            }
        }

        return false;
    },
});