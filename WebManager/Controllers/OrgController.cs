using System;
using System.Collections.Generic;
using System.Linq;
using Imms.Data;
using Imms.Data.Domain;
using Imms.Mes.Data;
using Imms.Mes.Data.Domain;
using Imms.WebManager.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Imms.WebManager.Controllers
{
    [Route("api/imms/org/workshop")]
    public class WorkshopController : SimpleCRUDController<Workshop>
    {
        public WorkshopController()
        {
            this.Logic = new SimpleCRUDLogic<Workshop>();
        }
    }

    [Route("api/imms/org/workline")]
    public class WorklineController : SimpleCRUDController<Workline>
    {
        public WorklineController()
        {
            this.Logic = new SimpleCRUDLogic<Workline>();
        }
    }

    [Route("api/imms/org/workstation")]
    public class WorkstationController : SimpleCRUDController<Workstation>
    {
        public WorkstationController()
        {
            this.Logic = new SimpleCRUDLogic<Workstation>();
        }
    }

    [Route("api/imms/org/operator")]
    public class OperatorController : SimpleCRUDController<Operator>
    {
        private readonly IHostingEnvironment _host = null;        
        public OperatorController(IHostingEnvironment host)
        {
            this._host = host;
            this.Logic = new OperatorLogic(host);
        }
    }
}