using CertificateManager.Entities;
using CertificateManager.Entities.Exceptions;
using CertificateManager.Logic;
using CertificateManager.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace CertificateManager.UnitTests
{
    [TestClass]
    public class UserManagementLogicTests
    {
        [TestMethod]
        [ExpectedException(typeof(ReferencedObjectDoesNotExistException))]
        public void UserManagementLogic_SetUser_InvalidUser_ThrowsReferencedObjectDoesNotExistException()
        {
            UpdateUserModel updateUserModel = new UpdateUserModel(){ Id = Guid.NewGuid() };
            

            Mock<IConfigurationRepository> configurationRepository = new Mock<IConfigurationRepository>();
            configurationRepository.Setup(x => x.GetAuthenticablePrincipal<AuthenticablePrincipal>(It.IsAny<Guid>())).Returns((AuthenticablePrincipal)null);


            UserManagementLogic userManagementLogic = new UserManagementLogic(configurationRepository.Object);


            userManagementLogic.SetUser(updateUserModel);

        }


        [TestMethod]
        [ExpectedException(typeof(InsufficientDataException))]
        public void UserManagementLogic_SetUser_UpdateModelHasNullUpn_ThrowsInsufficientDataException()
        {
            UpdateUserModel updateUserModel = new UpdateUserModel() { Id = Guid.NewGuid(), Name = null };
            AuthenticablePrincipal validUser = new AuthenticablePrincipal();

            Mock<IConfigurationRepository> configurationRepository = new Mock<IConfigurationRepository>();
            configurationRepository.Setup(x => x.GetAuthenticablePrincipal<AuthenticablePrincipal>(It.IsAny<Guid>())).Returns(validUser);

            UserManagementLogic userManagementLogic = new UserManagementLogic(configurationRepository.Object);

            userManagementLogic.SetUser(updateUserModel);

        }
    }
}
