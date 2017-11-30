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
            IEnumerable<SecurityRole> roles = configurationRepository.GetSecurityRoles();

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
            IEnumerable<ExternalIdentitySource> idps = configurationRepository.GetExternalIdentitySources();

            if (idps == null)
                return string.Empty;

            List<ExternalIdentitySourceConfigViewModel> editedIdps = new List<ExternalIdentitySourceConfigViewModel>();

            foreach(ExternalIdentitySource idp in idps)
            {
                editedIdps.Add(new ExternalIdentitySourceConfigViewModel(idp));
            }

            return string.Format("CmOptions.ExternalIdentitySources = {0}", JsonConvert.SerializeObject(editedIdps));
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
            AuthorizationLogic authorizationLogic = new AuthorizationLogic(configurationRepository);
            return string.Format("CmOptions.Scopes = {0}", JsonConvert.SerializeObject(authorizationLogic.GetAvailibleScopes()));
        }
    }
}
