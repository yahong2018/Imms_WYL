Ext.define('app.store.admin.SystemRoleStore', {
    extend: 'app.store.BaseStore',
    model: 'app.model.admin.SystemRoleModel',
    alias: 'widget.app_store_admin_SystemRoleStore',
    dao: {
        deleteUrl: 'security/systemRole/delete',
        insertUrl: 'security/systemRole/create',
        updateUrl: 'security/systemRole/update',
        selectUrl: 'security/systemRole/getAll',
    },

    loadRolePrivileges: function (role, callback) {
        var me = this;
        app.ux.Utils.ajaxRequest({
            url: 'api/security/systemRole/rolePrivileges?roleId=' + role.get('recordId'),
            successCallback: function (result, response, opts) {
                if (callback) {
                    callback.apply(me, [role, result]);
                }
            }
        })
    },
    updateRolePrivilege: function (role, privilegeList, callback) {
        var me = this;
       
        app.ux.Utils.ajaxRequest({
            url: 'api/security/systemRole/updatePrivileges?roleId=' + role.get('recordId'),
            method: 'POST',
            jsonData: privilegeList,
            successCallback: function (result, response, opts) {
                if (callback) {
                    callback.apply(me, [role, result]);
                }
            }
        })
    }
});