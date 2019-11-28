using System.Collections.Generic;
using System.Linq;
using Imms.Data;
using Imms.Data.Domain;
using Imms.Mes.Data;
using Imms.Mes.Data.Domain;
using Imms.WebManager.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

    [Route("api/imms/org/workstation")]
    public class WorkstationController : SimpleCRUDController<Workstation>
    {
        public WorkstationController()
        {
            this.Logic = new SimpleCRUDLogic<Workstation>();
        }

        [Route("getStationByWorkshop")]
        public ExtJsResult GetStationByWorkshop(long workshopId)
        {
            IQueryCollection query = this.HttpContext.Request.Query;

            int page = int.Parse(query["page"][0]);
            int start = int.Parse(query["start"][0]);
            int limit = int.Parse(query["limit"][0]);
            FilterExpression expression = new FilterExpression()
            {
                L = "parentId",
                O = "=",
                R = workshopId.ToString()
            };

            ExtJsResult result = Logic.GetAllWithExtResult(page, start, limit, new FilterExpression[] { expression });
            return result;
        }
    }

    [Route("api/imms/org/operator")]
    public class OperatorController : SimpleCRUDController<Operator>
    {
        public OperatorController() => this.Logic = new SimpleCRUDLogic<Operator>();        
    }
}