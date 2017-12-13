using CertificateManager.Entities;
using CertificateManager.Entities.Exceptions;
using CertificateManager.Entities.Interfaces;
using CertificateManager.Entities.Models;
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
            SetRoleScopesModel model = new SetRoleScopesModel()
            {
                RoleId = new Guid()
            };

            Mock<IAuthorizationLogic> authorizationLogic = new Mock<IAuthorizationLogic>();
            authorizationLogic.Setup(x => x.IsAuthorizedThrowsException(AuthorizationScopes.ManageRoles, It.IsAny<ClaimsPrincipal>(), It.IsAny<ILoggableEntity>(), It.IsAny<EventCategory>())).Throws(new UnauthorizedAccessException());

            RoleManagementLogic roleManagementLogic = new RoleManagementLogic(null, authorizationLogic.Object);

            roleManagementLogic.SetRoleScopes(model, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ReferencedObjectDoesNotExistException))]
        public void RoleManagementLogic_SetRoleScopes_InvalidScopeProvided_ThrowsReferencedObjectDoesNotExistException()
        {
            List<Scope> validScopes = new List<Scope>() { new Scope("TestScope", AuthorizationScopes.ManageRoles) };

            Mock<IAuthorizationLogic> authorizationLogic = new Mock<IAuthorizationLogic>();
            authorizationLogic.Setup(x => x.IsAuthorized(It.IsAny<Guid>(), It.IsAny<ClaimsPrincipal>())).Returns(true);
            authorizationLogic.Setup(x => x.GetAvailibleScopes()).Returns(validScopes);

            SetRoleScopesModel model = new SetRoleScopesModel()
            {
                RoleId = new Guid(),
                Scopes = new List<Guid>() { Guid.NewGuid() }
            };

            SecurityRole role = new SecurityRole() { Name = "TestRole", Id = Guid.NewGuid() };

            Mock<IConfigurationRepository> configurationRepository = new Mock<IConfigurationRepository>();
            configurationRepository.Setup(x => x.Get<SecurityRole>(It.IsAny<Guid>())).Returns(role);

            RoleManagementLogic roleManagementLogic = new RoleManagementLogic(configurationRepository.Object, authorizationLogic.Object);

            roleManagementLogic.SetRoleScopes(model, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ReferencedObjectDoesNotExistException))]
        public void RoleManagementLogic_AddRoleMember_UserNotFound_ThrowsReferencedObjectDoesNotExistException()
        {
            AddSecurityRoleMemberModel model = new AddSecurityRoleMemberModel()
            {
                MemberId = Guid.NewGuid(),
                RoleId = Guid.NewGuid()
            };
            
            ClaimsPrincipal user = new ClaimsPrincipal();
            Mock<IConfigurationRepository> configurationRepository = new Mock<IConfigurationRepository>();
            configurationRepository.Setup(x => x.Get<AuthenticablePrincipal>(model.MemberId)).Returns((AuthenticablePrincipal)null);

            RoleManagementLogic roleManagementLogic = new RoleManagementLogic(configurationRepository.Object, new AuthorizeInitialSetup(configurationRepository.Object));

            roleManagementLogic.AddRoleMember(model, user);
        }

        [TestMethod]
        [ExpectedException(typeof(ReferencedObjectDoesNotExistException))]
        public void RoleManagementLogic_AddRoleMember_RoleNotFound_ThrowsReferencedObjectDoesNotExistException()
        {
            ClaimsPrincipal user = new ClaimsPrincipal();

            AddSecurityRoleMemberModel model = new AddSecurityRoleMemberModel()
            {
                MemberId = Guid.NewGuid(),
                RoleId = Guid.NewGuid()
            };

            Mock<IConfigurationRepository> configurationRepository = new Mock<IConfigurationRepository>();
            configurationRepository.Setup(x => x.Get<AuthenticablePrincipal>(model.MemberId)).Returns(new AuthenticablePrincipal());
            configurationRepository.Setup(x => x.Get<SecurityRole>(model.RoleId)).Returns((SecurityRole)null);

            RoleManagementLogic roleManagementLogic = new RoleManagementLogic(configurationRepository.Object, new AuthorizeInitialSetup(configurationRepository.Object));

            roleManagementLogic.AddRoleMember(model, user);
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public void RoleManagementLogic_AddRoleMember_Unauthorized_ThrowsUnauthorizedAccessException()
        {
            ClaimsPrincipal user = new ClaimsPrincipal();
            AddSecurityRoleMemberModel model = new AddSecurityRoleMemberModel()
            {
                MemberId = Guid.NewGuid(),
                RoleId = Guid.NewGuid()
            };
            
            Mock<IAuthorizationLogic> authorizationLogic = new Mock<IAuthorizationLogic>();
            authorizationLogic.Setup(x => x.IsAuthorizedThrowsException(AuthorizationScopes.ManageRoles, user, It.IsAny<ILoggableEntity>(), It.IsAny<EventCategory>())).Throws(new UnauthorizedAccessException());

            Mock<IConfigurationRepository> configurationRepository = new Mock<IConfigurationRepository>();
            configurationRepository.Setup(x => x.Get<AuthenticablePrincipal>(model.MemberId)).Returns(new AuthenticablePrincipal());
            configurationRepository.Setup(x => x.Get<SecurityRole>(model.RoleId)).Returns((SecurityRole)null);

            RoleManagementLogic roleManagementLogic = new RoleManagementLogic(configurationRepository.Object, authorizationLogic.Object);

            roleManagementLogic.AddRoleMember(model, user);
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public void RoleManagementLogic_DeleteRole_Unauthorized_ThrowsUnauthorizedAccessException()
        {
            SecurityRole securityRole = new SecurityRole();
            ClaimsPrincipal user = new ClaimsPrincipal();

            Mock<IAuthorizationLogic> authorizationLogic = new Mock<IAuthorizationLogic>();
            authorizationLogic.Setup(x => x.IsAuthorizedThrowsException(AuthorizationScopes.ManageRoles, user, It.IsAny<ILoggableEntity>(), It.IsAny<EventCategory>())).Throws(new UnauthorizedAccessException());

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
            authorizationLogic.Setup(x => x.IsAuthorizedThrowsException(AuthorizationScopes.ManageRoles, user, It.IsAny<ILoggableEntity>(), It.IsAny<EventCategory>()));

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
            authorizationLogic.Setup(x => x.IsAuthorizedThrowsException(AuthorizationScopes.ManageRoles, user, It.IsAny<ILoggableEntity>(), It.IsAny<EventCategory>())).Throws(new UnauthorizedAccessException());

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
            authorizationLogic.Setup(x => x.IsAuthorizedThrowsException(AuthorizationScopes.ManageRoles, user, It.IsAny<ILoggableEntity>(), It.IsAny<EventCategory>())).Throws(new UnauthorizedAccessException());

            Mock<IConfigurationRepository> configurationRepository = new Mock<IConfigurationRepository>();

            RoleManagementLogic roleManagementLogic = new RoleManagementLogic(configurationRepository.Object, authorizationLogic.Object);

            roleManagementLogic.UpdateRole(securityRole, user);
        }
    }
}
