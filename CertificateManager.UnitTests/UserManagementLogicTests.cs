using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CertificateManager.Repository;
using Moq;

namespace CertificateManager.UnitTests
{
    [TestClass]
    public class UserManagementLogicTests
    {
        [TestMethod]
        public void UserManagementLogic_SetUser_InvalidUser_ThrowsReferencedObjectDoesNotExistException()
        {
            Mock<IConfigurationRepository> configurationRepository = new Mock<IConfigurationRepository>();
        }
    }
}
