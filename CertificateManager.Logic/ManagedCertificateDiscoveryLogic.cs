using System;

namespace CertificateManager.Logic
{
    public class ManagedCertificateDiscoveryLogic
    {
        NodeLogic nodeLogic;
        ActiveDirectoryIdentityProviderLogic adIdpLogic;
        CertificateManagementLogic certificateManagementLogic;

        public ManagedCertificateDiscoveryLogic(NodeLogic nodeLogic, ActiveDirectoryIdentityProviderLogic adIdpLogic, CertificateManagementLogic certificateManagementLogic)
        {
            this.nodeLogic = nodeLogic;
            this.adIdpLogic = adIdpLogic;
            this.certificateManagementLogic = certificateManagementLogic;
        }

        public void InvokeDiscovery(Guid nodeId)
        {

        }
    }
}
