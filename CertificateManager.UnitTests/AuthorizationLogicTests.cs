
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CertificateManager.Logic;
using CertificateManager.Entities;
using CertificateManager.Entities.Enumerations;
using System.Security.Claims;
using System;
using System.Collections.Generic;
using Moq;
using CertificateManager.Repository;

namespace CertificateManager.UnitTests
{
    [TestClass]
    public class AuthorizationLogicTests
    {
        private const string roleId = "318254c2-588b-47e5-ab65-ce6318851f77";
        private const string denyRoleId = "ace80b92-c8f2-4471-81c6-6961137bf98a";
        private const string allowRoleId = "318254c2-588b-47e5-ab65-ce6318851f77";
        private const string name = "unittester@certificatemanager.local";
        private const string uid = "8e129ef3-b485-4b9e-b5a3-d877491ce099";
        private const string nameClaim = "http://certificatemanager/upn";
        private const string roleClaim = "http://certificatemanager/role";
        private const string altNameClaim = "http://certificatemanager/alternative-upn";
        private const string uidClaim = "http://certificatemanager/uid";

        private ClaimsPrincipal GetClaimsPrincipalWithNoRoles()
        {
            ClaimsIdentity id = new ClaimsIdentity("UnitTestingAuthority", nameClaim, roleClaim);
            id.AddClaim(new Claim(nameClaim, name));
            id.AddClaim(new Claim(uidClaim, uid));
            ClaimsPrincipal principal = new ClaimsPrincipal(id);

            return principal;
        }

        private ClaimsPrincipal GetClaimsPrincipalWithRole()
        {
            ClaimsIdentity id = new ClaimsIdentity("UnitTestingAuthority", nameClaim, roleClaim);
            id.AddClaim(new Claim(nameClaim, name));
            id.AddClaim(new Claim(uidClaim, uid));
            id.AddClaim(new Claim(roleClaim, roleId));

            ClaimsPrincipal principal = new ClaimsPrincipal(id);

            return principal;
        }

        private Certificate GetCertificateWithDenyRoleClaimExpiredAce()
        {

            AccessControlEntry ace = new AccessControlEntry()
            {
                AceType = AceType.Deny,
                Id = Guid.NewGuid(),
                Expires = DateTime.Now.AddYears(-1),
                Identity = allowRoleId,
                IdentityType = IdentityType.Role
            };

            List<AccessControlEntry> acl = new List<AccessControlEntry>();
            acl.Add(ace);

            return new Certificate()
            {
                Acl = acl
            };

        }

        private Certificate GetCertificateWithAllowRoleClaimExpiredAce()
        {

            AccessControlEntry ace = new AccessControlEntry()
            {
                AceType = AceType.Allow,
                Id = Guid.NewGuid(),
                Expires = DateTime.Now.AddYears(-1),
                Identity = allowRoleId,
                IdentityType = IdentityType.Role
            };

            List<AccessControlEntry> acl = new List<AccessControlEntry>();
            acl.Add(ace);

            return new Certificate()
            {
                Acl = acl
            };

        }

        private Certificate GetCertificateWithAllowRoleClaimNoExpiry()
        {

            AccessControlEntry ace = new AccessControlEntry()
            {
                AceType = AceType.Allow,
                Id = Guid.NewGuid(),
                Expires = DateTime.MaxValue,
                Identity = allowRoleId,
                IdentityType = IdentityType.Role
            };

            List<AccessControlEntry> acl = new List<AccessControlEntry>();
            acl.Add(ace);

            return new Certificate()
            {
                Acl = acl
            };

        }

        private Certificate GetCertificateWithAllowUserPrincipalClaimNoExpiry()
        {

            AccessControlEntry ace = new AccessControlEntry()
            {
                AceType = AceType.Allow,
                Id = Guid.NewGuid(),
                Expires = DateTime.MaxValue,
                Identity = uid,
                IdentityType = IdentityType.User
            };

            List<AccessControlEntry> acl = new List<AccessControlEntry>();
            acl.Add(ace);

            return new Certificate()
            {
                Acl = acl
            };

        }

        private Certificate GetCertificateWithDenyRoleClaimNoExpiry()
        {

            AccessControlEntry ace = new AccessControlEntry()
            {
                AceType = AceType.Deny,
                Id = Guid.NewGuid(),
                Expires = DateTime.MaxValue,
                Identity = allowRoleId,
                IdentityType = IdentityType.Role
            };

            List<AccessControlEntry> acl = new List<AccessControlEntry>();
            acl.Add(ace);

            return new Certificate()
            {
                Acl = acl
            };

        }









        [TestMethod]
        public void AuthorizationLogic_CanViewPrivateKey_NullClaimPrincipal_ReturnsFalse()
        {
            AuthorizationLogic authorizationLogic = new AuthorizationLogic(null);

            Certificate certificate = new Certificate();
            ClaimsPrincipal claimsPrincipal = null;

            bool isAuthorized = authorizationLogic.CanViewPrivateKey(certificate, claimsPrincipal);

            Assert.IsFalse(isAuthorized);
        }

        [TestMethod]
        public void AuthorizationLogic_CanViewPrivateKey_NullCertificate_ReturnsFalse()
        {
            AuthorizationLogic authorizationLogic = new AuthorizationLogic(null);

            Certificate certificate = null;
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal();

            bool isAuthorized = authorizationLogic.CanViewPrivateKey(certificate, claimsPrincipal);

            Assert.IsFalse(isAuthorized);
        }

        [TestMethod]
        public void AuthorizationLogic_CanViewPrivateKey_CertificateWithNoAcl_ReturnsFalse()
        {
            AuthorizationLogic authorizationLogic = new AuthorizationLogic(null);
            
            Certificate certificate = new Certificate();
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal();

            bool isAuthorized = authorizationLogic.CanViewPrivateKey(certificate, claimsPrincipal);

            Assert.IsFalse(isAuthorized);
        }

        [TestMethod]
        public void AuthorizationLogic_CanViewPrivateKey_CertificateWithDenyRoleAce_ReturnsFalse()
        {
            AuthorizationLogic authorizationLogic = new AuthorizationLogic(null);

            Certificate certificate = GetCertificateWithDenyRoleClaimNoExpiry();
            ClaimsPrincipal claimsPrincipal = GetClaimsPrincipalWithRole();

            bool isAuthorized = authorizationLogic.CanViewPrivateKey(certificate, claimsPrincipal);

            Assert.IsFalse(isAuthorized);
        }

        [TestMethod]
        public void AuthorizationLogic_CanViewPrivateKey_CertificateWithAllowRoleAce_ReturnsTrue()
        {
            AuthorizationLogic authorizationLogic = new AuthorizationLogic(null);

            Certificate certificate = GetCertificateWithAllowRoleClaimNoExpiry();
            ClaimsPrincipal claimsPrincipal = GetClaimsPrincipalWithRole();

            bool isAuthorized = authorizationLogic.CanViewPrivateKey(certificate, claimsPrincipal);

            Assert.IsTrue(isAuthorized);
        }

        [TestMethod]
        public void AuthorizationLogic_CanViewPrivateKey_CertificateWithAllowUserPrincipalAce_ReturnsTrue()
        {
            AuthorizationLogic authorizationLogic = new AuthorizationLogic(null);

            Certificate certificate = GetCertificateWithAllowUserPrincipalClaimNoExpiry();
            ClaimsPrincipal claimsPrincipal = GetClaimsPrincipalWithRole();

            bool isAuthorized = authorizationLogic.CanViewPrivateKey(certificate, claimsPrincipal);

            Assert.IsTrue(isAuthorized);
        }

        [TestMethod]
        public void AuthorizationLogic_CanViewPrivateKey_CertificateWithAllowRole_DateExpiredAce_ReturnsFalse()
        {
            AuthorizationLogic authorizationLogic = new AuthorizationLogic(null);

            Certificate certificate = GetCertificateWithAllowRoleClaimExpiredAce();
            ClaimsPrincipal claimsPrincipal = GetClaimsPrincipalWithRole();

            bool isAuthorized = authorizationLogic.CanViewPrivateKey(certificate, claimsPrincipal);

            Assert.IsFalse(isAuthorized);
        }

        [TestMethod]
        public void AuthorizationLogic_CanViewPrivateKey_CertificateWithDenyRole_DateExpiredAce_ReturnsFalse()
        {
            AuthorizationLogic authorizationLogic = new AuthorizationLogic(null);

            Certificate certificate = GetCertificateWithDenyRoleClaimExpiredAce();
            ClaimsPrincipal claimsPrincipal = GetClaimsPrincipalWithRole();

            bool isAuthorized = authorizationLogic.CanViewPrivateKey(certificate, claimsPrincipal);

            Assert.IsFalse(isAuthorized);
        }

        [TestMethod]
        public void AuthorizationLogic_IsAuthorized_ClaimsPrincipalWithNoRoles_ReturnsFalse()
        {
            AuthorizationLogic authorizationLogic = new AuthorizationLogic(null);
            ClaimsPrincipal user = GetClaimsPrincipalWithNoRoles();

            bool isAuthorized = authorizationLogic.IsAuthorized(AuthorizationScopes.ManageRoles, user);

            Assert.IsFalse(isAuthorized);
        }

        [TestMethod]
        public void AuthorizationLogic_IsAuthorized_ClaimsPrincipalWithRoleNullScopes_ReturnsFalse()
        {
            SecurityRole role = new SecurityRole() { Name = "TestRole", Scopes = null };

            Mock<IConfigurationRepository> configurationRepository = new Mock<IConfigurationRepository>();
            configurationRepository.Setup(x => x.GetSecurityRole(It.IsAny<Guid>())).Returns(role);

            AuthorizationLogic authorizationLogic = new AuthorizationLogic(configurationRepository.Object);
            ClaimsPrincipal user = GetClaimsPrincipalWithNoRoles();

            bool isAuthorized = authorizationLogic.IsAuthorized(AuthorizationScopes.ManageRoles, user);

            Assert.IsFalse(isAuthorized);
        }

        [TestMethod]
        public void AuthorizationLogic_IsAuthorized_ClaimsPrincipalWithRoleEmptyScopes_ReturnsFalse()
        {
            SecurityRole role = new SecurityRole() { Name = "TestRole", Scopes = new List<Guid>() };

            Mock<IConfigurationRepository> configurationRepository = new Mock<IConfigurationRepository>();
            configurationRepository.Setup(x => x.GetSecurityRole(It.IsAny<Guid>())).Returns(role);

            AuthorizationLogic authorizationLogic = new AuthorizationLogic(configurationRepository.Object);
            ClaimsPrincipal user = GetClaimsPrincipalWithNoRoles();

            bool isAuthorized = authorizationLogic.IsAuthorized(AuthorizationScopes.ManageRoles, user);

            Assert.IsFalse(isAuthorized);
        }

        [TestMethod]
        public void AuthorizationLogic_IsAuthorized_ClaimsPrincipalWithRoleScopeIncluded_ReturnsTrue()
        {
            SecurityRole role = new SecurityRole() { Name = "TestRole", Id = new Guid(roleId), Scopes = new List<Guid>() { AuthorizationScopes.ManageRoles } };

            Mock<IConfigurationRepository> configurationRepository = new Mock<IConfigurationRepository>();
            configurationRepository.Setup(x => x.GetSecurityRole(It.IsAny<Guid>())).Returns(role);

            AuthorizationLogic authorizationLogic = new AuthorizationLogic(configurationRepository.Object);
            ClaimsPrincipal user = GetClaimsPrincipalWithRole();

            bool isAuthorized = authorizationLogic.IsAuthorized(AuthorizationScopes.ManageRoles, user);

            Assert.IsTrue(isAuthorized);
        }
    }
}
