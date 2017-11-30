using CertificateManager.Entities;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace CertificateManager.Logic.Interfaces
{
    public interface IAuthorizationLogic
    {
        bool CanViewPrivateKey(Certificate certificate, ClaimsPrincipal user);

        bool AuthorizedIssueCertificate();

        bool AuthorizedToManageRoles(ClaimsPrincipal user);

        void InitializeScopes();

        List<Scope> GetAvailibleScopes();

        ClaimsPrincipal UserContext();

        bool IsAuthorized(Guid scopeId, ClaimsPrincipal user);
    }
}
