﻿using CertificateManager.Entities;
using CertificateManager.Entities.Enumerations;
using CertificateManager.Entities.Extensions;
using CertificateManager.Entities.Interfaces;
using CertificateManager.Logic.Interfaces;
using CertificateManager.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace CertificateManager.Logic
{
    public class AuthorizationLogic : IAuthorizationLogic
    {
        private IConfigurationRepository configurationRepository;
        private IAuditLogic audit;

        public AuthorizationLogic(IConfigurationRepository configurationRepository, IAuditLogic audit)
        {
            this.audit = audit;
            this.configurationRepository = configurationRepository;
        }

        public bool AuthorizedIssueCertificate()
        {
            return false;
        }

        public bool AuthorizedToManageRoles(ClaimsPrincipal user)
        {
            if (IsAdministrator(user))
                return true;
            else
                return false;
        }

        private bool IncludesRole(ClaimsPrincipal user, Guid roleId)
        {
            return user.Claims
                .Where(claim => claim.Type == WellKnownClaim.Role && claim.Value == roleId.ToString())
                .Any();
        }

        private bool IsAdministrator(ClaimsPrincipal user)
        {
            return user.Claims
                .Where(claim => claim.Type == WellKnownClaim.Role && claim.Value == RoleManagementLogic.WellKnownAdministratorRoleId.ToString())
                .ToList()
                .Any();
        }

        public bool CanViewPrivateKey(ICertificatePasswordEntity certificate, ClaimsPrincipal user)
        {
            if (certificate == null || certificate.Acl == null)
                return false;

            if (user.GetUserId() == LocalIdentityProviderLogic.SystemUid)
                return true;

            if (user == null)
                return false;

            if (!certificate.Acl.Any())
                return false;


            bool isAuthorized = false;

            var roles = user.Claims.Where(claim => claim.Type == WellKnownClaim.Role);

            var upn = user.Claims.Where(claim => claim.Type == WellKnownClaim.Name).FirstOrDefault();

            var uid = user.Claims.Where(claim => claim.Type == WellKnownClaim.Uid).FirstOrDefault();

            foreach (AccessControlEntry ace in certificate.Acl)
            {
                //If the ACE is expired, just ignore the ace
                if (ace.Expires < DateTime.Now)
                    continue;

                if (ace.IdentityType == IdentityType.Role)
                {
                    foreach (var role in roles)
                    {
                        if (role.Value == ace.Identity & ace.AceType == AceType.Deny)
                        {
                            return false;
                        }
                        else if (role.Value == ace.Identity & ace.AceType == AceType.Allow)
                        {
                            isAuthorized = true;
                        }
                    }
                }



                if (ace.IdentityType == IdentityType.User)
                {
                    if (ace.Identity == uid.Value & ace.AceType == AceType.Deny)
                    {
                        return false;
                    }
                    else if (ace.Identity == uid.Value & ace.AceType == AceType.Allow)
                    {
                        isAuthorized = true;
                    }
                }

            }



            return isAuthorized;
        }

        public void InitializeScopes()
        {
            throw new InvalidOperationException("This method can only be used within the initial setup");
        }

        public List<Scope> GetAvailibleScopes()
        {
            return configurationRepository.GetAvailibleScopes().ToList();
        }

        public ClaimsPrincipal UserContext()
        {
            throw new NotImplementedException();
        }

        public bool IsAuthorized(Guid scopeId, ClaimsPrincipal user)
        {
            return isAuthorized(scopeId, user);
        }

        public List<SecurityRole> GetSecurityRolesFromClaims(IEnumerable<Claim> claims)
        {
            if (claims == null || !claims.Any())
                throw new ArgumentNullException(nameof(claims));

            List<SecurityRole> roles = new List<SecurityRole>();

            foreach(Claim claim in claims)
            {
                roles.Add(configurationRepository.Get<SecurityRole>( new Guid(claim.Value) ));
            }

            if(roles == null || !roles.Any())
                throw new Exception("Current user does not have any security roles");
            else
                return roles;
        }

        public void IsAuthorizedThrowsException(Guid scopeId, ClaimsPrincipal user, ILoggableEntity entity)
        {
            if(!isAuthorized(scopeId, user))
            {
                string message = string.Format("Access denied: The current user context is not authorized for the scope {0}", AuthorizationScopes.GetScope(scopeId).Name);
                throw new UnauthorizedAccessException(message);
            }
        }

        public void IsAuthorizedThrowsException(Guid scopeId, ClaimsPrincipal user, ILoggableEntity entity, EventCategory category)
        {
            if(isAuthorized(scopeId, user))
            {
                audit.LogSecurityAuditSuccess(user, entity, category);
            }

            if (!isAuthorized(scopeId, user))
            {
                string message = string.Format("Access denied: The current user context is not authorized for the scope {0}", AuthorizationScopes.GetScope(scopeId).Name);
                throw new UnauthorizedAccessException(message);
            }
        }

        private bool isAuthorized(Guid scopeId, ClaimsPrincipal user)
        {
            IEnumerable<Claim> claims = user.Claims.Where(claim => claim.Type == WellKnownClaim.Role);

            List<SecurityRole> roles;
            try
            {
                roles = GetSecurityRolesFromClaims(claims);
            }
            catch (Exception e)
            {
                return false;
            }

            foreach (SecurityRole role in roles)
            {
                if (role.Scopes.Contains(scopeId))
                    return true;
            }

            return false;
        }

        public bool IsAuthorized(AdcsTemplate template, ClaimsPrincipal user)
        {
            if(template == null || template.RolesAllowedToIssue == null)
            {
                throw new ArgumentNullException(nameof(template));
            }

            if(user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if(template.RolesAllowedToIssue.Count < 1 )
            {
                throw new UnauthorizedAccessException();
            }

            foreach(Guid roleId in template.RolesAllowedToIssue)
            {
                if(IncludesRole(user, roleId))
                {
                    return true;
                }
            }

            return false;

        }

        public List<AccessControlEntry> GetDefaultCertificateAcl(ClaimsPrincipal user)
        {
            List<AccessControlEntry> defaultAcl = new List<AccessControlEntry>();

            defaultAcl.Add(new AccessControlEntry()
            {
                Expires = DateTime.MaxValue,
                AceType = AceType.Allow,
                Id = Guid.NewGuid(),
                IdentityType = IdentityType.User,
                Identity = user.Identity.Name
            });

            defaultAcl.Add(new AccessControlEntry()
            {
                Expires = DateTime.MaxValue,
                AceType = AceType.Allow,
                Id = Guid.NewGuid(),
                IdentityType = IdentityType.Role,
                Identity = RoleManagementLogic.WellKnownAdministratorRoleId.ToString()
            });

            return defaultAcl;
        }
    }
}
