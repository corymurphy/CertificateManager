using CertificateManager.Logic.UXLogic;

namespace CertificateManager.Logic
{
    public class ClientsideConfigurationProviderInitialSetup : IClientsideConfigurationProvider
    {
        public string RenderDynamicScript()
        {
            return "document.location = '/initial-setup';";
        }

        public string RenderIdentitySourcesJsonArray()
        {
            return string.Empty;
        }

        public string RenderLocalAuthenticationStateJsonArray()
        {
            return string.Empty;
        }

        public string RenderSecurityRolesJsonArray()
        {
            return string.Empty;
        }
    }
}
