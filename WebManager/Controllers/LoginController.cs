using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Imms.Security.Data;
using Imms.Security.Data.Domain;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Imms.WebManager.Controllers
{
    [TypeFilter(typeof(Microsoft.AspNetCore.Mvc.Authorization.AllowAnonymousFilter))]
    public class LoginController : Controller
    {
        private const string ID_ERROR_MESSAGE = "ErrorMessage";

        public ActionResult Index()
        {
            ViewBag.ErrorMessage = this.TempData[ID_ERROR_MESSAGE];
            return View();
        }

        [HttpPost]
        public ActionResult Login(string userCode, string password)
        {
            if (string.IsNullOrEmpty(userCode) || string.IsNullOrEmpty(password))
            {
                this.TempData[LoginController.ID_ERROR_MESSAGE] = "账号和密码都必须输入！";
                return RedirectToAction("Index");
            }
            try
            {
                SecurityTokenDescriptor tokenDescriptor = SystemUserLogic.LoginWithApi(userCode, password, this.HttpContext);
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);
                HttpContext.Session.Set(GlobalConstants.AUTHROIZATION_SESSION_KEY, System.Text.Encoding.UTF8.GetBytes(tokenString));

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                this.TempData[ID_ERROR_MESSAGE] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult ApiLogin(string userCode, string password)
        {
            if (string.IsNullOrEmpty(userCode) || string.IsNullOrEmpty(password))
            {
                return Unauthorized("账号和密码没有输入!");
            }
            try
            {
                byte[] buffer = Convert.FromBase64String(password);
                string decodedPasswrod = System.Text.Encoding.ASCII.GetString(buffer);

                SecurityTokenDescriptor tokenDescriptor = SystemUserLogic.LoginWithApi(userCode, decodedPasswrod, this.HttpContext);
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                HttpContext.Response.Headers.Add("Authorization", "Bearer " + tokenString);
                return Ok(new
                {
                    access_token = tokenString, 
                    token_type = "Bearer",
                    profile = new
                    {
                        sid = userCode,
                        name = userCode,
                        auth_time = new DateTimeOffset(tokenDescriptor.Expires.Value.AddDays(-7)).ToUnixTimeSeconds(),
                        expires_at = new DateTimeOffset(tokenDescriptor.Expires.Value).ToUnixTimeSeconds()
                    }
                });
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        public async Task<ActionResult> Logout()
        {
            this.HttpContext.Session.Remove(GlobalConstants.AUTHROIZATION_SESSION_KEY);
            await this.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}