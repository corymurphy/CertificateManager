using CertificateManager.Entities;
using CertificateManager.Entities.Exceptions;
using CertificateManager.Logic.Interfaces;
using CertificateManager.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace CertificateManager.Logic
{
    public class RoleManagementLogic
    {
        private const string wellKnownAdministratorRoleId = "092020ed-57cf-4af6-a1f6-6e1fdd7f5e8a";

        public static Guid WellKnownAdministratorRoleId { get { return new Guid(wellKnownAdministratorRoleId); } }
        public static List<Guid> DefaultTemplateIssuerRoles { get { return new List<Guid>() { WellKnownAdministratorRoleId }; } }

        IConfigurationRepository configurationRepository;
        IAuthorizationLogic authorizationLogic;

        public RoleManagementLogic(IConfigurationRepository configurationRepository, IAuthorizationLogic authorizationLogic)
        {
            this.configurationRepository = configurationRepository;
            this.authorizationLogic = authorizationLogic;
        }

        public SecurityRole GetRole(Guid id)
        {
            return configurationRepository.GetSecurityRole(id);
        }

        public IEnumerable<SecurityRole> GetRoles()
        {
            return configurationRepository.GetSecurityRoles();
        }

        public SecurityRole AddRole(SecurityRole entity, ClaimsPrincipal user)
        {
            entity.Id = Guid.NewGuid();
            configurationRepository.InsertSecurityRole(entity);
            return entity;
        }

        public void DeleteRole(SecurityRole securityRole, ClaimsPrincipal user)
        {
            if (authorizationLogic.AuthorizedToManageRoles(user))
                throw new UnauthorizedAccessException("Current user context is not authorized to manage roles");

            if (securityRole.Id == RoleManagementLogic.WellKnownAdministratorRoleId)
                throw new InvalidOperationException("The built-in administrator account cannot be deleted");

            configurationRepository.DeleteSecurityRole(securityRole);
        }

        public void UpdateRole(SecurityRole securityRole, ClaimsPrincipal user)
        {
            configurationRepository.UpdateSecurityRole(securityRole);
        }

        public List<AuthenticablePrincipal> GetRoleMembers(Guid id)
        {
            SecurityRole role = configurationRepository.GetSecurityRole(id);

            SecurityRoleDetailsView roleView = new SecurityRoleDetailsView()
            {
                Id = role.Id,
                Enabled = role.Enabled,
                Name = role.Name
            };

            if (role.Member == null)
                return new List<AuthenticablePrincipal>();

            List<AuthenticablePrincipal> members = new List<AuthenticablePrincipal>();
            foreach (Guid memberId in role.Member)
            {
                members.Add(configurationRepository.GetAuthenticablePrincipal<AuthenticablePrincipal>(memberId));
            }

            return members;
        }

        public void DeleteRoleMember(Guid roleId, Guid memberId)
        {
            SecurityRole role = configurationRepository.GetSecurityRole(roleId);
            role.Member = role.Member.Where(member => member != memberId).ToList();
            configurationRepository.UpdateSecurityRole(role);
        }

        public AuthenticablePrincipal AddRoleMember(Guid roleId, Guid memberId)
        {
            AuthenticablePrincipal principal = configurationRepository.GetAuthenticablePrincipal<AuthenticablePrincipal>(memberId);

            if (principal == null)
                throw new ReferencedObjectDoesNotExistException("Specified member id was not found. No changes have been made.");

            SecurityRole role = configurationRepository.GetSecurityRole(roleId);

            if (role == null)
                throw new ReferencedObjectDoesNotExistException("Specified role id was not found. No changes have been made.");

            if (role.Member == null)
                role.Member = new List<Guid>();

            role.Member.Add(principal.Id);
            configurationRepository.UpdateSecurityRole(role);

            return principal;
        }

        public SecurityRole GetCertificateManagerAdministrator()
        {
            return configurationRepository.GetSecurityRole(WellKnownAdministratorRoleId);
        }

        public void SetRoleScopes(Guid roleId, List<Guid> scopes, ClaimsPrincipal user)
        {
            if (!authorizationLogic.IsAuthorized(AuthorizationScopes.ManageRolesScope, user))
                throw new UnauthorizedAccessException("Access denied: scope change will not be performed.");

            SecurityRole role = configurationRepository.GetSecurityRole(roleId);

            List<Scope> validScopes = authorizationLogic.GetAvailibleScopes();
            if(scopes != null && scopes.Any() )
            {
                foreach (Guid scope in scopes)
                {
                    if (!validScopes.Select(item => item.Id).Contains(scope))
                        throw new ReferencedObjectDoesNotExistException("Requested scope does not exist");
                }
            }
            else
            {
                scopes = new List<Guid>();
            }

            role.Scopes = scopes;

            configurationRepository.UpdateSecurityRole(role);
        }

        public void InitializeRoles(List<Guid> users)
        {
            SecurityRole adminRole = new SecurityRole()
            {
                Enabled = true,
                Id = WellKnownAdministratorRoleId,
                Member = users,
                Name = "Administrators@certificatemanager.local",
                Scopes = authorizationLogic.GetAvailibleScopes().Select(scope => scope.Id).ToList()
            };

            configurationRepository.InsertSecurityRole(adminRole);
        }
    }

}
