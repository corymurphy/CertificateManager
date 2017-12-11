using CertificateManager.Entities;
using CertificateManager.Logic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CertificateManager.Controllers
{
    public class AuthenticationController : Controller
    {
        IdentityAuthenticationLogic authenticationLogic;
        IRuntimeConfigurationState runtimeConfigurationState;
        HttpResponseHandler http;
        bool allowDevBypass = false;
        //RoleManagementLogic roleManagement;

        public AuthenticationController(IdentityAuthenticationLogic authenticationLogic, IRuntimeConfigurationState runtimeConfigurationState)
        {
            this.authenticationLogic = authenticationLogic;
            this.http = new HttpResponseHandler(this);
            this.allowDevBypass = true;
            this.runtimeConfigurationState = runtimeConfigurationState;
        }

        [HttpGet]
        [Route("view/auth/login")]
        public ActionResult Login()
        {
            if (runtimeConfigurationState.IsDevelopment)
            {
                ViewBag.ByPassAuth = true;
                ViewBag.FormAction = "/auth/login/dev-bypass";
            }
                
            else
            {
                ViewBag.ByPassAuth = false;
                ViewBag.FormAction = "/view/auth/login";
            }
            



            return View("Login");
        }

        [HttpPost]
        [Route("view/auth/login")]
        public async Task<ActionResult> Login(LoginLocalViewModel model)
        {
            try
            {
                await HttpContext.SignInAsync(authenticationLogic.Authenticate(model));
                return RedirectToAction("Index", "Home");
            }
            catch(Exception e)
            {
                return RedirectToAction("Login", "Authentication", new { status = "authentication_failure" });
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

        [HttpPost]
        [Route("auth/login/dev-bypass")]
        public async Task<ActionResult> AuthenticateDevBypass()
        {
            if(runtimeConfigurationState.IsDevelopment)
            {
                await HttpContext.SignInAsync(authenticationLogic.Authenticate("cmurphy"));
                return Redirect(Url.Content("~/")); 
            }
            else
            {
                return http.RespondBadRequest("Resource does not exist");
            }
            
        }
    }
}