using CertificateManager.Entities;
using CertificateManager.Logic;
using CertificateManager.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;

using Microsoft.AspNetCore.Authentication;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace CertificateManager.Controllers
{
    public class AuthenticationController : Controller
    {
        IConfigurationRepository configurationRepository;
        HttpResponseHandler http;
        //RoleManagementLogic roleManagement;

        public AuthenticationController(IConfigurationRepository configurationRepository)
        {
            this.configurationRepository = configurationRepository;
            this.http = new HttpResponseHandler(this);
            //this.roleManagement = new RoleManagementLogic(configurationRepository);
        }

        [HttpGet]
        [Route("view/auth/login")]
        public ActionResult Login()
        {
            return View("Login");
        }

        [HttpPost]
        [Route("view/auth/login")]
        public async Task<ActionResult> Login(LoginLocalViewModel model)
        {
            IdentityAuthenticationLogic authenticationLogic = new IdentityAuthenticationLogic(configurationRepository);

            try
            {
                await HttpContext.SignInAsync(authenticationLogic.AuthenticateLocal(model.UserPrincipalName, model.Password));
                return RedirectToAction("Profile");
            }
            catch
            {
                return RedirectToAction("Login");
            } 
        }


        [HttpGet]
        [Authorize]
        [Route("view/auth/profile")]
        public ActionResult Profile()
        {
            var a = User;

            return View("Profile");
        }

        [HttpGet]
        [Authorize]
        [Route("view/auth/logout")]
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            return View("Logout");
        }

        [HttpGet]
        [Route("view/auth/forbidden")]
        public ActionResult Forbidden()
        {
            HttpContext.Response.StatusCode = 403;

            return View("Forbidden");
        }

        [HttpGet]
        [Authorize]
        [Route("view/auth/login/windows")]
        public ActionResult LoginWindowsAuth()
        {
            var a = User;

            return RedirectToAction("Profile");
        }

        [HttpGet]
        [Route("view/auth/login/oidc")]
        public ActionResult AuthenticateOpenIdConnect()
        {
            return Challenge("OidcPrimary");
        }

    }
}