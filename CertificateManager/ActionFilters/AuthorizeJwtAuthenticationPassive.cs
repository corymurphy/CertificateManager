using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CertificateManager.ActionFilters
{
    public class AuthorizeJwtAuthenticationPassive : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {

            throw new System.NotImplementedException();
        }
    }
}
