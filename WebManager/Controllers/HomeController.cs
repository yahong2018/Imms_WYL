using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Imms.WebManager.Models;
using Imms.Data;
using Imms.Security.Data.Domain;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Imms.Security.Data;
using Microsoft.AspNetCore.Authorization;
using Imms.Mes.Data.Domain;
using Microsoft.AspNetCore.Http;

namespace Imms.WebManager.Controllers
{
    [Authorize]
    [Route("/")]
    [Route("/home")]
    public class HomeController : Controller
    {
        private readonly IHostingEnvironment host = null;
        public HomeController(IHostingEnvironment host)
        {
            this.host = host;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("keepAlive")]
        public string KeepAlive(){
            GlobalConstants.DefaultLogger.Info("客户端心跳:"+GlobalConstants.GetCurrentUser().UserCode);

            return "{}";
        }

        [Route("userMenu"), HttpGet]
        public List<SystemProgram> GetUserMenu()
        {
            long userId = GlobalConstants.GetCurrentUser().RecordId;
            List<SystemProgram> programList = new List<SystemProgram>();
            CommonRepository.UseDbContext(dbContext =>
            {
                var allPrograms = (from p in dbContext.Set<SystemProgram>()
                                   from rp in dbContext.Set<RolePrivilege>()
                                   from r in dbContext.Set<RoleUser>()
                                   where p.RecordId == rp.ProgramId
                                      && rp.RoleId == r.RoleId
                                      && r.UserId == userId
                                      && p.ProgramStatus == 0
                                   select p
                ).Include(x => x.Privielges)
                .ToList();

                programList.AddRange(allPrograms.Where(x => string.IsNullOrEmpty(x.ParentId)));
                foreach (SystemProgram program in programList)
                {
                    this.fillMenu(allPrograms, program);
                }
            });
            return programList;
        }

        private void fillMenu(List<SystemProgram> allPrograms, SystemProgram program)
        {
            string[] childIdList = program.Children.Select(x => x.RecordId).ToArray();
            program.Children.AddRange(allPrograms.Where(x => x.ParentId == program.RecordId && !childIdList.Contains(x.RecordId)));
            foreach (SystemProgram child in program.Children)
            {
                fillMenu(allPrograms, child);
            }
        }

        [Route("currentLogin"), HttpGet]
        public ActionResult<string> GetCurrentLogin()
        {
            SystemUser currentUser = (SystemUser)GlobalConstants.GetCurrentUser();
            string loginText = null;

            CommonRepository.UseDbContext(dbContext =>
            {
                var privilegeList = (
                    from rp in dbContext.Set<RolePrivilege>()
                    from ur in dbContext.Set<RoleUser>()
                    from r in dbContext.Set<SystemRole>()
                    where rp.RoleId == ur.RoleId
                       && r.RecordId == ur.RoleId
                       && ur.UserId == currentUser.RecordId
                    select r
                ).Distinct()
                .SelectMany(x => x.Privileges).ToList();

                string privilegeText = GlobalConstants.ToJson(privilegeList);

                loginText = "{\"company\":\"威雅利实业有限公司\","
                           + "\"userName\":\"" + currentUser.UserName + "\","
                           + "\"userCode\":\"" + currentUser.UserCode + "\","
                           + "\"privielges\":" + privilegeText
                           + "}";
            });
            return loginText;
        }


    }
}
