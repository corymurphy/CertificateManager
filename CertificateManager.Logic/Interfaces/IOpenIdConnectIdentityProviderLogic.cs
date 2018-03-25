using CertificateManager.Entities;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Security.Claims;

namespace CertificateManager.Logic.Interfaces
{
    public interface IOpenIdConnectIdentityProviderLogic
    {
        void AddIdp(OidcIdentityProvider idp, ClaimsPrincipal user);
        void DeleteIdp(OidcIdentityProvider idp, ClaimsPrincipal user);
        void UpdateIdp(OidcIdentityProvider idp, ClaimsPrincipal user);
        IEnumerable<OidcIdentityProvider> GetIdps(ClaimsPrincipal user);
        void InitializeMiddleware(IServiceCollection services);

    }
}
