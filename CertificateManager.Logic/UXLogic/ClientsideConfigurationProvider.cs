using CertificateManager.Entities;
using CertificateManager.Repository;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace CertificateManager.Logic.UXLogic
{
    public class ClientsideConfigurationProvider : IClientsideConfigurationProvider
    {
        IConfigurationRepository configurationRepository;
        public ClientsideConfigurationProvider(IConfigurationRepository configurationRepository)
        {
            this.configurationRepository = configurationRepository;
        }

        public string RenderSecurityRolesJsonArray()
        {
            IEnumerable<SecurityRole> roles = configurationRepository.GetAll<SecurityRole>();

            if (roles == null)
                return string.Empty;

            List<SecurityRoleSelectView> viewList = new List<SecurityRoleSelectView>();
            foreach(SecurityRole role in roles)
            {
                viewList.Add(new SecurityRoleSelectView()
                {
                    Id = role.Id,
                    Name = role.Name
                });
            }

            return string.Format("CmOptions.SecurityRoles = {0}", JsonConvert.SerializeObject(viewList));
        }

        public string RenderIdentitySourcesJsonArray()
        {
            IEnumerable<ActiveDirectoryMetadata> idps = configurationRepository.GetAll<ActiveDirectoryMetadata>();

            if (idps == null)
                return string.Empty;

            List<ActiveDirectoryMetadataConfigViewModel> editedIdps = new List<ActiveDirectoryMetadataConfigViewModel>();

            foreach(ActiveDirectoryMetadata idp in idps)
            {
                editedIdps.Add(new ActiveDirectoryMetadataConfigViewModel(idp));
            }

            return string.Format("CmOptions.ActiveDirectoryMetadatas = {0}", JsonConvert.SerializeObject(editedIdps));
        }

        public string RenderLocalAuthenticationStateJsonArray()
        {
            AppConfig appConfig = configurationRepository.GetAppConfig();

            return string.Format("CmOptions.LocalAuthenticationEnabled = {0}", appConfig.LocalLogonEnabled.ToString().ToLower());
        }

        public string RenderDynamicScript()
        {
            return string.Empty;   
        }

        public string RenderScopeMap()
        {
            AuthorizationLogic authorizationLogic = new AuthorizationLogic(configurationRepository, null);
            return string.Format("CmOptions.Scopes = {0}", JsonConvert.SerializeObject(authorizationLogic.GetAvailibleScopes()));
        }

        public string RenderIdentityTypes()
        {
            string[] identityTypes = new string[2] { "Role", "User" };

            return string.Format("CmOptions.IdentityTypes = {0}", JsonConvert.SerializeObject(identityTypes));

        }

        public string RenderAceTypes()
        {
            string[] aceTypes = new string[2] { "Allow", "Deny" };

            return string.Format("CmOptions.AceTypes = {0}", JsonConvert.SerializeObject(aceTypes));
        }
    }
}
