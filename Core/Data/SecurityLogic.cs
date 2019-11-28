using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Text;
using Imms.Data;
using Imms.Security.Data.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using System;
using IdentityModel;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace Imms.Security.Data
{
    public class SystemUserLogic : SimpleCRUDLogic<SystemUser>
    {
        public static SystemUser GetCurrentUser()
        {
            try
            {
                string userString = Imms.HttpContext.Current.User.Claims.First(x => x.Type == "CurrentUser").Value;
                SystemUser currentUser = userString.ToObject<SystemUser>();
                return currentUser;
            }
            catch
            {
                return null;
            }
        }

        public static async void Login(string userCode, string password, Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            SystemUser systemUser = SystemUserLogic.VerifyLoginAccount(userCode, password);
            var claims = new List<Claim>{
                {
                    new Claim("CurrentUser",systemUser.ToJson())
                }
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal user = new ClaimsPrincipal(claimsIdentity);
            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, user);
        }

        public static SecurityTokenDescriptor LoginWithApi(string userCode, string password, Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            SystemUser systemUser = SystemUserLogic.VerifyLoginAccount(userCode, password);
            byte[] key = Encoding.ASCII.GetBytes(GlobalConstants.JWT_SECRET_STRING);
            DateTime authTime = DateTime.UtcNow;
            DateTime expiresAt = authTime.AddDays(7);

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(JwtClaimTypes.Audience,"api"),
                    new Claim(JwtClaimTypes.Issuer,GlobalConstants.JWT_ISSURER_URL),
                    new Claim(JwtClaimTypes.Id, systemUser.UserCode.ToString()),
                    new Claim(JwtClaimTypes.Name, systemUser.UserName),
                    new Claim(JwtClaimTypes.Email, systemUser.Email),
                    // new Claim(JwtClaimTypes.PhoneNumber, systemUser.PhoneNumber),
                    new Claim("CurrentUser",systemUser.ToJson())
                }),
                Expires = expiresAt,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };
            return tokenDescriptor;
        }


        public static SystemUser VerifyLoginAccount(string userCode, string password)
        {
            SystemUser result = null;
            CommonRepository.UseDbContext(dbContext =>
            {
                result = dbContext.Set<SystemUser>()
                       .Where(x => x.UserCode == userCode)
                       .Include(x => x.Roles)
                            .ThenInclude(x => x.Role)
                        .FirstOrDefault();

                if (result == null || string.Compare(result.Pwd, GetMD5Hash(password), true) != 0)
                {
                    throw new BusinessException(GlobalConstants.EXCEPTION_CODE_DATA_NOT_FOUND, "用户名或密码错误!");
                }

                if (result.UserStatus == SystemUser.USER_STATUS_DISABLED)
                {
                    throw new BusinessException(GlobalConstants.EXCEPTION_CODE_CUSTOM, "账号已过期!");
                }

                result.LastLoginTime = DateTime.Now;
                dbContext.SaveChanges();
            });
            return result;
        }

        public static string GetMD5Hash(string str)
        {
            StringBuilder sb = new StringBuilder();
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
                int length = data.Length;
                for (int i = 0; i < length; i++)
                {
                    sb.Append(data[i].ToString("x2"));
                }
            }
            return sb.ToString();
        }

        protected override void BeforeInsert(SystemUser item, DbContext dbContext)
        {
            item.Pwd = SystemUserLogic.DEAFULT_PASSWORD;
        }

        protected override void BeforeUpdate(SystemUser item, DbContext dbContext)
        {
            if (string.IsNullOrEmpty(item.Pwd))
            {
                SystemUser dbItem = dbContext.Set<SystemUser>().Find(item.RecordId);
                item.Pwd = dbItem.Pwd;
            }
        }

        public int ResetPassword(long userId)
        {
            SystemUser old = CommonRepository.AssureExistsByFilter<SystemUser>(x => x.RecordId == userId);
            old.Pwd = SystemUserLogic.DEAFULT_PASSWORD;
            this.Update(old);
            return 1;
        }

        public int ChangeUserStatus(long userId, byte userStatus)
        {
            SystemUser old = CommonRepository.AssureExistsByFilter<SystemUser>(x => x.RecordId == userId);
            old.UserStatus = userStatus;
            this.Update(old);

            return 1;
        }

        public int UpdateUserRoles(long userId, RoleUser[] roleUsers)
        {
            CommonRepository.UseDbContextWithTransaction(dbContext =>
            {
                this.UpdateRoles(userId, roleUsers, dbContext);
            });
            return roleUsers.Length;
        }

        public void UpdateRoles(long userId, RoleUser[] roleUsers, DbContext dbContext)
        {
            string sql = "delete from " + ImmsDbContext.GetEntityTableName<RoleUser>() + " where user_id = @p0";
            dbContext.Database.ExecuteSqlCommand(sql, userId);

            foreach (RoleUser roleUser in roleUsers)
            {
                dbContext.Set<RoleUser>().Add(roleUser);
            }
            dbContext.SaveChanges();
        }

        public int ChangeUserPassword(long userId, string old, string pwd1, string pwd2)
        {
            string oldMd5 = GetMD5Hash(old);
            SystemUser user = CommonRepository.AssureExistsByFilter<SystemUser>(x => x.RecordId == userId);
            if (string.Compare(user.Pwd, oldMd5, true) != 0)
            {
                return -2;
            }
            string pwd1Md5 = GetMD5Hash(pwd1);
            string pwd2Md5 = GetMD5Hash(pwd2);
            if (string.Compare(pwd1Md5, pwd2Md5, true) != 0)
            {
                return -1;
            }
            user.Pwd = pwd2Md5;
            this.Update(user);

            return 0;
        }

        public static readonly string DEAFULT_PASSWORD = GetMD5Hash("888888");
    }

    public class SystemRoleLogic : SimpleCRUDLogic<SystemRole>
    {
        public List<SystemProgram> GetAllProgramWithPrivileges()
        {
            return
            GlobalConstants.DbContextFactory.GetContext().Set<SystemProgram>()
            .Include(x => x.Children)
            .Include(x => x.Privielges)
            .ToList();
        }

        public List<RolePrivilege> GetRolePrivileges(long roleId)
        {
            List<RolePrivilege> result = GlobalConstants.DbContextFactory.GetContext().Set<RolePrivilege>()
            .Where(x => x.RoleId == roleId)
            .ToList();
            return result;
        }

        public int UpdateRolePrivilege(long roleId, ProgramPrivilege[] currentPrivileges)
        {
            CommonRepository.UseDbContextWithTransaction(dbContext =>
            {
                List<RolePrivilege> oldPrivileges = dbContext.Set<RolePrivilege>().Where(x => x.RoleId == roleId).ToList();
                this.Revoke(oldPrivileges, currentPrivileges, dbContext);
                this.AssignNew(roleId, oldPrivileges, currentPrivileges, dbContext);

                dbContext.SaveChanges();
            });
            return currentPrivileges.Length;
        }

        private void AssignNew(long roleId, List<RolePrivilege> oldPrivileges, ProgramPrivilege[] currentPrivileges, DbContext dbContext)
        {
            long[] privilegeIdList = oldPrivileges.Select(x => x.ProgramPrivilegeId).ToArray();
            List<ProgramPrivilege> assignList = currentPrivileges.Where(x => !privilegeIdList.Contains(x.RecordId)).ToList();
            foreach (ProgramPrivilege privilege in assignList)
            {
                RolePrivilege rolePrivilege = new RolePrivilege();
                rolePrivilege.RoleId = roleId;
                rolePrivilege.ProgramId = privilege.ProgramId;
                rolePrivilege.PrivilegeCode = privilege.PrivilegeCode;
                rolePrivilege.ProgramPrivilegeId = privilege.RecordId;

                dbContext.Set<RolePrivilege>().Add(rolePrivilege);
            }
        }

        private void Revoke(List<RolePrivilege> oldPrivileges, ProgramPrivilege[] currentPrivileges, DbContext dbContext)
        {
            long[] privielgeIdList = currentPrivileges.Select(x => x.RecordId).ToArray();
            List<RolePrivilege> revokedList = oldPrivileges.Where(x => !privielgeIdList.Contains(x.ProgramPrivilegeId)).ToList();
            foreach (RolePrivilege revokedPrivilege in revokedList)
            {
                dbContext.Set<RolePrivilege>().Remove(revokedPrivilege);
            }
        }
    }
}