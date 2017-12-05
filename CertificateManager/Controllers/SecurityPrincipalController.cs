using CertificateManager.Logic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace CertificateManager.Controllers
{
    public class SecurityPrincipalController : Controller
    {
        HttpResponseHandler http;
        SecurityPrincipalLogic securityPrincipalLogic;

        public SecurityPrincipalController(SecurityPrincipalLogic securityPrincipalLogic)
        {
            this.http = new HttpResponseHandler(this);
            this.securityPrincipalLogic = securityPrincipalLogic;
        }

        [HttpGet]
        [Route("security/principals")]
        public JsonResult GetSecurityPrincipals()
        {
            return http.RespondSuccess(securityPrincipalLogic.GetSecurityPrincipals());
        }
    }
}