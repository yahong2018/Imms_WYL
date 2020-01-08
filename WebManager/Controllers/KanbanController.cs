using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading;
using Imms.Data;
using Imms.Mes.Data.Domain;
using Imms.Security.Data;
using Imms.Security.Data.Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;

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
            if (string.IsNullOrEmpty(lineNo))
            {
                lineNo = "TEST";
            }
          //  this.LoginWithLineNo(lineNo);

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

            string hostAndPort = (Request.HttpContext.Connection.LocalIpAddress.MapToIPv4().ToString() + ":" + Request.HttpContext.Connection.LocalPort);
            string protocol = this.HttpContext.Request.IsHttps?"https":"http";
            string basePath = protocol+"://"+hostAndPort+this.HttpContext.Request.PathBase.ToString();

            ViewBag.OperatorList = OperatorList;
            ViewBag.BasePath = basePath;

            return View("Line");
        }

        private void LoginWithLineNo(string lineNo)
        {
            SystemUser user = new SystemUser();
            user.UserCode = Guid.NewGuid().ToString();
            user.Email = "admin@zhxh.com";
            user.UserName = lineNo;
            SecurityTokenDescriptor tokenDescriptor = SystemUserLogic.CreateDescriptor(user);
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            HttpContext.Session.Set(GlobalConstants.AUTHROIZATION_SESSION_KEY, System.Text.Encoding.UTF8.GetBytes(tokenString));
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

        [Route("workshop")]
        public IActionResult Workshop(){
            return View("Workshop");
        }
        
        [Route("factory")]
        public IActionResult Factory(){
            return View("Factory");
        }
    }

    public class OperatorItem
    {
        public string DisplayName { get; set; }
        public string PicUrl { get; set; }
    }
}
