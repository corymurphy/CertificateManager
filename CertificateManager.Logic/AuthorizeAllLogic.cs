using CertificateManager.Entities;
using CertificateManager.Logic.Interfaces;
using CertificateManager.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace CertificateManager.Logic
{
    public class AuthorizeAllLogic : IAuthorizationLogic
    {

        private IConfigurationRepository configurationRepository;

        public AuthorizeAllLogic(IConfigurationRepository configurationRepository)
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
            List<Scope> scopes = new List<Scope>();

            scopes.Add(new Scope("ManageUsers", new Guid("b70db756-6d0a-4d69-8784-733fbb243594")));
            scopes.Add(new Scope("ManageRoles", AuthorizationScopes.ManageRolesScope));
            scopes.Add(new Scope("ManageIdentityProviders", new Guid("9c7c5e85-264e-49d2-9ddf-07065a0f90c2")));
            scopes.Add(new Scope("ManageCertificateAuthorities", new Guid("dcc20573-4969-4d59-966a-d945657a5888")));
            scopes.Add(new Scope("ManageAdcsTemplates", new Guid("a8ce22d2-98c6-44bc-9379-ada9910ac0ca")));
            scopes.Add(new Scope("CertificateFullControl", new Guid("266cea37-be5d-482e-9723-7cbf54777feb")));
            scopes.Add(new Scope("IssuePendingCertificates", new Guid("b517d658-e884-4bcb-a599-73a8f7f986c4")));
            scopes.Add(new Scope("DeleteCertificates", new Guid("02803f7c-5cf3-4bb4-8f57-1dc940035f5b")));

            configurationRepository.InsertScopes(scopes);
        }

        public ClaimsPrincipal UserContext()
        {
            throw new NotImplementedException();
        }

        public bool IsAuthorized(Guid scopeId, ClaimsPrincipal user)
        {
            return true;
        }

    }
}
