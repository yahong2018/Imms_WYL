using Imms.Mes.Data.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Imms.WebManager.Controllers
{
    [Route("api/imms/timesheet/workshift")]
    public class WorkShiftController : SimpleCRUDController<Workshift>
    {
        public WorkShiftController()
        {
            this.Logic = new Data.SimpleCRUDLogic<Workshift>();
        }
    }

    [Route("api/imms/timesheet/workshiftSpan")]
    public class WorkshiftSpanController : SimpleCRUDController<WorkshiftSpan>
    {
        public WorkshiftSpanController()
        {
            this.Logic = new Data.SimpleCRUDLogic<WorkshiftSpan>();
        }


    }
}