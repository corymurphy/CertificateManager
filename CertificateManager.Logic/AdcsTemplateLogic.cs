using CertificateManager.Entities;
using CertificateManager.Entities.Enumerations;
using CertificateManager.Entities.Exceptions;
using CertificateManager.Repository;
using System;
using System.Collections.Generic;

namespace CertificateManager.Logic
{
    public class AdcsTemplateLogic
    {
        DataTransformation dataTransform;
        IConfigurationRepository configurationRepository;
        IRuntimeConfigurationState runtimeConfigurationState;

        public AdcsTemplateLogic(IConfigurationRepository configurationRepository, IRuntimeConfigurationState runtimeConfigurationState)
        {
            this.configurationRepository = configurationRepository;
            this.runtimeConfigurationState = runtimeConfigurationState;
            this.dataTransform = new DataTransformation();
        }

        public AdcsTemplate AddTemplate(AdcsTemplate template)
        {
            IEnumerable<PrivateCertificateAuthorityConfig> ca = configurationRepository.GetPrivateCertificateAuthorities();

            if (ca == null)
                throw new PrivateCertificateAuthorityDoesNotExistException("A certificate authority must exist before adding a template");

            template.Id = Guid.NewGuid();
            configurationRepository.InsertAdcsTemplate(template);
            runtimeConfigurationState.ClearAlert(AlertType.NoTemplatesConfigured);
            return template;
        }

        public AdcsTemplateGetModel UpdateTemplate(AdcsTemplateUpdateModel updateEntity)
        {
            if (updateEntity.Name == "error")
                throw new PrivateCertificateAuthorityDoesNotExistException("A certificate authority must exist before adding a template");

            AdcsTemplate template = configurationRepository.GetAdcsTemplate(updateEntity.Id);

            template.Name = updateEntity.Name;
            template.RolesAllowedToIssue = dataTransform.ParseGuidList(updateEntity.RolesAllowedToIssue);
            //template.Hash = updateEntity.Hash;
            template.KeyUsage = updateEntity.KeyUsage;
            template.WindowsApi = updateEntity.WindowsApi;
            template.Cipher = updateEntity.Cipher;

            configurationRepository.UpdateAdcsTemplate(template);


            AdcsTemplateGetModel response = new AdcsTemplateGetModel();

            response.Name = template.Name;
            //response.Hash = template.Hash;
            response.Cipher = template.Cipher;
            response.KeyUsage = template.KeyUsage;
            response.WindowsApi = template.WindowsApi;
            response.Id = template.Id;

            response.RolesAllowedToIssueSelectView = new List<SecurityRoleSelectView>();
            foreach (Guid roleId in template.RolesAllowedToIssue)
            {
                var role = configurationRepository.GetSecurityRole(roleId);

                response.RolesAllowedToIssueSelectView.Add(
                    new SecurityRoleSelectView()
                    {
                        Id = roleId,
                        Name = role.Name
                    });
            }

            return response;
        }

        public void DeleteTemplate(Guid id)
        {
            configurationRepository.DeleteAdcsTemplates(id);
        }

        public List<AdcsTemplateGetModel> GetTemplates()
        {
            IEnumerable<AdcsTemplate> result = configurationRepository.GetAdcsTemplates();
            List<AdcsTemplateGetModel> response = new List<AdcsTemplateGetModel>();

            foreach (var template in result)
            {
                AdcsTemplateGetModel item = new AdcsTemplateGetModel();
                item.Name = template.Name;
                item.Cipher = template.Cipher;
                item.KeyUsage = template.KeyUsage;
                item.WindowsApi = template.WindowsApi;
                item.Id = template.Id;

                item.RolesAllowedToIssueSelectView = new List<SecurityRoleSelectView>();
                foreach (Guid roleId in template.RolesAllowedToIssue)
                {

                    var role = configurationRepository.GetSecurityRole(roleId);

                    item.RolesAllowedToIssueSelectView.Add(
                        new SecurityRoleSelectView()
                        {
                            Id = roleId,
                            Name = role.Name
                        });
                }

                response.Add(item);
            }

            return response;
        }
    }
}
