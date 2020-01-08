Ext.define('app.view.main.Main', {
	extend: 'Ext.container.Container',
	xtype: 'app-main',
	requires: [
		'app.view.main.MainController', 'app.view.main.MainModel',
	],

	uses: ['app.view.main.region.Center', 'app.view.main.region.Top',
		'app.view.main.region.Bottom', 'app.view.main.menu.MainMenuTree',		
	],

	controller: {
		//	id: 'main-controller',
		type: 'main'
	},
	viewModel: {
		type: 'main'
	},
	layout: "border",

	initComponent: function () {
		Ext.setGlyphFontFamily('FontAwesome');
		
		this.callParent(arguments);
	},

	startTask: function () {
		var mainView = this;
		mainView.getViewModel().set('system.name', "威雅利实业有限公司智能制造平台");

		Ext.TaskManager.start({
			interval: 1000,
			run: function () {
				var the_div = Ext.query('div[system-time-div]')[0];
				var msg = '北京时间 ' + Ext.util.Format.date(new Date(), "Y-m-d H:i:s");
				Ext.getDom(the_div).innerText = msg;
			}
		});

		var runner = mainView.getViewModel().get('system.taskRunner');
		var interval = mainView.getViewModel().get('system.autoRefreshInterval') * 1000 * 60 * 1;
		var task = runner.newTask({
			interval: interval,
			run: function () {
				console.log('start keep alive:' + Ext.util.Format.date(new Date(), "Y-m-d H:i:s"));

				app.ux.Utils.ajaxRequest({
					url:"home/keepAlive",
					method:"GET",
					successCallback:function(){
						console.log('sucess keep alive:' + Ext.util.Format.date(new Date(), "Y-m-d H:i:s"));
					},
					failCallback: function (response, opts){
						console.log('failed keep alive:' + Ext.util.Format.date(new Date(), "Y-m-d H:i:s"));
						console.log(response.responseText);

						task.stop();
						
						Ext.Msg.alert("系统提示","已与后台断开连接，请刷新页面重新登录，如果不能登录系统请联系管理员!");	
					}
				});
			}
		});
		mainView.getViewModel().set('system.autoRefreshTask', task);
		task.start();
	},

	listeners: {
		resize: function (container) {
			container.getController().onMainResize();
		},
		afterrender: function () {
			this.startTask();

			// var summaryGrid = this.down("maincenter");
			// summaryGrid = summaryGrid.down("gridpanel");
			// summaryGrid.summaryStore = Ext.create({ xtype: "imms_mfc_ProductSummaryStore", autoLoad: false });
			// summaryGrid.store.loadTodayReportData(summaryGrid.summaryStore);
		}
	},

	items: [{
		xtype: 'maintop',
		region: 'north'
	}, {
		xtype: 'mainbottom',
		region: 'south'
	}, {
		xtype: 'mainmenutree',
		region: 'west',
		title: '导航菜单',
		width: 180,
		collapsible: true,
		split: true
	}, {
		region: 'center',
		xtype: 'maincenter',
	}]
});