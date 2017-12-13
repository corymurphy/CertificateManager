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
    public class UserManagementLogic
    {
        IConfigurationRepository configurationRepository;
        IAuthorizationLogic authorizationLogic;

        public UserManagementLogic(IConfigurationRepository configurationRepository, IAuthorizationLogic authorizationLogic)
        {
            this.configurationRepository = configurationRepository;
            this.authorizationLogic = authorizationLogic;
        }

        public void SetPassword(ResetUserPasswordViewModel model, ClaimsPrincipal ctx)
        {
            AuthenticablePrincipal principal = this.GetUser(model.Id);

            authorizationLogic.IsAuthorizedThrowsException(AuthorizationScopes.ManageUsers, ctx, principal, EventCategory.UserManagementResetPassword);

            principal.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);

            configurationRepository.Update<AuthenticablePrincipal>(principal);
        }

        public AuthenticablePrincipal GetUser(Guid id)
        {
            return configurationRepository.Get<AuthenticablePrincipal>(id);
        }

        public void ImportUser(ImportUsersActiveDirectoryMetadataModel entity, ClaimsPrincipal user)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            authorizationLogic.IsAuthorizedThrowsException(AuthorizationScopes.ManageUsers, user, entity, EventCategory.UserManagementImport);
               
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
                AuthenticablePrincipal authenticablePrincipal = configurationRepository.Get<AuthenticablePrincipal>(id);

                if (authenticablePrincipal != null)
                    return true;
                else
                    return false;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        private void ValidateImportEntity(ImportUsersActiveDirectoryMetadataModel entity)
        {
            foreach (var user in entity.Users)
            {
                if (!string.IsNullOrWhiteSpace(user.SamAccountName))
                    if (configurationRepository.Exists<AuthenticablePrincipal>(x => x.Name == user.SamAccountName))
                        throw new ReferencedObjectAlreadyExistsException("The user selected to be imported already exists");

                if (!string.IsNullOrWhiteSpace(user.UserPrincipalName))
                    if (configurationRepository.Exists<AuthenticablePrincipal>(x => x.Name == user.UserPrincipalName))
                        throw new ReferencedObjectAlreadyExistsException("The user selected to be imported already exists");

            }
        }

        private void ImportWithoutMerge(ImportUsersActiveDirectoryMetadataModel entity)
        {
            foreach (var user in entity.Users)
            {
                if (!configurationRepository.Exists<ActiveDirectoryMetadata>(user.DomainId))
                    throw new ReferencedObjectDoesNotExistException("The authentication realm specified by the importing user does not exist");

                configurationRepository.Insert<AuthenticablePrincipal>(new AuthenticablePrincipal()
                {
                    Enabled = true,
                    Id = Guid.NewGuid(),
                    Name = user.SamAccountName
                });
            }
        }

        private void ImportMerge(ImportUsersActiveDirectoryMetadataModel entity)
        {
            if (entity.MergeWith == null)
                throw new MergeRequiresMergeTargetException("merge operation requires a valid userid to merge with");
            
            if (!this.UserExists(entity.MergeWith.Value))
                throw new MergeRequiresMergeTargetException("merge operation requires a valid userid to merge with");

            AuthenticablePrincipal existingUser = configurationRepository.Get<AuthenticablePrincipal>(entity.MergeWith.Value);

            foreach (var user in entity.Users)
            {
                if (!configurationRepository.Exists<ActiveDirectoryMetadata>(user.DomainId))
                    throw new ReferencedObjectDoesNotExistException("The authentication realm specified by the importing user does not exist");

                if (existingUser.AlternativeNames == null)
                    existingUser.AlternativeNames = new List<string>();

                if (string.IsNullOrWhiteSpace(user.SamAccountName) || string.IsNullOrWhiteSpace(user.UserPrincipalName))
                    throw new InsufficientDataException("UserPrincipalName or SamAccountName must be specified to import a user");

                if (!string.IsNullOrWhiteSpace(user.SamAccountName))
                    existingUser.AlternativeNames.Add(user.SamAccountName);

                if (!string.IsNullOrWhiteSpace(user.UserPrincipalName))
                    existingUser.AlternativeNames.Add(user.UserPrincipalName);
            }

            configurationRepository.Update<AuthenticablePrincipal>(existingUser);
        }

        public AddAuthenticablePrincipalEntity NewUser(AuthenticablePrincipal entity, ClaimsPrincipal user)
        {
            authorizationLogic.IsAuthorizedThrowsException(AuthorizationScopes.ManageUsers, user, entity, EventCategory.UserManagementNew);

            entity.Id = Guid.NewGuid();
            entity.PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password1@");
            configurationRepository.Insert<AuthenticablePrincipal>(entity);
            return new AddAuthenticablePrincipalEntity(entity);
        }

        public void DeleteUser(AuthenticablePrincipal entity, ClaimsPrincipal user)
        {
            authorizationLogic.IsAuthorizedThrowsException(AuthorizationScopes.ManageUsers, user, entity, EventCategory.UserManagementDelete);

            configurationRepository.Delete<AuthenticablePrincipal>(entity.Id);

            IEnumerable<SecurityRole> memberOf = configurationRepository.GetAuthenticablePrincipalMemberOf(entity.Id);

            if (memberOf != null)
            {
                foreach (var role in memberOf)
                {
                    role.Member = role.Member.Where(member => member != entity.Id).ToList();
                    configurationRepository.Delete<SecurityRole>(role.Id);
                    configurationRepository.Insert<SecurityRole>(role);
                }
            }
        }

        public GetUserModel SetUser(UpdateUserModel entity, ClaimsPrincipal user)
        {
            authorizationLogic.IsAuthorizedThrowsException(AuthorizationScopes.ManageUsers, user, entity, EventCategory.UserManagementSet);

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

            configurationRepository.Update<AuthenticablePrincipal>(authenticablePrincipal);

            return configurationRepository.Get<GetUserModel>(authenticablePrincipal.Id);
        }

        public IEnumerable<GetUserModel> GetUsers()
        {
            return configurationRepository.GetAll<GetUserModel>();
        }

        public IEnumerable<SearchAuthenticablePrincipalEntity> SearchUsers()
        {
            return configurationRepository.GetAll<SearchAuthenticablePrincipalEntity>();
        }
    }
}
