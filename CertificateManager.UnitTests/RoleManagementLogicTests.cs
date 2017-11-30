using CertificateManager.Entities;
using CertificateManager.Entities.Exceptions;
using CertificateManager.Logic;
using CertificateManager.Logic.Interfaces;
using CertificateManager.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace CertificateManager.UnitTests
{
    [TestClass]
    public class RoleManagementLogicTests
    {
        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public void RoleManagementLogic_SetRoleScopes_UnauthorizedUser_ThrowsUnauthorizedAccessException()
        {
            Mock<IAuthorizationLogic> authorizationLogic = new Mock<IAuthorizationLogic>();
            authorizationLogic.Setup(x => x.IsAuthorized(It.IsAny<Guid>(), It.IsAny<ClaimsPrincipal>())).Returns(false);

            RoleManagementLogic roleManagementLogic = new RoleManagementLogic(null, authorizationLogic.Object);

            roleManagementLogic.SetRoleScopes(new Guid(), null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ReferencedObjectDoesNotExistException))]
        public void RoleManagementLogic_SetRoleScopes_InvalidScopeProvided_ThrowsReferencedObjectDoesNotExistException()
        {
            List<Scope> validScopes = new List<Scope>() { new Scope("TestScope", AuthorizationScopes.ManageRolesScope) };

            Mock<IAuthorizationLogic> authorizationLogic = new Mock<IAuthorizationLogic>();
            authorizationLogic.Setup(x => x.IsAuthorized(It.IsAny<Guid>(), It.IsAny<ClaimsPrincipal>())).Returns(true);
            authorizationLogic.Setup(x => x.GetAvailibleScopes()).Returns(validScopes);


            SecurityRole role = new SecurityRole() { Name = "TestRole", Id = Guid.NewGuid() };

            Mock<IConfigurationRepository> configurationRepository = new Mock<IConfigurationRepository>();
            configurationRepository.Setup(x => x.GetSecurityRole(It.IsAny<Guid>())).Returns(role);

            RoleManagementLogic roleManagementLogic = new RoleManagementLogic(configurationRepository.Object, authorizationLogic.Object);

            List<Guid> invalidScopes = new List<Guid>() { Guid.NewGuid() };
            Guid user = new Guid();

            roleManagementLogic.SetRoleScopes(user, invalidScopes, null);
        }


    }
}
