using CertificateManager.Entities;
using CertificateManager.Entities.Exceptions;
using CertificateManager.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CertificateManager.Logic
{
    public class RoleManagementLogic
    {
        IConfigurationRepository configurationRepository;

        public RoleManagementLogic(IConfigurationRepository configurationRepository)
        {
            this.configurationRepository = configurationRepository;
        }

        public SecurityRole GetRole(Guid id)
        {
            return configurationRepository.GetSecurityRole(id);
        }

        public IEnumerable<SecurityRole> GetRoles()
        {
            return configurationRepository.GetSecurityRoles();
        }

        public SecurityRole AddRole(SecurityRole entity)
        {
            entity.Id = Guid.NewGuid();
            configurationRepository.InsertSecurityRole(entity);
            return entity;
        }

        public void DeleteRole(SecurityRole securityRole)
        {
            configurationRepository.DeleteSecurityRole(securityRole);
        }

        public void UpdateRole(SecurityRole securityRole)
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
                members.Add(configurationRepository.GetAuthenticablePrincipal(memberId));
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
            AuthenticablePrincipal principal = configurationRepository.GetAuthenticablePrincipal(memberId);

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
    }

}
