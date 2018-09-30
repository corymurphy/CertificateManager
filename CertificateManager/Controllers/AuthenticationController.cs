using CertificateManager.Entities;
using CertificateManager.Logic;
using CertificateManager.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace CertificateManager.Controllers
{
    public class AuthenticationController : Controller
    {
        IdentityAuthenticationLogic authenticationLogic;
        CertificateManagementLogic certificateManagementLogic;
        IRuntimeConfigurationState runtimeConfigurationState;
        IConfigurationRepository configurationRepository;
        HttpResponseHandler http;
        bool allowDevBypass = false;
        //RoleManagementLogic roleManagement;

        public AuthenticationController(IdentityAuthenticationLogic authenticationLogic, IRuntimeConfigurationState runtimeConfigurationState, IConfigurationRepository configRepo, CertificateManagementLogic certificateManagementLogic)
        {
            this.authenticationLogic = authenticationLogic;
            this.http = new HttpResponseHandler(this);
            this.allowDevBypass = true;
            this.runtimeConfigurationState = runtimeConfigurationState;
            this.configurationRepository = configRepo;
            this.certificateManagementLogic = certificateManagementLogic;
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
        [Authorize(AuthenticationSchemes = "Windows")]
        [Route("view/auth/login/windows")]
        public JsonResult LoginWindowsAuth()
        {
            var a = User;


            return http.RespondSuccess(a);
            //return "authenticated";
            //return RedirectToAction("Profile");
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

        [HttpPost]
        [Route("auth/token-issuance/jwt/basic")]
        public string GetJwtToken()
        {
            AppConfig config = configurationRepository.GetAppConfig();

            DownloadPfxCertificateEntity certEntity = certificateManagementLogic.GetPfxCertificateContent(config.JwtCertificateId);

            string password = certificateManagementLogic.GetCertificatePassword(config.JwtCertificateId, LocalIdentityProviderLogic.GetSystemIdentity()).DecryptedPassword;

            X509Certificate2 cert = new X509Certificate2(certEntity.Content, password);

            X509SecurityKey securityKey = new X509SecurityKey(cert);

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            //signingCredentials: new SigningCredentials(new RsaSecurityKey(publicAndPrivate), SecurityAlgorithms.RsaSha256Signature),

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(),
                Issuer = config.LocalIdpIdentifier,
                IssuedAt = DateTime.Now,
                Audience = config.LocalIdpIdentifier,
                NotBefore = DateTime.Now,
                Expires = DateTime.Now.AddMinutes(config.JwtValidityPeriod),
                SigningCredentials = new SigningCredentials(
                    securityKey,
                    SecurityAlgorithms.RsaSha256Signature)
            };

            var token = handler.CreateToken(tokenDescriptor);


            return token.ToString();
        }

    }
}