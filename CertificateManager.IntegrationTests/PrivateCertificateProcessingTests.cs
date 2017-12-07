using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CertificateManager.Repository;
using System.IO;
using CertificateServices;
using CertificateManager.Entities;
using CertificateManager.Logic;
using CertificateServices.Enumerations;
using System.Security.Cryptography.X509Certificates;
using Moq;
using System.Security.Claims;
using CertificateManager.Logic.Interfaces;
using CertificateManager.Entities.Enumerations;

namespace CertificateManager.IntegrationTests
{
    [TestClass]
    public class PrivateCertificateProcessingTests
    {
        private string caServerName = "srv14.cm.local";
        private string caCommonName = "CertificateAuthority";
        private string username = "Administrator";
        private string domain = "cm.local";
        private string password = "Password1@";

        LiteDbConfigurationRepository configDb;
        Win32CertificateProvider certProvider = new Win32CertificateProvider();
        LiteDbCertificateRepository certDb;
        X509Normalization x509Normalization = new X509Normalization();

        Mock<ClaimsPrincipal> user;
        RoleManagementLogic roleManagementLogic;

        private IAuthorizationLogic GetAuthorizationLogic_Allow()
        {
            Mock<IAuthorizationLogic> mock = new Mock<IAuthorizationLogic>();
            mock.Setup(x => x.IsAuthorized(It.IsAny<Guid>(), It.IsAny<ClaimsPrincipal>())).Returns(true);
            mock.Setup(x => x.IsAuthorized(It.IsAny<AdcsTemplate>(), It.IsAny<ClaimsPrincipal>())).Returns(true);
            return mock.Object;
        }

        private IAuthorizationLogic GetAuthorizationLogic_Deny()
        {
            Mock<IAuthorizationLogic> mock = new Mock<IAuthorizationLogic>();
            mock.Setup(x => x.IsAuthorized(It.IsAny<Guid>(), It.IsAny<ClaimsPrincipal>())).Returns(false);
            mock.Setup(x => x.IsAuthorized(It.IsAny<AdcsTemplate>(), It.IsAny<ClaimsPrincipal>())).Returns(false);
            return mock.Object;
        }

        [TestMethod]
        public void PrivateCertificateProcessing_CreateCertificate_CngRsa2048_ClientServerAuth_Success()
        {

            KeyUsage keyUsage = KeyUsage.ServerAuthentication | KeyUsage.ClientAuthentication;
            CreatePrivateCertificateModel model = new CreatePrivateCertificateModel()
            {
                CipherAlgorithm = CipherAlgorithm.RSA,
                KeyUsage = keyUsage.ToString(),
                HashAlgorithm = HashAlgorithm.SHA256,
                KeySize = 2048,
                Provider = WindowsApi.Cng,
                SubjectAlternativeNamesRaw = "integrationtestdomain.com,integrationtestdomain",
                SubjectCity = "Seattle",
                SubjectCommonName = "integrationtestdomain",
                SubjectCountry = "US",
                SubjectDepartment = "Engineering",
                SubjectState = "WA",
                SubjectOrganization = "IntegrationTestingCorp"
            };


            PrivateCertificateProcessing processor = new PrivateCertificateProcessing(certDb, configDb, certProvider, GetAuthorizationLogic_Allow(), user.Object);
            CreatePrivateCertificateResult result = processor.CreateCertificateWithPrivateKey(model);

            Assert.AreEqual(PrivateCertificateRequestStatus.Success, result.Status);
        }

        [TestMethod]
        public void PrivateCertificateProcessing_CreateCertificate_CngRsa2048_ClientServerAuth_ReturnedX509Certificate2HasClientServerAuthKeyUsage()
        {

            KeyUsage expected = KeyUsage.ServerAuthentication | KeyUsage.ClientAuthentication;

            CreatePrivateCertificateModel model = new CreatePrivateCertificateModel()
            {
                CipherAlgorithm = CipherAlgorithm.RSA,
                KeyUsage = expected.ToString(),
                HashAlgorithm = HashAlgorithm.SHA256,
                KeySize = 2048,
                Provider = WindowsApi.Cng,
                SubjectAlternativeNamesRaw = "integrationtestdomain.com,integrationtestdomain",
                SubjectCity = "Seattle",
                SubjectCommonName = "integrationtestdomain",
                SubjectCountry = "US",
                SubjectDepartment = "Engineering",
                SubjectState = "WA",
                SubjectOrganization = "IntegrationTestingCorp"
            };


            PrivateCertificateProcessing processor = new PrivateCertificateProcessing(certDb, configDb, certProvider, GetAuthorizationLogic_Allow(), user.Object);
            CreatePrivateCertificateResult result = processor.CreateCertificateWithPrivateKey(model);

            X509Certificate2 cert = new X509Certificate2(result.PfxByte, result.Password);

            KeyUsage actualKeyUsage = x509Normalization.GetKeyUsage(cert);

            Assert.AreEqual(expected, actualKeyUsage);
        }

        [TestMethod]
        public void PrivateCertificateProcessing_CreateCertificate_CngRsa2048_ServerAuth_Success()
        {
            CreatePrivateCertificateModel model = new CreatePrivateCertificateModel()
            {
                CipherAlgorithm = CipherAlgorithm.RSA,
                KeyUsage = KeyUsage.ServerAuthentication.ToString(),
                HashAlgorithm = HashAlgorithm.SHA256,
                KeySize = 2048,
                Provider = WindowsApi.Cng,
                SubjectAlternativeNamesRaw = "integrationtestdomain.com,integrationtestdomain",
                SubjectCity = "Seattle",
                SubjectCommonName = "integrationtestdomain",
                SubjectCountry = "US",
                SubjectDepartment = "Engineering",
                SubjectState = "WA",
                SubjectOrganization = "IntegrationTestingCorp"
            };


            PrivateCertificateProcessing processor = new PrivateCertificateProcessing(certDb, configDb, certProvider, GetAuthorizationLogic_Allow(), user.Object);
            CreatePrivateCertificateResult result = processor.CreateCertificateWithPrivateKey(model);

            Assert.AreEqual(PrivateCertificateRequestStatus.Success, result.Status);
        }

        [TestMethod]
        public void PrivateCertificateProcessing_CreateCertificate_CryptoApiRsa2048Sha256_ServerAuth_Success()
        {
            CreatePrivateCertificateModel model = new CreatePrivateCertificateModel()
            {
                CipherAlgorithm = CipherAlgorithm.RSA,
                KeyUsage = KeyUsage.ServerAuthentication.ToString(),
                HashAlgorithm = HashAlgorithm.SHA256,
                KeySize = 2048, 
                Provider = WindowsApi.CryptoApi,
                SubjectAlternativeNamesRaw = "integrationtestdomain.com,integrationtestdomain",
                SubjectCity = "Seattle", 
                SubjectCommonName = "integrationtestdomain",
                SubjectCountry = "US",
                SubjectDepartment = "Engineering", 
                SubjectState = "WA", 
                SubjectOrganization = "IntegrationTestingCorp"
            };


            PrivateCertificateProcessing processor = new PrivateCertificateProcessing(certDb, configDb, certProvider, GetAuthorizationLogic_Allow(), user.Object);
            CreatePrivateCertificateResult result = processor.CreateCertificateWithPrivateKey(model);

            Assert.AreEqual(PrivateCertificateRequestStatus.Success, result.Status);
        }

        [TestMethod]
        public void PrivateCertificateProcessing_CreateCertificate_CngEcdh256_ServerAuth_Success()
        {
            CreatePrivateCertificateModel model = new CreatePrivateCertificateModel()
            {
                CipherAlgorithm = CipherAlgorithm.ECDH,
                KeyUsage = KeyUsage.ServerAuthentication.ToString(),
                HashAlgorithm = HashAlgorithm.SHA256,
                KeySize = 256,
                Provider = WindowsApi.Cng,
                SubjectAlternativeNamesRaw = "integrationtestdomain.com,integrationtestdomain",
                SubjectCity = "Seattle",
                SubjectCommonName = "integrationtestdomain",
                SubjectCountry = "US",
                SubjectDepartment = "Engineering",
                SubjectState = "WA",
                SubjectOrganization = "IntegrationTestingCorp"
            };


            PrivateCertificateProcessing processor = new PrivateCertificateProcessing(certDb, configDb, certProvider, GetAuthorizationLogic_Allow(), user.Object);
            CreatePrivateCertificateResult result = processor.CreateCertificateWithPrivateKey(model);

            Assert.AreEqual(PrivateCertificateRequestStatus.Success, result.Status);
        }

        [TestMethod]
        public void PrivateCertificateProcessing_CreateCertificate_CngEcdsa256_ServerAuth_Success()
        {
            CreatePrivateCertificateModel model = new CreatePrivateCertificateModel()
            {
                CipherAlgorithm = CipherAlgorithm.ECDSA,
                KeyUsage = KeyUsage.ServerAuthentication.ToString(),
                HashAlgorithm = HashAlgorithm.SHA256,
                KeySize = 256,
                Provider = WindowsApi.Cng,
                SubjectAlternativeNamesRaw = "integrationtestdomain.com,integrationtestdomain",
                SubjectCity = "Seattle",
                SubjectCommonName = "integrationtestdomain",
                SubjectCountry = "US",
                SubjectDepartment = "Engineering",
                SubjectState = "WA",
                SubjectOrganization = "IntegrationTestingCorp"
            };


            PrivateCertificateProcessing processor = new PrivateCertificateProcessing(certDb, configDb, certProvider, GetAuthorizationLogic_Allow(), user.Object);
            CreatePrivateCertificateResult result = processor.CreateCertificateWithPrivateKey(model);

            Assert.AreEqual(PrivateCertificateRequestStatus.Success, result.Status);
        }


        [TestMethod]
        public void PrivateCertificateProcessing_CreateCertificate_CngEcdsa256_NoKeyUsage_ReturnsPrivateCertificateRequestStatusSuccess()
        {
            CreatePrivateCertificateModel model = new CreatePrivateCertificateModel()
            {
                CipherAlgorithm = CipherAlgorithm.RSA,
                KeyUsage = KeyUsage.None.ToString(),
                HashAlgorithm = HashAlgorithm.SHA256,
                KeySize = 2048,
                Provider = WindowsApi.CryptoApi,
                SubjectAlternativeNamesRaw = "integrationtestdomain.com,integrationtestdomain",
                SubjectCity = "Seattle",
                SubjectCommonName = "integrationtestdomain",
                SubjectCountry = "US",
                SubjectDepartment = "Engineering",
                SubjectState = "WA",
                SubjectOrganization = "IntegrationTestingCorp"
            };


            PrivateCertificateProcessing processor = new PrivateCertificateProcessing(certDb, configDb, certProvider, GetAuthorizationLogic_Allow(), user.Object);
            CreatePrivateCertificateResult result = processor.CreateCertificateWithPrivateKey(model);

            Assert.AreEqual(PrivateCertificateRequestStatus.Success, result.Status);
        }

        [TestMethod]
        public void PrivateCertificateProcessing_CreateCertificate_CngEcdsa256_NoKeyUsage_ReturnedX509Certificate2HasNoKeyUsage()
        {
            KeyUsage expected = KeyUsage.None;

            CreatePrivateCertificateModel model = new CreatePrivateCertificateModel()
            {
                CipherAlgorithm = CipherAlgorithm.RSA,
                KeyUsage = KeyUsage.None.ToString(),
                HashAlgorithm = HashAlgorithm.SHA256,
                KeySize = 2048,
                Provider = WindowsApi.CryptoApi,
                SubjectAlternativeNamesRaw = "integrationtestdomain.com,integrationtestdomain",
                SubjectCity = "Seattle",
                SubjectCommonName = "integrationtestdomain",
                SubjectCountry = "US",
                SubjectDepartment = "Engineering",
                SubjectState = "WA",
                SubjectOrganization = "IntegrationTestingCorp"
            };


            PrivateCertificateProcessing processor = new PrivateCertificateProcessing(certDb, configDb, certProvider, GetAuthorizationLogic_Allow(), user.Object);
            CreatePrivateCertificateResult result = processor.CreateCertificateWithPrivateKey(model);

            X509Certificate2 cert = new X509Certificate2(result.PfxByte, result.Password);

            KeyUsage actualKeyUsage = x509Normalization.GetKeyUsage(cert);

            Assert.AreEqual(expected, actualKeyUsage);
        }

        [TestInitialize]
        public void InitializeTest()
        {
            user = new Mock<ClaimsPrincipal>();

            string configPath = Path.GetTempFileName();
            configDb = new LiteDbConfigurationRepository(configPath);

            configDb.Insert<AdcsTemplate>(new AdcsTemplate()
            {
                WindowsApi = WindowsApi.CryptoApi,
                Cipher = CipherAlgorithm.RSA,
                //Hash = HashAlgorithm.SHA256,
                KeyUsage = KeyUsage.ServerAuthentication,
                Name = "ServerAuthentication-CapiRsa"
            });

            configDb.Insert<AdcsTemplate>(new AdcsTemplate()
            {
                WindowsApi = WindowsApi.CryptoApi,
                Cipher = CipherAlgorithm.RSA,
                //Hash = HashAlgorithm.SHA256,
                KeyUsage = KeyUsage.None,
                Name = "NoKeyUsage-CapiRsa"
            });

            configDb.Insert<AdcsTemplate>(new AdcsTemplate()
            {
                WindowsApi = WindowsApi.Cng,
                Cipher = CipherAlgorithm.RSA,
               // Hash = HashAlgorithm.SHA256,
                KeyUsage = KeyUsage.ServerAuthentication,
                Name = "ServerAuthentication-CngRsa"
            });

            configDb.Insert<AdcsTemplate>(new AdcsTemplate()
            {
                WindowsApi = WindowsApi.Cng,
                Cipher = CipherAlgorithm.RSA,
                //Hash = HashAlgorithm.SHA256,
                KeyUsage = KeyUsage.ServerAuthentication | KeyUsage.ClientAuthentication,
                Name = "ClientServerAuthentication-CngRsa"
            });

            configDb.Insert<AdcsTemplate>(new AdcsTemplate()
            {
                WindowsApi = WindowsApi.Cng,
                Cipher = CipherAlgorithm.ECDH,
                //Hash = HashAlgorithm.SHA256,
                KeyUsage = KeyUsage.ServerAuthentication,
                Name = "ServerAuthentication-CngEcdh"
            });

            configDb.Insert<AdcsTemplate>(new AdcsTemplate()
            {
                WindowsApi = WindowsApi.Cng,
                Cipher = CipherAlgorithm.ECDSA,
                //Hash = HashAlgorithm.SHA256,
                KeyUsage = KeyUsage.ServerAuthentication,
                Name = "ServerAuthentication-CngEcdsa"
            });

            Logic.SecretKeyProvider secretKeyProvider = new Logic.SecretKeyProvider();

            AppConfig appConfig = new AppConfig(){ EncryptionKey = secretKeyProvider.NewSecretBase64(32) };

            configDb.SetAppConfig(appConfig);


            ExternalIdentitySource identitySource = new ExternalIdentitySource()
            {
                Domain = "cm.local",
                Enabled = true,
                ExternalIdentitySourceType = ExternalIdentitySourceType.ActiveDirectoryBasic,
                Id = Guid.NewGuid(),
                Name = "cm.local",
                Password = password,
                Username = username,
                SearchBase = "DC=cm,DC=local"

            };


            PrivateCertificateAuthorityConfig caConfig = new PrivateCertificateAuthorityConfig()
            {
                CommonName = caCommonName,
                ServerName = caServerName,
                HashAlgorithm = HashAlgorithm.SHA256,
                Id = Guid.NewGuid(),
                IdentityProviderId = identitySource.Id
            };

            configDb.Insert<PrivateCertificateAuthorityConfig>(caConfig);

            configDb.Insert<ExternalIdentitySource>(identitySource);

            var config = configDb.GetAdcsTemplate(HashAlgorithm.SHA256, CipherAlgorithm.RSA, WindowsApi.Cng, KeyUsage.ClientAuthentication | KeyUsage.ServerAuthentication);

            string certDbPath = Path.GetTempFileName();
            certDb = new LiteDbCertificateRepository(certDbPath);
        }
    }
}
