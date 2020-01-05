using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Imms.Data;
using Imms.Mes.Data.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Imms.Mes.Data
{
    public class WorkorderLogic : SimpleCRUDLogic<Workorder>
    {
        private IApplicationBuilder _App;
        public WorkorderLogic(IApplicationBuilder app)
        {
            this._App = app;
        }

        protected override void AfterUpdate(Workorder item, DbContext dbContext)
        {
            if (item.OrderStatus > 0)
            {
                Imms.Mes.Services.Kanban.Line.DataService LineDataService = _App.ApplicationServices.GetService<Imms.Mes.Services.Kanban.Line.DataService>();
                Task.Run(() => LineDataService.RefreshActiveWorkorders());

                Imms.Mes.Services.Kanban.Workshop.WorkshopDataService workshopDataService = _App.ApplicationServices.GetService<Imms.Mes.Services.Kanban.Workshop.WorkshopDataService>();
                Task.Run(() => workshopDataService.RefreshWorkorder());


                Imms.Mes.Services.Kanban.Factory.FactoryDataService factoryDataService = _App.ApplicationServices.GetService<Imms.Mes.Services.Kanban.Factory.FactoryDataService>();
                Task.Run(() => factoryDataService.RefreshWorkorder());
            }
        }

        public void StartWorkder(Workorder workorder)
        {
            if (workorder.OrderStatus > 0)
            {
                return;
            }
            try
            {
                this.DoStart(workorder);
                GlobalConstants.DefaultLogger.Info("已启动工单:" + workorder.OrderNo);
            }
            catch (Exception e)
            {
                GlobalConstants.DefaultLogger.Error("启动工单出现错误:" + e.Message);
                GlobalConstants.DefaultLogger.Debug(e.StackTrace);
            }
            this.RefreshKanban();
        }

        public void RefreshKanban()
        {
            Imms.Mes.Services.Kanban.Line.DataService dataService = _App.ApplicationServices.GetService<Imms.Mes.Services.Kanban.Line.DataService>();
            Task.Run(() =>
            {
                dataService.RefreshOrgAndSpanData();
                dataService.RefreshActiveWorkorders();
            });

            Imms.Mes.Services.Kanban.Workshop.WorkshopDataService workshopDataService = _App.ApplicationServices.GetService<Imms.Mes.Services.Kanban.Workshop.WorkshopDataService>();
            Task.Run(() =>
            {
                workshopDataService.RefreshOrgData();
                workshopDataService.RefreshWorkorder();
            });

            Imms.Mes.Services.Kanban.Factory.FactoryDataService factoryDataService = _App.ApplicationServices.GetService<Imms.Mes.Services.Kanban.Factory.FactoryDataService>();
            Task.Run(() =>
            {
                factoryDataService.RefreshOrgData();
                factoryDataService.RefreshWorkorder();
            });
        }

        private void DoStart(Workorder workorder)
        {
            workorder.OrderStatus = Workorder.WOKORDER_STATUS_STARTED;
            workorder.TimeStartActual = DateTime.Now;

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
            {
                using (DbContext dbContext = GlobalConstants.DbContextFactory.GetContext())
                {
                    ActiveWorkorder active = dbContext.Set<ActiveWorkorder>().Where(x => x.LineNo == workorder.LineNo).FirstOrDefault();
                    bool isUpdate = true;
                    if (active == null)
                    {
                        GlobalConstants.DefaultLogger.Info("产线:" + workorder.LineNo + "的activeOrder不存在，需要新增.");
                        active = new ActiveWorkorder();
                        active.LineNo = workorder.LineNo;

                        dbContext.Add(active);
                        isUpdate = false;
                    }
                    else
                    {
                        GlobalConstants.DefaultLogger.Info("产线:" + workorder.LineNo + "的原activeOrder已存在，需要修改.");
                        Workorder oldOrder = dbContext.Set<Workorder>().Where(x => x.OrderNo == active.WorkorderNo).First();
                        if (oldOrder.OrderStatus != Workorder.WORKORDER_SATUS_CLOSED && oldOrder.OrderNo != workorder.OrderNo)
                        {
                            oldOrder.OrderStatus = Workorder.WORKORDER_SATUS_CLOSED;
                            oldOrder.TimeEndActual = DateTime.Now;

                            GlobalConstants.ModifyEntityStatus<Workorder>(oldOrder, dbContext);
                        }

                    }
                    active.WorkorderNo = workorder.OrderNo;
                    active.LastUpdateTime = DateTime.Now;
                    active.PartNo = workorder.PartNo;
                    active.GID = 0;
                    active.DID = 0;
                    active.UpdateStatus = 0;

                    if (isUpdate)
                    {
                        GlobalConstants.ModifyEntityStatus<ActiveWorkorder>(active, dbContext);
                    }
                    GlobalConstants.ModifyEntityStatus<Workorder>(workorder, dbContext);

                    dbContext.SaveChanges();
                    scope.Complete();
                }
            }
        }
    }
}