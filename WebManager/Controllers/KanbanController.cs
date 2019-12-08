using System;
using System.Linq;
using System.Threading;
using Imms.Data;
using Imms.Mes.Data.Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Imms.WebManager.Controllers
{
    [Route("kanban")]
    [TypeFilter(typeof(Microsoft.AspNetCore.Mvc.Authorization.AllowAnonymousFilter))]
    public class KanbanController : Controller
    {
        private readonly IHostingEnvironment env;

        public KanbanController(IHostingEnvironment hostingEnvironment)
        {
            env = hostingEnvironment;
        }

        [Route("line")]
        public IActionResult Index()
        {
            string lineNo = this.Request.Query["lineNo"];
            Operator[] operators = CommonRepository.GetAllByFilter<Operator>(x => x.orgCode == lineNo && x.Seq != -1).OrderBy(x => x.Seq).Take(4).ToArray();

            OperatorItem[] OperatorList = new OperatorItem[]{
                new OperatorItem(){DisplayName="拉长 张三",PicUrl=""},
                new OperatorItem(){DisplayName="检测 李四",PicUrl=""},
                new OperatorItem(){DisplayName="外观 王五",PicUrl=""},
                new OperatorItem(){DisplayName="包装 赵大",PicUrl=""},
            };

            for (int i = 0; i < operators.Length && i < 4; i++)
            {
                OperatorItem item = OperatorList[i];
                Operator op = operators[i];
                item.DisplayName = op.Title + " " + op.EmpName;
                item.PicUrl = op.Pic;
            }

            ViewBag.OperatorList = OperatorList;

            return View("Line");
        }

        // [Route("line")]
        // public IActionResult Index()
        // {
        //     string lineNo = this.Request.Query["lineNo"];
        //     Workline line = CommonRepository.GetOneByFilter<Workline>(x => x.LineCode == lineNo);
        //     Workshop workshop = CommonRepository.GetOneByFilter<Workshop>(x => x.RecordId == line.ParentId);
        //     string relatePath = $"upload/operators/{workshop.OrgCode}/{line.OrgCode}";
        //     string rootPath = $"{env.WebRootPath}/{relatePath}".Replace("\\","/");
        //     string[] files = System.IO.Directory.GetFiles(rootPath).OrderBy(x => x).ToArray();
        //     OperatorItem[] OperatorList = new OperatorItem[]{
        //         new OperatorItem(){DisplayName="拉长 张三",PicUrl="upload/operators/W01/W01L01/C00001_张三_拉长.jpg"},
        //         new OperatorItem(){DisplayName="检测 李四",PicUrl="upload/operators/W01/W01L01/C00002_李四_测试.jpg"},
        //         new OperatorItem(){DisplayName="外观 王五",PicUrl="upload/operators/W01/W01L01/C00003_王五_外观.jpg"},
        //         new OperatorItem(){DisplayName="包装 赵大",PicUrl="upload/operators/W01/W01L01/C00004_赵大_包装.png"},
        //     };
        //     for (int i = 0; i < files.Length && i < 4; i++)
        //     {
        //         OperatorItem item = OperatorList[i];
        //         string fullPath = files[i];
        //         string fileName = System.IO.Path.GetFileNameWithoutExtension(fullPath);
        //         string[] fields = fileName.Split("_", StringSplitOptions.RemoveEmptyEntries);

        //         item.DisplayName = fields[3] + " " + fields[2];
        //         item.PicUrl = relatePath + "/" + System.IO.Path.GetFileName(fullPath);
        //     }

        //     ViewBag.OperatorList = OperatorList;

        //     return View("Line");
        // }

        [Route("line_wrapper")]
        public IActionResult LineWrapper()
        {
            return View("Line_Wrapper");
        }
    }

    public class OperatorItem
    {
        public string DisplayName { get; set; }
        public string PicUrl { get; set; }
    }
}
