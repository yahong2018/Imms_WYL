using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using Imms.Core;
using Imms.Data;
using Imms.Mes.Data;
using Imms.Mes.Data.Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Imms.WebManager.Controllers
{
    [Route("api/imms/mfc/workorder")]
    public class WorkorderController : SimpleCRUDController<Workorder>
    {
        public WorkorderController()
        {
            this.Logic = new WorkorderLogic();
        }

        [Route("start"), HttpPost]
        public BusinessException Start([FromBody]Workorder item)
        {
            (this.Logic as WorkorderLogic).StartWorkder(item);
            return new BusinessException(GlobalConstants.EXCEPTION_CODE_NO_ERROR);
        }
    }

    [Route("api/imms/mfc/workorderActual")]
    public class WorkorderActualController : SimpleCRUDController<WorkorderActual>
    {
        public WorkorderActualController()
        {
            this.Logic = new SimpleCRUDLogic<WorkorderActual>();
        }
    }
}