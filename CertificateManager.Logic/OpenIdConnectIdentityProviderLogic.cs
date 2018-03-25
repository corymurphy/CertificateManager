using CertificateManager.Entities;
using CertificateManager.Entities.Exceptions;
using CertificateManager.Logic.Interfaces;
using CertificateManager.Repository;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace CertificateManager.Logic
{
    public class OpenIdConnectIdentityProviderLogic : IOpenIdConnectIdentityProviderLogic
    {
        IConfigurationRepository configurationRepository;
        IAuthorizationLogic authorizationLogic;

        public List<OidcIdentityProvider> RegisteredIdps { get; set; }

        public OpenIdConnectIdentityProviderLogic(IConfigurationRepository configurationRepository, IAuthorizationLogic authorizationLogic)
        {
            this.configurationRepository = configurationRepository;
            this.authorizationLogic = authorizationLogic;
            this.RegisteredIdps = new List<OidcIdentityProvider>();
        }

        public IEnumerable<OidcIdentityProvider> GetIdentityProviders()
        {
            return configurationRepository.GetAll<OidcIdentityProvider>();
        }

        public IEnumerable<OidcIdentityProvider> GetIdps(ClaimsPrincipal user)
        {
            if(!authorizationLogic.IsAuthorized(AuthorizationScopes.ManageIdentityProviders, user))
            {
                throw new UnauthorizedAccessException("Access Denied: This current user principal is not authorized to view OpenId Connect Identity Providers.");
            }

            IEnumerable<OidcIdentityProvider> idpList = configurationRepository.GetAll<OidcIdentityProvider>();

            if (idpList == null)
            {
                return new List<OidcIdentityProvider>();
            }
            else
            {
                return idpList;
            }
        }

        public void UpdateIdp(OidcIdentityProvider idp, ClaimsPrincipal user)
        {
            
            if(idp == null)
            {
                throw new Exception("Identity provider was not provided.");
            }

            authorizationLogic.IsAuthorizedThrowsException(AuthorizationScopes.ManageIdentityProviders, user, idp);

            if (!configurationRepository.Exists<OidcIdentityProvider>(idp.Id))
            {
                throw new Exception("Identity provider does not exist");
            }

            configurationRepository.Update<OidcIdentityProvider>(idp);
        }

        public void DeleteIdp(OidcIdentityProvider idp, ClaimsPrincipal user)
        {
            if (idp == null)
            {
                throw new Exception("Identity provider was not provided.");
            }

            authorizationLogic.IsAuthorizedThrowsException(AuthorizationScopes.ManageIdentityProviders, user, idp);

            if (!configurationRepository.Exists<OidcIdentityProvider>(idp.Id))
            {
                throw new Exception("Identity provider does not exist");
            }

            configurationRepository.Delete<OidcIdentityProvider>(idp.Id);
        }

        public void AddIdp(OidcIdentityProvider idp, ClaimsPrincipal user)
        {
            try
            {
                new System.Uri(idp.Authority);
            }
            catch
            {
                throw new ObjectNotInCorrectStateException("Authority must be a well formed base URL without the well-known oidc path");
            }

            if (string.IsNullOrEmpty(idp.ClientId) || string.IsNullOrEmpty(idp.Name))
            {
                throw new ObjectNotInCorrectStateException("Client Id and Name must be provided for this Idp");
            }

            configurationRepository.Insert<OidcIdentityProvider>(idp);
        }

        public void InitializeMiddleware(IServiceCollection services)
        {
            foreach (OidcIdentityProvider idp in this.GetIdentityProviders())
            {
                services.AddAuthentication().AddOpenIdConnect
                (
                    options =>
                    {
                        options.Authority = idp.Authority;
                        options.ClientId = idp.ClientId;
                        options.Scope.Add("email");
                        options.CallbackPath = new Microsoft.AspNetCore.Http.PathString( @"/" +  idp.Id );
                    }
                );
            }
        }

        
    }
}
