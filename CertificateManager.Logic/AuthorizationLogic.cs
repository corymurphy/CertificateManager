using CertificateManager.Entities;
using CertificateManager.Entities.Enumerations;
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

        public AuthorizationLogic(IConfigurationRepository configurationRepository)
        {
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
                .Where(claim => claim.Type == IdentityAuthenticationLogic.RoleClaimIdentifier && claim.Value == roleId.ToString())
                .Any();
        }

        private bool IsAdministrator(ClaimsPrincipal user)
        {
            return user.Claims
                .Where(claim => claim.Type == IdentityAuthenticationLogic.RoleClaimIdentifier && claim.Value == RoleManagementLogic.WellKnownAdministratorRoleId.ToString())
                .ToList()
                .Any();
        }

        public bool CanViewPrivateKey(Certificate certificate, ClaimsPrincipal user)
        {
            if (certificate == null || certificate.Acl == null)
                return false;

            if (user == null)
                return false;

            if (!certificate.Acl.Any())
                return false;


            bool isAuthorized = false;

            var roles = user.Claims.Where(claim => claim.Type == IdentityAuthenticationLogic.RoleClaimIdentifier);

            var upn = user.Claims.Where(claim => claim.Type == IdentityAuthenticationLogic.UpnClaimIdentifier).FirstOrDefault();

            var uid = user.Claims.Where(claim => claim.Type == IdentityAuthenticationLogic.UidClaimIdentifier).FirstOrDefault();

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
                roles.Add(configurationRepository.GetSecurityRole( new Guid(claim.Value) ));
            }

            if(roles == null || !roles.Any())
                throw new Exception("Current user does not have any security roles");
            else
                return roles;
        }

        public void IsAuthorizedThrowsException(Guid scopeId, ClaimsPrincipal user)
        {
            if(!isAuthorized(scopeId, user))
            {
                string message = string.Format("Access denied: The current user context is not authorized for the scope {0}", AuthorizationScopes.GetScope(scopeId).Name);
                throw new UnauthorizedAccessException(message);
            }
        }

        private bool isAuthorized(Guid scopeId, ClaimsPrincipal user)
        {
            IEnumerable<Claim> claims = user.Claims.Where(claim => claim.Type == IdentityAuthenticationLogic.RoleClaimIdentifier);

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
    }
}
