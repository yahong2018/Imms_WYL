using System;
using System.Linq;
using System.Transactions;
using Imms.Data;
using Imms.Mes.Data.Domain;
using Microsoft.EntityFrameworkCore;

namespace Imms.Mes.Data
{
    public class WorkorderLogic : SimpleCRUDLogic<Workorder>
    {
        public void StartWorkder(Workorder workorder)
        {
            if (workorder.OrderStatus > 0)
            {
                return;
            }
            workorder.OrderStatus = 1;
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
                    active.WorkorderNo = workorder.OrderNo;
                    active.LastUpdateTime = DateTime.Now;
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