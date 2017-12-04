using CertificateManager.Entities;
using CertificateManager.Entities.Exceptions;
using CertificateManager.Logic;
using CertificateManager.Logic.Interfaces;
using CertificateManager.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Security.Claims;

namespace CertificateManager.UnitTests
{
    [TestClass]
    public class UserManagementLogicTests
    {
        private ImportUsersExternalIdentitySourceModel GetImportUsersExternalIdentitySourceModelValid()
        {
            return new ImportUsersExternalIdentitySourceModel()
            {
                MergeWith = Guid.NewGuid(),
                Merge = true,
                Users = new System.Collections.Generic.List<ExternalIdentitySourceAuthPrincipalQueryResultModel>()
                {
                    new ExternalIdentitySourceAuthPrincipalQueryResultModel()
                    {
                        SamAccountName = "TestAccountName",
                        UserPrincipalName = "TestUpn"
                    }
                }
            };
        }

        private AuthenticablePrincipal GetAuthenticablePrincipalValid()
        {
            return new AuthenticablePrincipal()
            {
                Enabled = true,
                Name = "TestUser", 
                Id = Guid.NewGuid(),
                LocalLogonEnabled = true
            };
            
        }

        private UpdateUserModel GetUpdateUserModelValid()
        {
            return new UpdateUserModel()
            {
                Enabled = true,
                Id = Guid.NewGuid(),
                Name = "TestUser"
            };
        }

        private AddAuthenticablePrincipalEntity GetAddAuthenticablePrincipalEntityValid()
        {
            return new AddAuthenticablePrincipalEntity()
            {
                Name = "TestUser",
                Id = Guid.NewGuid()
            };
        }

        [TestMethod]
        [ExpectedException(typeof(ReferencedObjectDoesNotExistException))]
        public void UserManagementLogic_SetUser_InvalidUser_ThrowsReferencedObjectDoesNotExistException()
        {
            ClaimsPrincipal user = new ClaimsPrincipal();
            UpdateUserModel updateUserModel = new UpdateUserModel(){ Id = Guid.NewGuid() };
            
            Mock<IConfigurationRepository> configurationRepository = new Mock<IConfigurationRepository>();
            configurationRepository.Setup(x => x.GetAuthenticablePrincipal<AuthenticablePrincipal>(It.IsAny<Guid>())).Returns((AuthenticablePrincipal)null);

            UserManagementLogic userManagementLogic = new UserManagementLogic(configurationRepository.Object, new AuthorizeInitialSetup(configurationRepository.Object));

            userManagementLogic.SetUser(updateUserModel, user);
        }

        [TestMethod]
        [ExpectedException(typeof(InsufficientDataException))]
        public void UserManagementLogic_SetUser_UpdateModelHasNullUpn_ThrowsInsufficientDataException()
        {
            ClaimsPrincipal user = new ClaimsPrincipal();
            UpdateUserModel updateUserModel = new UpdateUserModel() { Id = Guid.NewGuid(), Name = null };
            AuthenticablePrincipal validUser = new AuthenticablePrincipal();

            Mock<IConfigurationRepository> configurationRepository = new Mock<IConfigurationRepository>();
            configurationRepository.Setup(x => x.GetAuthenticablePrincipal<AuthenticablePrincipal>(It.IsAny<Guid>())).Returns(validUser);

            UserManagementLogic userManagementLogic = new UserManagementLogic(configurationRepository.Object, new AuthorizeInitialSetup(configurationRepository.Object));

            userManagementLogic.SetUser(updateUserModel, user);
        }

        [TestMethod]
        public void UserManagementLogic_UserExists_RepositoryThrowsException_ReturnsFalse()
        {
            Mock<IConfigurationRepository> configurationRepository = new Mock<IConfigurationRepository>();
            configurationRepository.Setup(x => x.GetAuthenticablePrincipal<AuthenticablePrincipal>(It.IsAny<Guid>())).Throws(new Exception());

            UserManagementLogic userManagementLogic = new UserManagementLogic(configurationRepository.Object, new AuthorizeInitialSetup(configurationRepository.Object));

            bool result = userManagementLogic.UserExists(Guid.NewGuid());

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void UserManagementLogic_UserExists_RepositoryReturnsNull_ReturnsFalse()
        {
            Mock<IConfigurationRepository> configurationRepository = new Mock<IConfigurationRepository>();
            configurationRepository.Setup(x => x.GetAuthenticablePrincipal<AuthenticablePrincipal>(It.IsAny<Guid>())).Returns((AuthenticablePrincipal)null);

            UserManagementLogic userManagementLogic = new UserManagementLogic(configurationRepository.Object, new AuthorizeInitialSetup(configurationRepository.Object));

            bool result = userManagementLogic.UserExists(Guid.NewGuid());

            Assert.IsFalse(result);
        }

        [TestMethod]
        [ExpectedException(typeof(MergeRequiresMergeTargetException))]
        public void UserManagementLogic_ImportMerge_EntityWithNullMergeWith_ThrowsMergeRequiresMergeTargetException()
        {
            ClaimsPrincipal user = new ClaimsPrincipal();
            ImportUsersExternalIdentitySourceModel entity = new ImportUsersExternalIdentitySourceModel()
            {
                MergeWith = (Guid?)null,
                Merge = true,
                Users = new System.Collections.Generic.List<ExternalIdentitySourceAuthPrincipalQueryResultModel>()
                {
                    new ExternalIdentitySourceAuthPrincipalQueryResultModel()
                    {
                        SamAccountName = "TestAccountName",
                        UserPrincipalName = "TestUpn"
                    }
                    
                }
            };

            Mock<IConfigurationRepository> configurationRepository = new Mock<IConfigurationRepository>();
            configurationRepository.Setup(x => x.UserPrincipalNameExists(It.IsAny<string>())).Returns(false);

            UserManagementLogic userManagementLogic = new UserManagementLogic(configurationRepository.Object, new AuthorizeInitialSetup(configurationRepository.Object));

            userManagementLogic.ImportUser(entity, user);
        }

        [TestMethod]
        [ExpectedException(typeof(MergeRequiresMergeTargetException))]
        public void UserManagementLogic_ImportMerge_EntityWithExistingSamAccountName_ThrowsMergeRequiresMergeTargetException()
        {
            ClaimsPrincipal user = new ClaimsPrincipal();
            ImportUsersExternalIdentitySourceModel entity = new ImportUsersExternalIdentitySourceModel()
            {
                MergeWith = Guid.NewGuid(),
                Merge = true,
                Users = new System.Collections.Generic.List<ExternalIdentitySourceAuthPrincipalQueryResultModel>()
                {
                    new ExternalIdentitySourceAuthPrincipalQueryResultModel()
                    {
                        SamAccountName = "TestAccountName",
                        UserPrincipalName = "TestUpn"
                    }
                }
            };

            Mock<IConfigurationRepository> configurationRepository = new Mock<IConfigurationRepository>();
            configurationRepository.Setup(x => x.UserPrincipalNameExists(It.IsAny<string>())).Returns(false);
            configurationRepository.Setup(x => x.GetAuthenticablePrincipal<AuthenticablePrincipal>(It.IsAny<Guid>())).Returns((AuthenticablePrincipal)null);

            UserManagementLogic userManagementLogic = new UserManagementLogic(configurationRepository.Object, new AuthorizeInitialSetup(configurationRepository.Object));

            userManagementLogic.ImportUser(entity, user);
        }

        [TestMethod]
        [ExpectedException(typeof(ReferencedObjectDoesNotExistException))]
        public void UserManagementLogic_ImportMerge_EntityWithInvalidAuthenticationRealm_ThrowsReferencedObjectDoesNotExistException()
        {
            ClaimsPrincipal user = new ClaimsPrincipal();
            ImportUsersExternalIdentitySourceModel entity = new ImportUsersExternalIdentitySourceModel()
            {
                MergeWith = Guid.NewGuid(),
                Merge = true,
                Users = new System.Collections.Generic.List<ExternalIdentitySourceAuthPrincipalQueryResultModel>()
                {
                    new ExternalIdentitySourceAuthPrincipalQueryResultModel()
                    {
                        SamAccountName = "TestAccountName",
                        UserPrincipalName = "TestUpn"
                    }
                }
            };

            Mock<IConfigurationRepository> configurationRepository = new Mock<IConfigurationRepository>();
            configurationRepository.Setup(x => x.UserPrincipalNameExists(It.IsAny<string>())).Returns(false);
            configurationRepository.Setup(x => x.GetAuthenticablePrincipal<AuthenticablePrincipal>(It.IsAny<Guid>())).Returns(new AuthenticablePrincipal());

            UserManagementLogic userManagementLogic = new UserManagementLogic(configurationRepository.Object, new AuthorizeInitialSetup(configurationRepository.Object));

            userManagementLogic.ImportUser(entity, user);
        }
       
        [TestMethod]
        [ExpectedException(typeof(InsufficientDataException))]
        public void UserManagementLogic_ImportMerge_EntityWithEmptyUserPrincipalName_ThrowsInsufficientDataException()
        {
            ClaimsPrincipal user = new ClaimsPrincipal();
            ImportUsersExternalIdentitySourceModel entity = new ImportUsersExternalIdentitySourceModel()
            {
                MergeWith = Guid.NewGuid(),
                Merge = true,
                Users = new System.Collections.Generic.List<ExternalIdentitySourceAuthPrincipalQueryResultModel>()
                {
                    new ExternalIdentitySourceAuthPrincipalQueryResultModel()
                    {
                        SamAccountName = "TestAccountName",
                        UserPrincipalName = string.Empty
                    }
                }
            };

            Mock<IConfigurationRepository> configurationRepository = new Mock<IConfigurationRepository>();
            configurationRepository.Setup(x => x.UserPrincipalNameExists(It.IsAny<string>())).Returns(false);
            configurationRepository.Setup(x => x.GetAuthenticablePrincipal<AuthenticablePrincipal>(It.IsAny<Guid>())).Returns(new AuthenticablePrincipal());
            configurationRepository.Setup(x => x.ExternalIdentitySourceExists(It.IsAny<Guid>())).Returns(true);

            UserManagementLogic userManagementLogic = new UserManagementLogic(configurationRepository.Object, new AuthorizeInitialSetup(configurationRepository.Object));

            userManagementLogic.ImportUser(entity, user);
        }

        [TestMethod]
        [ExpectedException(typeof(InsufficientDataException))]
        public void UserManagementLogic_ImportMerge_EntityWithEmptySamAccountName_ThrowsInsufficientDataException()
        {
            ClaimsPrincipal user = new ClaimsPrincipal();
            ImportUsersExternalIdentitySourceModel entity = new ImportUsersExternalIdentitySourceModel()
            {
                MergeWith = Guid.NewGuid(),
                Merge = true,
                Users = new System.Collections.Generic.List<ExternalIdentitySourceAuthPrincipalQueryResultModel>()
                {
                    new ExternalIdentitySourceAuthPrincipalQueryResultModel()
                    {
                        SamAccountName = string.Empty,
                        UserPrincipalName = "TestUpn"
                    }
                }
            };

            Mock<IConfigurationRepository> configurationRepository = new Mock<IConfigurationRepository>();
            configurationRepository.Setup(x => x.UserPrincipalNameExists(It.IsAny<string>())).Returns(false);
            configurationRepository.Setup(x => x.GetAuthenticablePrincipal<AuthenticablePrincipal>(It.IsAny<Guid>())).Returns(new AuthenticablePrincipal());
            configurationRepository.Setup(x => x.ExternalIdentitySourceExists(It.IsAny<Guid>())).Returns(true);

            UserManagementLogic userManagementLogic = new UserManagementLogic(configurationRepository.Object, new AuthorizeInitialSetup(configurationRepository.Object));

            userManagementLogic.ImportUser(entity, user);
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public void UserManagementLogic_ImportUser_Unauthorized_ThrowsUnauthorizedAccessException()
        {
            ClaimsPrincipal user = new ClaimsPrincipal();
            ImportUsersExternalIdentitySourceModel entity = GetImportUsersExternalIdentitySourceModelValid();

            Mock<IAuthorizationLogic> authorizationLogic = new Mock<IAuthorizationLogic>();
            authorizationLogic.Setup(x => x.IsAuthorizedThrowsException(AuthorizationScopes.ManageUsers, user)).Throws(new UnauthorizedAccessException());

            Mock<IConfigurationRepository> configurationRepository = new Mock<IConfigurationRepository>();

            UserManagementLogic userManagementLogic = new UserManagementLogic(configurationRepository.Object, authorizationLogic.Object);

            userManagementLogic.ImportUser(entity, user);
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public void UserManagementLogic_DeleteUser_Unauthorized_ThrowsUnauthorizedAccessException()
        {
            ClaimsPrincipal user = new ClaimsPrincipal();
            AuthenticablePrincipal authenticablePrincipal = GetAuthenticablePrincipalValid();

            Mock<IAuthorizationLogic> authorizationLogic = new Mock<IAuthorizationLogic>();
            authorizationLogic.Setup(x => x.IsAuthorizedThrowsException(AuthorizationScopes.ManageUsers, user)).Throws(new UnauthorizedAccessException());

            Mock<IConfigurationRepository> configurationRepository = new Mock<IConfigurationRepository>();

            UserManagementLogic userManagementLogic = new UserManagementLogic(configurationRepository.Object, authorizationLogic.Object);

            userManagementLogic.DeleteUser(authenticablePrincipal, user);
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public void UserManagementLogic_SetUser_Unauthorized_ThrowsUnauthorizedAccessException()
        {
            ClaimsPrincipal user = new ClaimsPrincipal();
            UpdateUserModel authenticablePrincipal = GetUpdateUserModelValid();

            Mock<IAuthorizationLogic> authorizationLogic = new Mock<IAuthorizationLogic>();
            authorizationLogic.Setup(x => x.IsAuthorizedThrowsException(AuthorizationScopes.ManageUsers, user)).Throws(new UnauthorizedAccessException());

            Mock<IConfigurationRepository> configurationRepository = new Mock<IConfigurationRepository>();

            UserManagementLogic userManagementLogic = new UserManagementLogic(configurationRepository.Object, authorizationLogic.Object);

            userManagementLogic.SetUser(authenticablePrincipal, user);
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public void UserManagementLogic_NewUser_Unauthorized_ThrowsUnauthorizedAccessException()
        {
            ClaimsPrincipal user = new ClaimsPrincipal();
            AuthenticablePrincipal newUser = GetAuthenticablePrincipalValid();

            Mock<IAuthorizationLogic> authorizationLogic = new Mock<IAuthorizationLogic>();
            authorizationLogic.Setup(x => x.IsAuthorizedThrowsException(AuthorizationScopes.ManageUsers, user)).Throws(new UnauthorizedAccessException());

            Mock<IConfigurationRepository> configurationRepository = new Mock<IConfigurationRepository>();

            UserManagementLogic userManagementLogic = new UserManagementLogic(configurationRepository.Object, authorizationLogic.Object);

            userManagementLogic.NewUser(newUser, user);
        }
    }
}
