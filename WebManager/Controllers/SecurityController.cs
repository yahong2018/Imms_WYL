using Imms.Data;
using Imms.Security.Data;
using Imms.Security.Data.Domain;
using Imms.WebManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Imms.WebManager
{
    [Route("api/security/systemUser")]
    public class SystemUserController : SimpleCRUDController<SystemUser>
    {
        public SystemUserController() => this.Logic = new SystemUserLogic();

        [Route("userRoles"), HttpGet]
        public List<SystemRole> GetUserRoles(long userId)
        {
            List<SystemRole> result = null;
            CommonRepository.UseDbContext(dbContext =>
            {
                result = (from r in dbContext.Set<SystemRole>()
                          from ru in dbContext.Set<RoleUser>()
                          where ru.UserId == userId
                            && ru.RoleId == r.RecordId
                          select r).ToList();
            });

            return result;
        }

        [Route("resetPassword"), HttpGet]
        public int ResetPassword(long userId)
        {
            return (Logic as SystemUserLogic).ResetPassword(userId);
        }

        [Route("enable"), HttpGet]
        public int Enable(long userId)
        {
            return (Logic as SystemUserLogic).ChangeUserStatus(userId, SystemUser.USER_STATUS_ENABLED);
        }

        [Route("disable"), HttpGet]
        public int Disable(long userId)
        {
            return (Logic as SystemUserLogic).ChangeUserStatus(userId, SystemUser.USER_STATUS_DISABLED);
        }

        [Route("updateUserRoles"), HttpPost]
        public int UpdateUserRoles(long userId, [FromBody] RoleUser[] roleUsers)
        {
            return (Logic as SystemUserLogic).UpdateUserRoles(userId, roleUsers);
        }

        [Route("changeCurrentUserPassword"), HttpPost]
        public int ChangeCurrentUserPassword([FromBody] PasswordChangeItem item)
        {
            long userId = long.Parse(this.HttpContext.User.Claims.First(x => x.Type == "UserId").Value);
            return (Logic as SystemUserLogic).ChangeUserPassword(userId, item.Old, item.Pwd1, item.Pwd2);
        }

        public class PasswordChangeItem
        {
            public string Old { get; set; }
            public string Pwd1 { get; set; }
            public string Pwd2 { get; set; }
        }
    }

    [Route("api/security/systemRole")]
    public class SystemRoleController : SimpleCRUDController<SystemRole>
    {
        public SystemRoleController() => this.Logic = new SystemRoleLogic();

        [Route("rolePrivileges"), HttpGet]
        public List<RolePrivilege> GetRolePrivileges(long roleId)
        {
            return (this.Logic as SystemRoleLogic).GetRolePrivileges(roleId);
        }

        [Route("allMenuWithPrivilege"), HttpGet]
        public List<ProgramWithPrivilgeViewModel> GetAllMenuWithPrivilege()
        {
            List<SystemProgram> allPrograms = (this.Logic as SystemRoleLogic).GetAllProgramWithPrivileges()
                    .Where(x => string.IsNullOrEmpty(x.ParentId))
                    .OrderBy(x => x.ShowOrder)
                    .ToList();

            List<ProgramWithPrivilgeViewModel> result = new List<ProgramWithPrivilgeViewModel>();
            foreach (SystemProgram program in allPrograms)
            {
                ProgramWithPrivilgeViewModel menu = new ProgramWithPrivilgeViewModel(program);
                result.Add(menu);
            }

            return result;
        }

        [Route("updatePrivileges"), HttpPost]
        public int UpdatePrivileges(long roleId, [FromBody]ProgramPrivilege[] currentPrivileges)
        {
            return (this.Logic as SystemRoleLogic).UpdateRolePrivilege(roleId, currentPrivileges);
        }
    }


}