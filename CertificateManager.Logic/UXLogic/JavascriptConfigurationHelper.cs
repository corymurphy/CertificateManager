using CertificateManager.Entities;
using CertificateManager.Repository;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace CertificateManager.Logic.UXLogic
{
    public class JavascriptConfigurationHelper
    {
        IConfigurationRepository configurationRepository;
        public JavascriptConfigurationHelper(IConfigurationRepository configurationRepository)
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
    }
}
