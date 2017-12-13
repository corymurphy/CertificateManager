using CertificateManager.Entities;
using CertificateManager.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace CertificateManager.Logic.Interfaces
{
    public interface IAuthorizationLogic
    {
        bool CanViewPrivateKey(ICertificatePasswordEntity certificate, ClaimsPrincipal user);

        bool AuthorizedIssueCertificate();

        bool AuthorizedToManageRoles(ClaimsPrincipal user);

        void InitializeScopes();

        List<Scope> GetAvailibleScopes();

        ClaimsPrincipal UserContext();

        bool IsAuthorized(Guid scopeId, ClaimsPrincipal user);

        void IsAuthorizedThrowsException(Guid scopeId, ClaimsPrincipal user, ILoggableEntity entity);

        void IsAuthorizedThrowsException(Guid scopeId, ClaimsPrincipal user, ILoggableEntity entity, EventCategory category);

        bool IsAuthorized(AdcsTemplate template, ClaimsPrincipal user);

        List<AccessControlEntry> GetDefaultCertificateAcl(ClaimsPrincipal user);
    }
}
