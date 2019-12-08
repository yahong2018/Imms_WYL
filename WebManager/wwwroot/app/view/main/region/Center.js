Ext.define('app.view.main.region.Center', {
	extend: 'Ext.tab.Panel',
	alias: 'widget.maincenter',
	requires: ["app.view.main.region.ActiveWorkorderGrid"],
	uses: ['app.ux.ButtonTransparent'],

	initComponent: function () {
		this.items = [{
			//	id:'homePage',
			glyph: 0xf015,
			title: '首页',
			border: true,
			frame: false,
			bodyCls: 'panel-background',
			reorderable: false,
			layout: "border",
			items: [
				{
					region: "north",
					height: 50,
					xtype: "panel",
					layout: "hbox",
					items: [
						{
							xtype: "label",
							text: "生产计划执行情况",
							flex: 1,
							style: "text-align:center;font-size:24px;font-weight:bolder;line-height:50px;vertical-align: middle;"
						}, {
							xtype: "button",
							text: "刷新",
							width: 80,
							margin: "5 5 5 5",
							handler: function () {
								var summaryGrid = this.up("maincenter").down("app_view_main_region_ActiveWorkorderGrid");
								summaryGrid.store.load();	
							}
						}
					]
				},
				{
					region: "center",
					xtype: "app_view_main_region_ActiveWorkorderGrid",					
				}
			]
		}];
		this.callParent();
	},
	listeners: {
	}
});

