using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using Imms.Core;
using Imms.Data;
using Imms.Mes.Data;
using Imms.Mes.Data.Domain;
using Imms.WebManager.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Imms.WebManager.Controllers
{
    [Route("api/imms/mfc/workorder")]
    public class WorkorderController : SimpleCRUDController<Workorder>
    {
        public WorkorderController(IHostingEnvironment host)
        {
            this.Logic = new WorkorderLogic(Startup.AppBuilder, host);
        }

        [Route("start"), HttpPost]
        public BusinessException Start([FromBody]Workorder item)
        {
            (this.Logic as WorkorderLogic).StartWorkder(item);
            return new BusinessException(GlobalConstants.EXCEPTION_CODE_NO_ERROR);
        }

        [Route("complete"), HttpPost]
        public BusinessException Complete([FromBody]Workorder item)
        {
            (this.Logic as WorkorderLogic).CompleteWorkder(item);
            return new BusinessException(GlobalConstants.EXCEPTION_CODE_NO_ERROR);
        }

        [Route("import"), HttpPost]
        public string Import(int rowStart,int rowEnd,int errorHandle)
        {
            if (HttpContext.Request.Form.Files.Count == 0)
            {
                return "No file exists";
            }
            var file = HttpContext.Request.Form.Files[0];            

            int count = (this.Logic as WorkorderLogic).ImportExcel(rowStart,rowEnd,errorHandle,file);

            return "已成功导入"+count+"条记录!";
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

    [Route("api/imms/mfc/workstationProductSummary")]
    public class WorkstationProductSummaryController : SimpleCRUDController<WorkstationProductSummary>
    {
        public WorkstationProductSummaryController()
        {
            this.Logic = new SimpleCRUDLogic<WorkstationProductSummary>();
        }
    }

    [Route("api/imms/mfc/lineProductSummaryDateSpan")]
    public class LineProductSummaryDateSpanController : SimpleCRUDController<LineProductSummaryDateSpan>
    {
        public LineProductSummaryDateSpanController()
        {
            this.Logic = new SimpleCRUDLogic<LineProductSummaryDateSpan>();
        }

        protected override QueryParameter GetQueryParameters()
        {
            QueryParameter parameter = base.GetQueryParameters();
            parameter.SortExpr = "ProductDate desc,WorkorderNo asc,SpanId asc";

            return parameter;
        }

        protected override void AfterGetAll(ExtJsResult result)
        {
            List<LineProductSummaryDateSpan> list = result.RootProperty as List<LineProductSummaryDateSpan>;
            string[] workorderNos = list.Select(x => x.WorkorderNo).Distinct().ToArray();

            List<LineProductSummaryDateSpanViewModel> viewModelList = new List<LineProductSummaryDateSpanViewModel>();
            foreach (LineProductSummaryDateSpan summary in list)
            {
                LineProductSummaryDateSpanViewModel viewModel = new LineProductSummaryDateSpanViewModel();
                viewModel.RecordId = summary.RecordId;
                viewModel.Clone(summary);
                viewModelList.Add(viewModel);
            }

            CommonRepository.UseDbContext(dbContext =>
            {
                List<WorkshiftSpan> spans = dbContext.Set<WorkshiftSpan>().ToList();
                List<Workorder> workorders = dbContext.Set<Workorder>().Where(x => workorderNos.Contains(x.OrderNo)).ToList();
                foreach (LineProductSummaryDateSpanViewModel viewModel in viewModelList)
                {
                    viewModel.QtyTotal = viewModel.QtyBad + viewModel.QtyGood;
                    Workorder order = workorders.Where(x => x.OrderNo == viewModel.WorkorderNo).FirstOrDefault();
                    if (order != null)
                    {
                        viewModel.UPH = order.UPH;
                    }
                    viewModel.QtyTotal = viewModel.QtyBad + viewModel.QtyBad;
                    if (viewModel.UPH > 0)
                    {
                        viewModel.OTD = ((float)viewModel.QtyGood / (float)viewModel.UPH) * 100;
                    }
                    if (viewModel.QtyTotal > 0)
                    {
                        viewModel.Fail = ((float)viewModel.QtyBad / (float)viewModel.QtyTotal) * 100;
                    }
                    WorkshiftSpan span = spans.Where(x => x.RecordId == viewModel.SpanId).FirstOrDefault();
                    if (span != null)
                    {
                        viewModel.SpanName = span.TimeBegin + "~" + span.TimeEnd;
                    }
                }
            });
            result.RootProperty = viewModelList;
        }
    }
}