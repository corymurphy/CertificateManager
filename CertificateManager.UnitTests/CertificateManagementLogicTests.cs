using CertificateManager.Entities;
using CertificateManager.Entities.Exceptions;
using CertificateManager.Entities.Interfaces;
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
    public class CertificateManagementLogicTests
    {

        private IAuditLogic GetAuditMock()
        {
            Mock<IAuditLogic> audit = new Mock<IAuditLogic>();
            audit.Setup(x => x.LogSecurityAuditFailure(It.IsAny<ClaimsPrincipal>(), It.IsAny<ILoggableEntity>(), It.IsAny<EventCategory>()));
            audit.Setup(x => x.LogSecurityAuditSuccess(It.IsAny<ClaimsPrincipal>(), It.IsAny<ILoggableEntity>(), It.IsAny<EventCategory>()));
            return audit.Object;
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public void CertificateManagementLogic_GetCertificatePassword_Unauthorized_ThrowsUnauthorizedAccessException()
        {
            Mock<ICertificateRepository> certRepo = new Mock<ICertificateRepository>();
            certRepo.Setup(x => x.Get<Certificate>(It.IsAny<Guid>())).Returns(new Certificate());

            Mock<IAuthorizationLogic> authLogic = new Mock<IAuthorizationLogic>();
            authLogic.Setup(x => x.CanViewPrivateKey(It.IsAny<ICertificatePasswordEntity>(), It.IsAny<ClaimsPrincipal>())).Returns(false);

            CertificateManagementLogic certificateManagement = new CertificateManagementLogic(null, certRepo.Object, authLogic.Object, GetAuditMock(), null, null);

            certificateManagement.GetCertificatePassword(Guid.NewGuid(), null);
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public void CertificateManagementLogic_ResetCertificatePassword_Unauthorized_ThrowsUnauthorizedAccessException()
        {
            Mock<ICertificateRepository> certRepo = new Mock<ICertificateRepository>();
            certRepo.Setup(x => x.Get<Certificate>(It.IsAny<Guid>())).Returns(new Certificate());

            Mock<IAuthorizationLogic> authLogic = new Mock<IAuthorizationLogic>();
            authLogic.Setup(x => x.CanViewPrivateKey(It.IsAny<ICertificatePasswordEntity>(), It.IsAny<ClaimsPrincipal>())).Returns(false);

            CertificateManagementLogic certificateManagement = new CertificateManagementLogic(null, certRepo.Object, authLogic.Object, GetAuditMock(), null, null);

            certificateManagement.ResetCertificatePassword(Guid.NewGuid(), null);
        }


        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CertificateManagementLogic_ResetCertificatePassword_HashValidationFails_ThrowsInvalidOperationException()
        {
            Mock<IHashProvider> hashProvider = new Mock<IHashProvider>();
            hashProvider.Setup(x => x.ValidateData(It.IsAny<byte[]>(), It.IsAny<byte[]>())).Returns(false);

            Mock<ICertificateRepository> certRepo = new Mock<ICertificateRepository>();
            certRepo.Setup(x => x.Get<Certificate>(It.IsAny<Guid>())).Returns(new Certificate());

            Mock<IAuthorizationLogic> authLogic = new Mock<IAuthorizationLogic>();
            authLogic.Setup(x => x.CanViewPrivateKey(It.IsAny<ICertificatePasswordEntity>(), It.IsAny<ClaimsPrincipal>())).Returns(true);

            CertificateManagementLogic certificateManagement = new CertificateManagementLogic(null, certRepo.Object, authLogic.Object, GetAuditMock(), null, null, hashProvider.Object);

            certificateManagement.ResetCertificatePassword(Guid.NewGuid(), null);
        }


        [TestMethod]
        [ExpectedException(typeof(ObjectNotInCorrectStateException))]
        public void CertificateManagementLogic_ResetCertificatePassword_CertificateDoesNotHavePrivateKey_ThrowsObjectNotInCorrectStateException()
        {
            Mock<IHashProvider> hashProvider = new Mock<IHashProvider>();
            hashProvider.Setup(x => x.ValidateData(It.IsAny<byte[]>(), It.IsAny<byte[]>())).Returns(true);

            Certificate cert = new Certificate()
            {
                HasPrivateKey = false,
                CertificateStorageFormat = CertificateStorageFormat.Pfx
            };

            Mock<ICertificateRepository> certRepo = new Mock<ICertificateRepository>();
            certRepo.Setup(x => x.Get<Certificate>(It.IsAny<Guid>())).Returns(cert);

            Mock<IAuthorizationLogic> authLogic = new Mock<IAuthorizationLogic>();
            authLogic.Setup(x => x.CanViewPrivateKey(It.IsAny<ICertificatePasswordEntity>(), It.IsAny<ClaimsPrincipal>())).Returns(true);

            CertificateManagementLogic certificateManagement = new CertificateManagementLogic(null, certRepo.Object, authLogic.Object, GetAuditMock(), null, null, hashProvider.Object);

            certificateManagement.ResetCertificatePassword(Guid.NewGuid(), null);
        }



        [TestMethod]
        [ExpectedException(typeof(ObjectNotInCorrectStateException))]
        public void CertificateManagementLogic_ResetCertificatePassword_CertificateStorageFormatCert_ThrowsObjectNotInCorrectStateException()
        {
            Mock<IHashProvider> hashProvider = new Mock<IHashProvider>();
            hashProvider.Setup(x => x.ValidateData(It.IsAny<byte[]>(), It.IsAny<byte[]>())).Returns(true);

            Certificate cert = new Certificate()
            {
                HasPrivateKey = true,
                CertificateStorageFormat = CertificateStorageFormat.Cer
            };

            Mock<ICertificateRepository> certRepo = new Mock<ICertificateRepository>();
            certRepo.Setup(x => x.Get<Certificate>(It.IsAny<Guid>())).Returns(cert);

            Mock<IAuthorizationLogic> authLogic = new Mock<IAuthorizationLogic>();
            authLogic.Setup(x => x.CanViewPrivateKey(It.IsAny<ICertificatePasswordEntity>(), It.IsAny<ClaimsPrincipal>())).Returns(true);

            CertificateManagementLogic certificateManagement = new CertificateManagementLogic(null, certRepo.Object, authLogic.Object, GetAuditMock(), null, null, hashProvider.Object);

            certificateManagement.ResetCertificatePassword(Guid.NewGuid(), null);
        }
    }
}
