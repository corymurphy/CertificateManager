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

        public AuthenticablePrincipal GetUser(Guid id)
        {
            return configurationRepository.GetAuthenticablePrincipal<AuthenticablePrincipal>(id);
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

        public bool UserExists(Guid id)
        {
            try
            {
                AuthenticablePrincipal authenticablePrincipal = configurationRepository.GetAuthenticablePrincipal<AuthenticablePrincipal>(id);

                if (authenticablePrincipal != null)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
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
                    Name = user.SamAccountName
                });
            }
        }

        private void ImportMerge(ImportUsersExternalIdentitySourceModel entity)
        {
            if (entity.MergeWith == null)
                throw new MergeRequiresMergeTargetException("merge operation requires a valid userid to merge with");
            
            if (!this.UserExists(entity.MergeWith))
                throw new MergeRequiresMergeTargetException("merge operation requires a valid userid to merge with");

            AuthenticablePrincipal existingUser = configurationRepository.GetAuthenticablePrincipal<AuthenticablePrincipal>(entity.MergeWith);

            foreach (var user in entity.Users)
            {
                if (!configurationRepository.ExternalIdentitySourceExists(user.DomainId))
                    throw new ReferencedObjectDoesNotExistException("The authentication realm specified by the importing user does not exist");

                if (existingUser.AlternativeNames == null)
                    existingUser.AlternativeNames = new List<string>();

                if (string.IsNullOrWhiteSpace(user.SamAccountName) && string.IsNullOrWhiteSpace(user.UserPrincipalName))
                    throw new InsufficientDataException("UserPrincipalName or SamAccountName must be specified to import a user");

                if (!string.IsNullOrWhiteSpace(user.SamAccountName))
                    existingUser.AlternativeNames.Add(user.SamAccountName);

                if (!string.IsNullOrWhiteSpace(user.UserPrincipalName))
                    existingUser.AlternativeNames.Add(user.UserPrincipalName);
            }

            configurationRepository.UpdateAuthenticablePrincipal(existingUser);
        }

        public AddAuthenticablePrincipalEntity NewUser(AuthenticablePrincipal entity)
        {
            entity.Id = Guid.NewGuid();
            entity.PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password1@");
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

        public GetUserModel SetUser(UpdateUserModel entity)
        {
            if (!UserExists(entity.Id))
                throw new ReferencedObjectDoesNotExistException("User does not exist");


            if (string.IsNullOrWhiteSpace(entity.Name))
            {
                throw new InsufficientDataException("A user must have a userprincipalname");
            }
            else
            {
                if (configurationRepository.UserPrincipalNameExists(entity.Name, entity.Id))
                    throw new ReferencedObjectNotUniqueException("UserPrincipalName must be unique.");
            }

            List<string> altNames = new List<string>();


            if (entity.AlternativeNames != null)
            {
                altNames = entity.AlternativeNames.Distinct().ToList();
                foreach (string upn in altNames)
                {
                    if (configurationRepository.UserPrincipalNameExists(upn, entity.Id))
                        throw new ReferencedObjectNotUniqueException("UserPrincipalName must be unique.");
                }
            }

            AuthenticablePrincipal authenticablePrincipal = GetUser(entity.Id);
            authenticablePrincipal.Name = entity.Name;
            authenticablePrincipal.LocalLogonEnabled = entity.LocalLogonEnabled;
            authenticablePrincipal.Enabled = entity.Enabled;
            authenticablePrincipal.AlternativeNames = altNames;

            configurationRepository.UpdateAuthenticablePrincipal(authenticablePrincipal);

            return configurationRepository.GetAuthenticablePrincipal<GetUserModel>(authenticablePrincipal.Id);
        }

        public IEnumerable<GetUserModel> GetUsers()
        {
            return configurationRepository.GetAuthenticablePrincipals<GetUserModel>();
        }

        public IEnumerable<SearchAuthenticablePrincipalEntity> SearchUsers()
        {
            return configurationRepository.GetAuthenticablePrincipalsSearch();
        }
    }
}
