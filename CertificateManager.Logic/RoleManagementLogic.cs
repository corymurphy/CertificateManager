using CertificateManager.Entities;
using CertificateManager.Entities.Exceptions;
using CertificateManager.Entities.Models;
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
            return configurationRepository.Get<SecurityRole>(id);
        }

        public IEnumerable<SecurityRole> GetRoles()
        {
            return configurationRepository.GetAll<SecurityRole>();
        }

        public SecurityRole AddRole(SecurityRole entity, ClaimsPrincipal user)
        {
            entity.Id = Guid.NewGuid();

            authorizationLogic.IsAuthorizedThrowsException(AuthorizationScopes.ManageRoles, user, entity, EventCategory.RoleManagementNew);
            
            configurationRepository.Insert<SecurityRole>(entity);
            return entity;
        }

        public void DeleteRole(SecurityRole securityRole, ClaimsPrincipal user)
        {
            authorizationLogic.IsAuthorizedThrowsException(AuthorizationScopes.ManageRoles, user, securityRole, EventCategory.RoleManagementDelete);

            if (securityRole.Id == RoleManagementLogic.WellKnownAdministratorRoleId)
                throw new InvalidOperationException("The built-in administrator account cannot be deleted");

            configurationRepository.Delete<SecurityRole>(securityRole.Id);
        }

        public void UpdateRole(SecurityRole securityRole, ClaimsPrincipal user)
        {
            authorizationLogic.IsAuthorizedThrowsException(AuthorizationScopes.ManageRoles, user, securityRole, EventCategory.RoleManagementUpdate);

            configurationRepository.Update<SecurityRole>(securityRole);
        }

        public List<AuthenticablePrincipal> GetRoleMembers(Guid id)
        {
            SecurityRole role = configurationRepository.Get<SecurityRole>(id);

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
                members.Add(configurationRepository.Get<AuthenticablePrincipal>(memberId));
            }

            return members;
        }

        public void DeleteRoleMember(DeleteSecurityRoleMemberModel model, ClaimsPrincipal user)
        {
            authorizationLogic.IsAuthorizedThrowsException(AuthorizationScopes.ManageRoles, user, model, EventCategory.RoleManagementUpdate);

            SecurityRole role = configurationRepository.Get<SecurityRole>(model.RoleId);
            role.Member = role.Member.Where(member => member != model.MemberId).ToList();
            configurationRepository.Update<SecurityRole>(role);
        }

        public AuthenticablePrincipal AddRoleMember(AddSecurityRoleMemberModel model, ClaimsPrincipal user)
        {
            authorizationLogic.IsAuthorizedThrowsException(AuthorizationScopes.ManageRoles, user, model, EventCategory.RoleManagementAddMember);

            AuthenticablePrincipal principal = configurationRepository.Get<AuthenticablePrincipal>(model.MemberId);

            if (principal == null)
                throw new ReferencedObjectDoesNotExistException("Specified member id was not found. No changes have been made.");

            SecurityRole role = configurationRepository.Get<SecurityRole>(model.RoleId);

            if (role == null)
                throw new ReferencedObjectDoesNotExistException("Specified role id was not found. No changes have been made.");

            if (role.Member == null)
                role.Member = new List<Guid>();

            role.Member.Add(principal.Id);
            configurationRepository.Update<SecurityRole>(role);

            return principal;
        }

        public SecurityRole GetCertificateManagerAdministrator()
        {
            return configurationRepository.Get<SecurityRole>(WellKnownAdministratorRoleId);
        }

        public void SetRoleScopes(SetRoleScopesModel model, ClaimsPrincipal user)
        {
            authorizationLogic.IsAuthorizedThrowsException(AuthorizationScopes.ManageRoles, user, model, EventCategory.RoleManagementSetScopes);

            SecurityRole role = configurationRepository.Get<SecurityRole>(model.RoleId);

            List<Scope> validScopes = authorizationLogic.GetAvailibleScopes();
            if(model.Scopes != null && model.Scopes.Any() )
            {
                foreach (Guid scope in model.Scopes)
                {
                    if (!validScopes.Select(item => item.Id).Contains(scope))
                        throw new ReferencedObjectDoesNotExistException("Requested scope does not exist");
                }
            }
            else
            {
                model.Scopes = new List<Guid>();
            }

            role.Scopes = model.Scopes;

            configurationRepository.Update<SecurityRole>(role);
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

            configurationRepository.Insert<SecurityRole>(adminRole);
        }
    }

}
