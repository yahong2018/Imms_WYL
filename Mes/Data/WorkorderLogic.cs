using System;
using System.Linq;
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
            Imms.Mes.Services.Kanban.Line.DataService dataService = _App.ApplicationServices.GetService<Imms.Mes.Services.Kanban.Line.DataService>();
            dataService.RefreshActiveWorkorders();
        }

        public void StartWorkder(Workorder workorder)
        {
            if (workorder.OrderStatus > 0)
            {
                return;
            }
            this.DoStart(workorder);

            Imms.Mes.Services.Kanban.Line.DataService dataService = _App.ApplicationServices.GetService<Imms.Mes.Services.Kanban.Line.DataService>();
            dataService.RefreshActiveWorkorders();
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
                        active = new ActiveWorkorder();
                        active.LineNo = workorder.LineNo;

                        dbContext.Add(active);
                        isUpdate = false;
                    }
                    else
                    {
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