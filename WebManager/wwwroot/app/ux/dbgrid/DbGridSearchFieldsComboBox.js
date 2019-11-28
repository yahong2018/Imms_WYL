Ext.define('app.ux.dbgrid.DbGridSearchFieldsComboBox', {
    extend: 'Ext.form.ComboBox',
    xtype: 'dbgrid_DbGridSearchFieldsComboBox',
    constructor: function (config) {

        var searchColumns=[];
        for(var i=0;i<config.dbGrid.columns.length;i++){
            if(!config.dbGrid.columns[i].disableSearch){
                searchColumns.push(config.dbGrid.columns[i])
            }
        }

        var store = Ext.create('Ext.data.Store', {
            data: searchColumns
        });

        var configBase = {
            store: store,
            queryMode: 'local',
            valueField: 'dataIndex',
            displayField: 'text'
        }

        Ext.applyIf(config,configBase);

        this.callParent(arguments);
    }
});