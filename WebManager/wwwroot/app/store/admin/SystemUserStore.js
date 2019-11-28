Ext.define('app.store.admin.SystemUserStore', {
    extend: 'app.store.BaseStore',
    model: 'app.model.admin.SystemUserModel',
    alias: 'widget.app_store_admin_SystemUserStore',
    uses: ['app.model.admin.UserRoleModel', 'app.ux.Utils','Ext.window.Toast'],
    dao: {
        deleteUrl: 'security/systemUser/delete',
        insertUrl: 'security/systemUser/create',
        updateUrl: 'security/systemUser/update',
        selectUrl: 'security/systemUser/getAll',
    },

    restePassword: function (user) {
        var me = this;
        app.ux.Utils.ajaxRequest({
            url: 'api/security/systemUser/resetPassword?userId=' + user.get('recordId'),
            successCallback: function (record, response, opts) {
                Ext.Msg.alert('系统提示', '密码已重设为系统缺省密码!');
            }
        });
    },
    startUser: function (user) {
        var me = this;
        app.ux.Utils.ajaxRequest({
            url: 'api/security/systemUser/enable?userId=' + user.get('recordId'),
            successCallback: function (record, response, opts) {
                user.beginEdit();
                user.set('userStatus', 0);
                user.endEdit();
                me.commitChanges();

                Ext.toast({
                    html: '用户已启用',
                    title: '系统提示',
                    width: 200,
                    align: 't'
                });               
            }
        });
    },
    stopUser: function (user) {
        var me = this;
        app.ux.Utils.ajaxRequest({
            url: 'api/security/systemUser/disable?userId=' + user.get('recordId'),
            successCallback: function (record, response, opts) {
                user.beginEdit();
                user.set('userStatus', 1);
                user.endEdit();
                me.commitChanges();

                Ext.toast({
                    html: '用户已停用',
                    title: '系统提示',
                    width: 200,
                    align: 't'
                });    
            }
        });
    },
    beforeUpdate: function (current, old) {
        current.password = old.get('password');
    },
    getUserRoles: function (user) {
        if (user.userRoles != null) {
            return;
        }
        
        app.ux.Utils.ajaxRequest({
            url: 'api/security/systemUser/userRoles?userId=' + user.get('recordId'),
            successCallback: function (result, response, opts) {
                user.userRoles = [];
                for (var i = 0; i < result.length; i++) {
                    user.userRoles.push(Ext.create('app.model.admin.SystemRoleModel', result[i]));
                }
            }
        });
    },
    updateUserRoles: function (win, user, roles) {
        var userRoles = [];
        var userId = user.get('recordId');
        for (var i = 0; i < roles.length; i++) {
            userRoles.push({
                userId: user.get('recordId'),
                roleId: roles[i].get('recordId')
            });
        }

        app.ux.Utils.ajaxRequest({
            url: 'api/security/systemUser/updateUserRoles?userId=' + userId,
            method: 'POST',
            jsonData: userRoles,
            successCallback: function (result, response, opts) {
                user.userRoles = roles;
                win.close();
                win.destroy();

                Ext.toast({
                    html: '数据已保存',
                    title: '系统提示',
                    width: 200,
                    align: 't'
                });
            }
        });
    }
});