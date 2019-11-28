Ext.define('app.view.main.region.Center', {
	extend: 'Ext.tab.Panel',
	alias: 'widget.maincenter',
	requires: ["app.view.imms.rpt.rptProductionOrderProgress.RptProductionOrderProgressGrid"],
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
					xtype:"panel",
					layout:"hbox",
					items:[
						{
							xtype: "label",
							text: "本日生产状态一览表",
							flex:1,
							style: "text-align:center;font-size:24px;font-weight:bolder;line-height:50px;vertical-align: middle;"
						},{
							xtype:"button",
							text:"刷新",
							width:80,
							margin:"5 5 5 5",
							handler:function(){
								var summaryGrid = this.up("maincenter").down("app_view_imms_rpt_rptProductionOrderProgress_RptProductionOrderProgressGrid");
								summaryGrid.store.load();								
							}
						}
					]
				},
				{
					region: "center",
					xtype: "app_view_imms_rpt_rptProductionOrderProgress_RptProductionOrderProgressGrid",
					height: "100%",
					filter:function(){
						return {
							L: "productDate",
							O: "=",
							R: Ext.util.Format.date(new Date(), 'Y/m/d')
						};
					},
					hideDefeaultPagebar:true,
					hideDefaultToolbar:true,
				}
			]
		}];
		this.callParent();
	},
	listeners: {
	}
});

