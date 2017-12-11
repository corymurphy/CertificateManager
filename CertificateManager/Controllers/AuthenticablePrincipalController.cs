using CertificateManager.Entities;
using CertificateManager.Logic;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CertificateManager.Controllers
{
    public class AuthenticablePrincipalController : Controller
    {
        HttpResponseHandler http;
        UserManagementLogic userManagement;
        IdentityAuthenticationLogic authenticationLogic;

        public AuthenticablePrincipalController( UserManagementLogic userManagement, IdentityAuthenticationLogic identityAuthenticationLogic)
        {
            this.http = new HttpResponseHandler(this);
            this.userManagement = userManagement;
            this.authenticationLogic = identityAuthenticationLogic;
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
            userManagement.SetPassword(model);
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
        public JsonResult ImportUsers(ImportUsersActiveDirectoryMetadataModel entity)
        {
            try
            {
                userManagement.ImportUser(entity, User);
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
                AddAuthenticablePrincipalEntity result = userManagement.NewUser(entity, User);
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
                userManagement.DeleteUser(entity, User);
                return http.RespondSuccess();
            }
            catch
            {
                return http.RespondServerError();
            }
        }
    
        [HttpPut]
        [Route("security/authenticable-principal")]
        public JsonResult UpdateAuthenticablePrincipal(UpdateUserModel entity)
        {
            return http.RespondSuccess(userManagement.SetUser(entity, User));
        }
    }
}