using CertificateManager.Entities;
using CertificateManager.Entities.Enumerations;
using CertificateManager.Logic;
using CertificateManager.Repository;
using CertificateServices;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace CertificateManager.Controllers
{
    public class PkiConfigurationAdcsTemplateController : Controller
    {
        IConfigurationRepository configurationRepository;
        IRuntimeConfigurationState runtimeConfigurationState;
        HttpResponseHandler http;
        DataTransformation dataTransform;

        public PkiConfigurationAdcsTemplateController(IConfigurationRepository configurationRepository, IRuntimeConfigurationState runtimeConfigurationState)
        {
            this.configurationRepository = configurationRepository;
            this.runtimeConfigurationState = runtimeConfigurationState;
            this.http = new HttpResponseHandler(this);
            this.dataTransform = new DataTransformation();
        }

        [HttpGet]
        [Route("pki-config/templates")]
        public JsonResult GetTemplates()
        {
            IEnumerable<AdcsTemplate> result = configurationRepository.GetAdcsTemplates();
            List<AdcsTemplateGetModel> response = new List<AdcsTemplateGetModel>();

            foreach(var template in result)
            {
                AdcsTemplateGetModel item = new AdcsTemplateGetModel();
                item.Name = template.Name;
                item.Hash = template.Hash;
                item.Cipher = template.Cipher;
                item.KeyUsage = template.KeyUsage;
                item.WindowsApi = template.WindowsApi;
                item.Id = template.Id;

                item.RolesAllowedToIssueSelectView = new List<SecurityRoleSelectView>();
                foreach(Guid roleId in template.RolesAllowedToIssue)
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

            return http.RespondSuccess(response);
        }

        [HttpDelete]
        [Route("pki-config/template")]
        public JsonResult DeletePkiTemplate(Guid id)
        {
            configurationRepository.DeleteAdcsTemplates(id);

            return Json(new { status = "success" });
        }


        [HttpPut]
        [Route("pki-config/template")]
        public JsonResult UpdateAdcsTemplate(AdcsTemplateUpdateModel updateEntity)
        {
            if(updateEntity.Name == "error")
                return http.RespondPreconditionFailed("A certificate authority must exist before adding templates");

            AdcsTemplate template = configurationRepository.GetAdcsTemplate(updateEntity.Id);

            template.Name = updateEntity.Name;
            template.RolesAllowedToIssue = dataTransform.ParseGuidList(updateEntity.RolesAllowedToIssue);
            template.Hash = updateEntity.Hash;
            template.KeyUsage = updateEntity.KeyUsage;
            template.WindowsApi = updateEntity.WindowsApi;
            template.Cipher = updateEntity.Cipher;

            configurationRepository.UpdateAdcsTemplate(template);


            AdcsTemplateGetModel response = new AdcsTemplateGetModel();

            response.Name = template.Name;
            response.Hash = template.Hash;
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

            return http.RespondSuccess(response);
        }

        [HttpPost]
        [Route("pki-config/template")]
        public JsonResult AddAdcsTemplate(AdcsTemplate template)
        {
            IEnumerable<PrivateCertificateAuthorityConfig> actual = configurationRepository.GetPrivateCertificateAuthorities();

            if(actual == null)
            {
                return http.RespondPreconditionFailed("A certificate authority must exist before adding templates");
            }

            template.Id = Guid.NewGuid();
            configurationRepository.InsertAdcsTemplate(template);
            runtimeConfigurationState.ClearAlert(AlertType.NoTemplatesConfigured);
            return Json(template);
        }
    }
}