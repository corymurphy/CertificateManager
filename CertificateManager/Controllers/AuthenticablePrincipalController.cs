using CertificateManager.Entities;
using CertificateManager.Logic;
using CertificateManager.Repository;
using CertificateServices.ActiveDirectory;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CertificateManager.Controllers
{
    public class AuthenticablePrincipalController : Controller
    {
        IConfigurationRepository configurationRepository;
        HttpResponseHandler http;
        UserManagementLogic userManagement;
        IdentityAuthenticationLogic authenticationLogic;

        public AuthenticablePrincipalController(IConfigurationRepository configurationRepository, IActiveDirectoryAuthenticator activeDirectoryAuthenticator)
        {
            this.configurationRepository = configurationRepository;
            this.http = new HttpResponseHandler(this);
            this.userManagement = new UserManagementLogic(configurationRepository);
            this.authenticationLogic = new IdentityAuthenticationLogic(configurationRepository, activeDirectoryAuthenticator);
        }

        [HttpGet]
        [Route("view/security/auth-principals")]
        public ActionResult AuthenticablePrincipalView()
        {
            return View("View");
        }

        [HttpPut]
        [Route("security/authenticable-principal/password")]
        public ActionResult ResetUserPassword(ResetUserPasswordViewModel model)
        {
            AuthenticablePrincipal principal = configurationRepository.GetAuthenticablePrincipal(model.Id);
            principal.PasswordHash = authenticationLogic.HashPassword(model.NewPassword);
            //principal.PasswordHash = localAuthProvider.Hash(model.NewPassword);
            configurationRepository.UpdateAuthenticablePrincipal(principal);
            return http.RespondSuccess();
        }

        [HttpGet]
        [Route("security/authenticable-principals")]
        public JsonResult GetAllAuthenticablePrincipals()
        {
            try
            {
                return http.RespondSuccess(userManagement.GetUsers());
            }
            catch
            {
                return http.RespondServerError();
            }
        }

        [HttpGet]
        [Route("security/authenticable-principals/search")]
        public JsonResult SearchAuthenticablePrincipals()
        {
            try
            {
                return http.RespondSuccess(userManagement.SearchUsers());
            }
            catch
            {
                return http.RespondServerError();
            }
        }

        [HttpPost]
        [Route("security/authenticable-principal/import")]
        public JsonResult ImportUsers(ImportUsersExternalIdentitySourceModel entity)
        {
            try
            {
                userManagement.ImportUser(entity);
                return http.RespondSuccess();
            }
            catch(Exception e)
            {
                return http.RespondServerError(e.Message);
            }
        }

        [HttpPost]
        [Route("security/authenticable-principal")]
        public JsonResult AddAuthenticablePrincipal(AuthenticablePrincipal entity)
        {
            try
            {
                AddAuthenticablePrincipalEntity result = userManagement.NewUser(entity);
                return http.RespondSuccess(result);
            }
            catch
            {
                return http.RespondServerError();
            }
        }

        [HttpDelete]
        [Route("security/authenticable-principal")]
        public JsonResult DeleteAuthenticablePrincipal(AuthenticablePrincipal entity)
        {
            try
            {
                userManagement.DeleteUser(entity);
                return http.RespondSuccess();
            }
            catch
            {
                return http.RespondServerError();
            }
        }
    
        [HttpPut]
        [Route("security/authenticable-principal")]
        public JsonResult UpdateAuthenticablePrincipal(AuthenticablePrincipal entity)
        {
            try
            {
                userManagement.SetUser(entity);
                return http.RespondSuccess();
            }
            catch
            {
                return http.RespondServerError();
            }
        }
    }
}