using CertificateManager.Logic.Interfaces;
using CertificateManager.Repository;

namespace CertificateManager.Logic
{
    public class CertificateManagementLogic
    {
        ICertificateRepository certificateRepository;
        IAuthorizationLogic authorizationLogic;
        LocalIdentityProviderLogic localIdentityProviderLogic;

        public CertificateManagementLogic(ICertificateRepository certificateRepository, IAuthorizationLogic authorizationLogic, LocalIdentityProviderLogic localIdentityProviderLogic)
        {
            this.certificateRepository = certificateRepository;
            this.authorizationLogic = authorizationLogic;
            this.localIdentityProviderLogic = localIdentityProviderLogic;
        }
    }
}
