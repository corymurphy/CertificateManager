using CertificateManager.Entities;
using CertificateManager.Logic.Interfaces;
using CertificateManager.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace CertificateManager.Logic
{
    public class AuthorizeInitialSetup : IAuthorizationLogic
    {

        private IConfigurationRepository configurationRepository;

        public AuthorizeInitialSetup(IConfigurationRepository configurationRepository)
        {
            this.configurationRepository = configurationRepository;
        }


        public bool AuthorizedIssueCertificate()
        {
            return true;
        }

        public bool AuthorizedToManageRoles(ClaimsPrincipal user)
        {
            return true;
        }

        public bool CanViewPrivateKey(Certificate certificate, ClaimsPrincipal user)
        {
            return true;
        }

        public List<Scope> GetAvailibleScopes()
        {
            return configurationRepository.GetAvailibleScopes().ToList();
        }

        public void InitializeScopes()
        {
            configurationRepository.InsertScopes(AuthorizationScopes.InitialScopes);
        }

        public ClaimsPrincipal UserContext()
        {
            throw new NotImplementedException();
        }

        public bool IsAuthorized(Guid scopeId, ClaimsPrincipal user)
        {
            return true;
        }

        public void IsAuthorizedThrowsException(Guid scopeId, ClaimsPrincipal user)
        {
            
        }
    }
}
