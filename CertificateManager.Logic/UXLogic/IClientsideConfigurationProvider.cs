namespace CertificateManager.Logic.UXLogic
{
    public interface IClientsideConfigurationProvider
    {
        string RenderDynamicScript();

        string RenderIdentitySourcesJsonArray();

        string RenderLocalAuthenticationStateJsonArray();

        string RenderSecurityRolesJsonArray();

        string RenderScopeMap();

        string RenderIdentityTypes();

        string RenderAceTypes();

    }
}
