using CertificateManager.Entities;
using CertificateManager.Entities.Exceptions;
using CertificateManager.Logic.ActiveDirectory;
using CertificateManager.Logic.ActiveDirectory.Interfaces;
using CertificateManager.Repository;
using CertificateServices;
using CertificateServices.ActiveDirectory.Entities;
using CertificateServices.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CertificateManager.Logic
{
    public class AdcsTemplateLogic
    {
        DataTransformation dataTransform;
        IConfigurationRepository configurationRepository;
        IActiveDirectoryRepository activeDirectory;


        public AdcsTemplateLogic(IConfigurationRepository configurationRepository, IActiveDirectoryRepository activeDirectory)
        {
            this.activeDirectory = activeDirectory;
            this.configurationRepository = configurationRepository;
            this.dataTransform = new DataTransformation();
        }

        public AdcsTemplate DiscoverTemplate(CipherAlgorithm cipher, WindowsApi api, KeyUsage keyUsage)
        {
            Expression<Func<AdcsTemplate, bool>> query = template => template.Cipher == cipher && template.WindowsApi == api && template.KeyUsage.ToString() == keyUsage.ToString();
            AdcsTemplate results = configurationRepository.Get<AdcsTemplate>(query).First();

            return results;
        }

        public bool ValidateTemplateWithRequest(CreatePrivateCertificateModel model, AdcsTemplate template)
        {
            if(model.KeySize < template.MinimumKeySize)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public AdcsTemplateGetModel AddTemplate(AdcsTemplate template)
        {
            IEnumerable<PrivateCertificateAuthorityConfig> ca = configurationRepository.GetAll<PrivateCertificateAuthorityConfig>();

            if (ca == null)
            {
                throw new PrivateCertificateAuthorityDoesNotExistException("A certificate authority must exist before adding a template");
            }

            this.ValidateTemplatePublishedActiveDirectory(template);

            List<AdcsCertificateTemplate> templatesFromAllRealms = this.GetTemplateInAllActiveDirectoryRealms(template.Name);

            template.Id = Guid.NewGuid();
            template.MinimumKeySize = templatesFromAllRealms.Select(x => x.MinimumKeySize).Max();
            configurationRepository.Insert<AdcsTemplate>(template);

            return this.GetTemplate(template.Id);
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

        public AdcsTemplateGetModel GetTemplate(Guid id)
        {
            AdcsTemplate template = configurationRepository.Get<AdcsTemplate>(id);

            return new AdcsTemplateGetModel()
            {
                Name = template.Name,
                Cipher = template.Cipher,
                KeyUsage = template.KeyUsage,
                WindowsApi = template.WindowsApi,
                Id = template.Id,
                RolesAllowedToIssueSelectView = GetSecrityRoleListView(template.RolesAllowedToIssue)
            };

        }

        public List<SecurityRoleSelectView> GetSecrityRoleListView(List<Guid> rolesAllowedToIssue) 
        {
            List<SecurityRoleSelectView> list = new List<SecurityRoleSelectView>();
            foreach (Guid roleId in rolesAllowedToIssue)
            {

                var role = configurationRepository.Get<SecurityRole>(roleId);

                list.Add(
                    new SecurityRoleSelectView()
                    {
                        Id = roleId,
                        Name = role.Name
                    });
            }


            return list;
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

                item.RolesAllowedToIssueSelectView = this.GetSecrityRoleListView(template.RolesAllowedToIssue);

                response.Add(item);
            }

            return response;
        }

        public void ValidateTemplatePublishedActiveDirectory(AdcsTemplate template)
        {
            IEnumerable<ActiveDirectoryMetadata> metadataList = configurationRepository.GetAll<ActiveDirectoryMetadata>();

            if(metadataList == null)
            {
                throw new AdcsTemplateValidationException("There are no active directory domains configured.");
            }

            foreach (ActiveDirectoryMetadata metadata in metadataList)
            {
                List<AdcsCertificateTemplate> templates = this.GetActiveDirectoryTemplates(metadata);

                if(!templates.Where(x => x.Name == template.Name).Any())
                {
                    throw new AdcsTemplateValidationException("Adcs template is not published in Active Directory");
                }

                if (templates.Where(x => x.Name == template.Name).Count() > 1)
                {
                    throw new AdcsTemplateValidationException("Search for Adcs templates by name in Active Directory returned more than one result, this is not allowed");
                }

                AdcsCertificateTemplate adTemplate = templates.Where(x => x.Name == template.Name).First();


                if(template.WindowsApi != adTemplate.WindowsApi)
                {
                    string msg = string.Format("Certificate Manager Template Windows API does not match the template in active directory. AD shows {0}, CertificateManager requested {1}", adTemplate.WindowsApi, template.WindowsApi);
                    throw new AdcsTemplateValidationException(msg);
                }

                if(template.Cipher != adTemplate.Cipher)
                {
                    string msg = string.Format("Certificate Manager Template cipher algorithm does not match the template in active directory. AD shows {0}, CertificateManager requested {1}", adTemplate.Cipher, template.Cipher);
                    throw new AdcsTemplateValidationException(msg);
                }
                if (!adTemplate.AllowsClientProvidedSubject())
                {
                    throw new AdcsTemplateValidationException("Adcs template was found in Active Directory, but the template does not allow the client to specify the subject");
                }

                if(adTemplate.RequiresStrongKeyProtection())
                {
                    throw new AdcsTemplateValidationException("Adcs template in Active Directory requires strong key protection. Certificate Manager inplements strong key protection that is incompatible with Active Directory Certificate Services. ");
                }

                if(adTemplate.PendAllRequests())
                {
                    throw new AdcsTemplateValidationException("Issuance requires pending the certificate for manager approval. This is not compatible with Certificate Manager");
                }

                if(adTemplate.RequireUserInteraction())
                {
                    throw new AdcsTemplateValidationException("Issuance requires user interaction. This is not compatible with Certificate Manager");
                }


            }


        }

        public List<AdcsCertificateTemplate> GetActiveDirectoryTemplates(ActiveDirectoryMetadata metadata)
        {
            List<AdcsCertificateTemplate> templates = activeDirectory.Search<AdcsCertificateTemplate>(NamingContext.Configuration, metadata);

            return templates;
        }

        public AdcsCertificateTemplate GetTemplateInAllActiveDirectoryRealms(string name, ActiveDirectoryMetadata metadata)
        {
            List<AdcsCertificateTemplate> templates = activeDirectory.Search<AdcsCertificateTemplate>("name", name, NamingContext.Configuration, metadata);

            if(!templates.Any())
            {
                throw new AdcsTemplateValidationException(string.Format("Adcs template is not published in Active Directory - {0}", metadata.Domain));
            }

            if(templates.Count > 1)
            {
                throw new AdcsTemplateValidationException(string.Format("More than one template in {0} matched the query. Please specify a unique template name. ", metadata.Domain));
            }

            return templates.First();
        }

        public List<AdcsCertificateTemplate> GetTemplateInAllActiveDirectoryRealms(string name)
        {
            IEnumerable<ActiveDirectoryMetadata> metadataList = configurationRepository.GetAll<ActiveDirectoryMetadata>();

            if (metadataList == null)
            {
                throw new AdcsTemplateValidationException("There are no active directory domains configured.");
            }

            List<AdcsCertificateTemplate> publishedTemplates = new List<AdcsCertificateTemplate>();

            foreach (ActiveDirectoryMetadata metadata in metadataList)
            {
                publishedTemplates.Add(this.GetTemplateInAllActiveDirectoryRealms(name, metadata));
            }

            return publishedTemplates;
        }

        public AdcsCertificateTemplate GetActiveDirectoryPublishedTemplate(string name, ActiveDirectoryMetadata metadata)
        {
            return activeDirectory.Search<AdcsCertificateTemplate>(NamingContext.Configuration, metadata)
                .Where(x => x.Name == name)
                .First();
        }
    }
}
