using CertificateManager.Repository;

namespace CertificateManager.Logic
{
    public class AuthorizationLogic
    {
        private IConfigurationRepository configurationRepository;

        public AuthorizationLogic(IConfigurationRepository configurationRepository)
        {
            this.configurationRepository = configurationRepository;
        }

        public bool AuthorizedIssueCertificate()
        {
            return false;
        }

    }
}
