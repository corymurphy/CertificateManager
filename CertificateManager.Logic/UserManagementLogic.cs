using CertificateManager.Entities;
using CertificateManager.Entities.Exceptions;
using CertificateManager.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CertificateManager.Logic
{
    public class UserManagementLogic
    {
        IConfigurationRepository configurationRepository;

        public UserManagementLogic(IConfigurationRepository configurationRepository)
        {
            this.configurationRepository = configurationRepository;
        }

        public void ImportUser(ImportUsersExternalIdentitySourceModel entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            ValidateImportEntity(entity);

            if (entity.Merge)
                ImportMerge(entity);
            else
                ImportWithoutMerge(entity);

        }

        private void ValidateImportEntity(ImportUsersExternalIdentitySourceModel entity)
        {
            foreach (var user in entity.Users)
            {
                if (!string.IsNullOrWhiteSpace(user.SamAccountName))
                    if (configurationRepository.UserPrincipalNameExists(user.SamAccountName))
                        throw new ReferencedObjectAlreadyExistsException("The user selected to be imported already exists");

                if (!string.IsNullOrWhiteSpace(user.UserPrincipalName))
                    if (configurationRepository.UserPrincipalNameExists(user.UserPrincipalName))
                        throw new ReferencedObjectAlreadyExistsException("The user selected to be imported already exists");

            }
        }

        private void ImportWithoutMerge(ImportUsersExternalIdentitySourceModel entity)
        {
            foreach (var user in entity.Users)
            {
                if (!configurationRepository.ExternalIdentitySourceExists(user.DomainId))
                    throw new ReferencedObjectDoesNotExistException("The authentication realm specified by the importing user does not exist");

                configurationRepository.InsertAuthenticablePrincipal(new AuthenticablePrincipal()
                {
                    Enabled = true,
                    Id = Guid.NewGuid(),
                    UserPrincipalName = user.SamAccountName
                });
            }
        }

        private void ImportMerge(ImportUsersExternalIdentitySourceModel entity)
        {
            if (entity.MergeWith == null)
                throw new MergeRequiresMergeTargetException("merge operation requires a valid userid to merge with");
            
            if (!configurationRepository.AuthenticablePrincipalExists(entity.MergeWith))
                throw new MergeRequiresMergeTargetException("merge operation requires a valid userid to merge with");

            AuthenticablePrincipal existingUser = configurationRepository.GetAuthenticablePrincipal(entity.MergeWith);

            foreach (var user in entity.Users)
            {
                if (!configurationRepository.ExternalIdentitySourceExists(user.DomainId))
                    throw new ReferencedObjectDoesNotExistException("The authentication realm specified by the importing user does not exist");

                if (existingUser.AlternativeUserPrincipalNames == null)
                    existingUser.AlternativeUserPrincipalNames = new List<string>();

                if (string.IsNullOrWhiteSpace(user.SamAccountName) && string.IsNullOrWhiteSpace(user.UserPrincipalName))
                    throw new InsufficientDataException("UserPrincipalName or SamAccountName must be specified to import a user");

                if (!string.IsNullOrWhiteSpace(user.SamAccountName))
                    existingUser.AlternativeUserPrincipalNames.Add(user.SamAccountName);

                if (!string.IsNullOrWhiteSpace(user.UserPrincipalName))
                    existingUser.AlternativeUserPrincipalNames.Add(user.UserPrincipalName);
            }

            configurationRepository.UpdateAuthenticablePrincipal(existingUser);
        }

        public AddAuthenticablePrincipalEntity NewUser(AuthenticablePrincipal entity)
        {
            entity.Id = Guid.NewGuid();
            configurationRepository.InsertAuthenticablePrincipal(entity);
            return new AddAuthenticablePrincipalEntity(entity);
        }

        public void DeleteUser(AuthenticablePrincipal entity)
        {
            configurationRepository.DeleteAuthenticablePrincipal(entity);

            IEnumerable<SecurityRole> memberOf = configurationRepository.GetAuthenticablePrincipalMemberOf(entity.Id);

            if (memberOf != null)
            {
                foreach (var role in memberOf)
                {
                    role.Member = role.Member.Where(member => member != entity.Id).ToList();
                    configurationRepository.DeleteSecurityRole(role);
                    configurationRepository.InsertSecurityRole(role);
                }
            }
        }

        public void SetUser(AuthenticablePrincipal entity)
        {
            if (string.IsNullOrWhiteSpace(entity.UserPrincipalName))
            {
                throw new InsufficientDataException("A user must have a userprincipalname");
            }
            else
            {
                if (configurationRepository.UserPrincipalNameExists(entity.UserPrincipalName, entity.Id))
                    throw new ReferencedObjectNotUniqueException("UserPrincipalName must be unique.");
            }
            
            if (entity.AlternativeUserPrincipalNames != null)
            {
                foreach (string upn in entity.AlternativeUserPrincipalNames)
                {
                    if (configurationRepository.UserPrincipalNameExists(upn, entity.Id))
                        throw new ReferencedObjectNotUniqueException("UserPrincipalName must be unique.");
                }
            }

            configurationRepository.UpdateAuthenticablePrincipal(entity);
        }

        public IEnumerable<AuthenticablePrincipal> GetUsers()
        {
            return configurationRepository.GetAuthenticablePrincipals();
        }

        public IEnumerable<SearchAuthenticablePrincipalEntity> SearchUsers()
        {
            return configurationRepository.GetAuthenticablePrincipalsSearch();
        }
    }
}
