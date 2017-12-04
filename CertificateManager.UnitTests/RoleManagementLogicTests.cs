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
            List<Scope> validScopes = new List<Scope>() { new Scope("TestScope", AuthorizationScopes.ManageRoles) };

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

        [TestMethod]
        [ExpectedException(typeof(ReferencedObjectDoesNotExistException))]
        public void RoleManagementLogic_AddRoleMember_UserNotFound_ThrowsReferencedObjectDoesNotExistException()
        {
            Guid memberId = Guid.NewGuid();
            Guid roleId = Guid.NewGuid();
            ClaimsPrincipal user = new ClaimsPrincipal();
            Mock<IConfigurationRepository> configurationRepository = new Mock<IConfigurationRepository>();
            configurationRepository.Setup(x => x.GetAuthenticablePrincipal<AuthenticablePrincipal>(memberId)).Returns((AuthenticablePrincipal)null);

            RoleManagementLogic roleManagementLogic = new RoleManagementLogic(configurationRepository.Object, new AuthorizeInitialSetup(configurationRepository.Object));

            roleManagementLogic.AddRoleMember(roleId, memberId, user);
        }

        [TestMethod]
        [ExpectedException(typeof(ReferencedObjectDoesNotExistException))]
        public void RoleManagementLogic_AddRoleMember_RoleNotFound_ThrowsReferencedObjectDoesNotExistException()
        {
            Guid memberId = Guid.NewGuid();
            Guid roleId = Guid.NewGuid();
            ClaimsPrincipal user = new ClaimsPrincipal();

            Mock<IConfigurationRepository> configurationRepository = new Mock<IConfigurationRepository>();
            configurationRepository.Setup(x => x.GetAuthenticablePrincipal<AuthenticablePrincipal>(memberId)).Returns(new AuthenticablePrincipal());
            configurationRepository.Setup(x => x.GetSecurityRole(roleId)).Returns((SecurityRole)null);

            RoleManagementLogic roleManagementLogic = new RoleManagementLogic(configurationRepository.Object, new AuthorizeInitialSetup(configurationRepository.Object));

            roleManagementLogic.AddRoleMember(roleId, memberId, user);
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public void RoleManagementLogic_AddRoleMember_Unauthorized_ThrowsUnauthorizedAccessException()
        {
            Guid memberId = Guid.NewGuid();
            Guid roleId = Guid.NewGuid();
            ClaimsPrincipal user = new ClaimsPrincipal();

            Mock<IAuthorizationLogic> authorizationLogic = new Mock<IAuthorizationLogic>();
            authorizationLogic.Setup(x => x.IsAuthorized(AuthorizationScopes.ManageRoles, user)).Returns(false);

            Mock<IConfigurationRepository> configurationRepository = new Mock<IConfigurationRepository>();
            configurationRepository.Setup(x => x.GetAuthenticablePrincipal<AuthenticablePrincipal>(memberId)).Returns(new AuthenticablePrincipal());
            configurationRepository.Setup(x => x.GetSecurityRole(roleId)).Returns((SecurityRole)null);

            RoleManagementLogic roleManagementLogic = new RoleManagementLogic(configurationRepository.Object, authorizationLogic.Object);

            roleManagementLogic.AddRoleMember(roleId, memberId, user);
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public void RoleManagementLogic_DeleteRole_Unauthorized_ThrowsUnauthorizedAccessException()
        {
            SecurityRole securityRole = new SecurityRole();
            ClaimsPrincipal user = new ClaimsPrincipal();

            Mock<IAuthorizationLogic> authorizationLogic = new Mock<IAuthorizationLogic>();
            authorizationLogic.Setup(x => x.AuthorizedToManageRoles(user)).Returns(false);

            Mock<IConfigurationRepository> configurationRepository = new Mock<IConfigurationRepository>();

            RoleManagementLogic roleManagementLogic = new RoleManagementLogic(configurationRepository.Object, authorizationLogic.Object);

            roleManagementLogic.DeleteRole(securityRole, user);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RoleManagementLogic_DeleteRole_DeleteWellKnownAdministratorId_ThrowsInvalidOperationException()
        {
            SecurityRole securityRole = new SecurityRole()
            {
                Id = RoleManagementLogic.WellKnownAdministratorRoleId
            };

            ClaimsPrincipal user = new ClaimsPrincipal();

            Mock<IAuthorizationLogic> authorizationLogic = new Mock<IAuthorizationLogic>();
            authorizationLogic.Setup(x => x.AuthorizedToManageRoles(user)).Returns(true);

            Mock<IConfigurationRepository> configurationRepository = new Mock<IConfigurationRepository>();

            RoleManagementLogic roleManagementLogic = new RoleManagementLogic(configurationRepository.Object, authorizationLogic.Object);

            roleManagementLogic.DeleteRole(securityRole, user);
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public void RoleManagementLogic_AddRole_Unauthorized_ThrowsUnauthorizedAccessException()
        {
            SecurityRole securityRole = new SecurityRole();
            ClaimsPrincipal user = new ClaimsPrincipal();

            Mock<IAuthorizationLogic> authorizationLogic = new Mock<IAuthorizationLogic>();
            authorizationLogic.Setup(x => x.IsAuthorized(AuthorizationScopes.ManageRoles, user)).Returns(false);

            Mock<IConfigurationRepository> configurationRepository = new Mock<IConfigurationRepository>();

            RoleManagementLogic roleManagementLogic = new RoleManagementLogic(configurationRepository.Object, authorizationLogic.Object);

            roleManagementLogic.AddRole(securityRole, user);
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public void RoleManagementLogic_UpdateRole_Unauthorized_ThrowsUnauthorizedAccessException()
        {
            SecurityRole securityRole = new SecurityRole();
            ClaimsPrincipal user = new ClaimsPrincipal();

            Mock<IAuthorizationLogic> authorizationLogic = new Mock<IAuthorizationLogic>();
            authorizationLogic.Setup(x => x.IsAuthorized(AuthorizationScopes.ManageRoles, user)).Returns(false);

            Mock<IConfigurationRepository> configurationRepository = new Mock<IConfigurationRepository>();

            RoleManagementLogic roleManagementLogic = new RoleManagementLogic(configurationRepository.Object, authorizationLogic.Object);

            roleManagementLogic.UpdateRole(securityRole, user);
        }
    }
}
