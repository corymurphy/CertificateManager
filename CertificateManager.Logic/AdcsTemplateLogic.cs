﻿using CertificateManager.Entities;
using CertificateManager.Entities.Exceptions;
using CertificateManager.Repository;
using CertificateServices;
using CertificateServices.Enumerations;
using System;
using System.Collections.Generic;

namespace CertificateManager.Logic
{
    public class AdcsTemplateLogic
    {
        DataTransformation dataTransform;
        IConfigurationRepository configurationRepository;


        public AdcsTemplateLogic(IConfigurationRepository configurationRepository)
        {
            this.configurationRepository = configurationRepository;
            this.dataTransform = new DataTransformation();
        }


        public AdcsTemplate AddTemplate(AdcsTemplate template)
        {
            IEnumerable<PrivateCertificateAuthorityConfig> ca = configurationRepository.GetAll<PrivateCertificateAuthorityConfig>();

            if (ca == null)
                throw new PrivateCertificateAuthorityDoesNotExistException("A certificate authority must exist before adding a template");

            template.Id = Guid.NewGuid();
            configurationRepository.Insert<AdcsTemplate>(template);
            //runtimeConfigurationState.ClearAlert(AlertType.NoTemplatesConfigured);
            return template;
        }

        public AdcsTemplate AddTemplate(string name, CipherAlgorithm cipher, KeyUsage keyUsage, WindowsApi api, List<Guid> rolesAllowedToIssue)
        {
            AdcsTemplate template = new AdcsTemplate()
            {
                Name = name, 
                Cipher = cipher,
                KeyUsage = keyUsage,
                WindowsApi = api,
                RolesAllowedToIssue = rolesAllowedToIssue
            };

            configurationRepository.Insert<AdcsTemplate>(template);

            return template;
        }

        public AdcsTemplateGetModel UpdateTemplate(AdcsTemplateUpdateModel updateEntity)
        {
            if (updateEntity.Name == "error")
                throw new PrivateCertificateAuthorityDoesNotExistException("A certificate authority must exist before adding a template");

            AdcsTemplate template = configurationRepository.Get<AdcsTemplate>(updateEntity.Id);

            template.Name = updateEntity.Name;
            template.RolesAllowedToIssue = dataTransform.ParseGuidList(updateEntity.RolesAllowedToIssue);
            template.KeyUsage = updateEntity.KeyUsage;
            template.WindowsApi = updateEntity.WindowsApi;
            template.Cipher = updateEntity.Cipher;

            configurationRepository.Update<AdcsTemplate>(template);


            AdcsTemplateGetModel response = new AdcsTemplateGetModel();

            response.Name = template.Name;
            response.Cipher = template.Cipher;
            response.KeyUsage = template.KeyUsage;
            response.WindowsApi = template.WindowsApi;
            response.Id = template.Id;

            response.RolesAllowedToIssueSelectView = new List<SecurityRoleSelectView>();
            foreach (Guid roleId in template.RolesAllowedToIssue)
            {
                var role = configurationRepository.Get<SecurityRole>(roleId);

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
            configurationRepository.Delete<AdcsTemplate>(id);
        }

        public List<AdcsTemplateGetModel> GetTemplates()
        {
            IEnumerable<AdcsTemplate> result = configurationRepository.GetAll<AdcsTemplate>();
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

                    var role = configurationRepository.Get<SecurityRole>(roleId);

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
