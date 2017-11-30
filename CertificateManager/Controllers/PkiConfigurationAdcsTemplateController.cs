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
        AdcsTemplateLogic AdcsTemplateLogic;
        IConfigurationRepository configurationRepository;
        IRuntimeConfigurationState runtimeConfigurationState;
        AdcsTemplateLogic adcsTemplateLogic;
        HttpResponseHandler http;
        DataTransformation dataTransform;

        public PkiConfigurationAdcsTemplateController(IConfigurationRepository configurationRepository)
        {
            this.configurationRepository = configurationRepository;
            //this.runtimeConfigurationState = runtimeConfigurationState;
            this.adcsTemplateLogic = new AdcsTemplateLogic(configurationRepository);
            this.http = new HttpResponseHandler(this);
            this.dataTransform = new DataTransformation();
        }

        [HttpGet]
        [Route("pki-config/templates")]
        public JsonResult GetTemplates()
        {
            try
            {
                IEnumerable<AdcsTemplateGetModel> templates = adcsTemplateLogic.GetTemplates();
                return http.RespondSuccess(templates);
            }
            catch(Exception e)
            {
                return http.RespondServerError();
            }
        }

        [HttpDelete]
        [Route("pki-config/template")]
        public JsonResult DeletePkiTemplate(Guid id)
        {
            try
            {
                adcsTemplateLogic.DeleteTemplate(id);
                return http.RespondSuccess();
            }
            catch
            {
                return http.RespondServerError();
            }
        }

        [HttpPut]
        [Route("pki-config/template")]
        public JsonResult UpdateAdcsTemplate(AdcsTemplateUpdateModel updateEntity)
        {
            try
            {
                AdcsTemplateGetModel result = adcsTemplateLogic.UpdateTemplate(updateEntity);
                return http.RespondSuccess(result);
            }
            catch(Exception e)
            {
                return http.RespondPreconditionFailed(e.Message);
            }
        }

        [HttpPost]
        [Route("pki-config/template")]
        public JsonResult AddAdcsTemplate(AdcsTemplate template)
        {
            try
            {
                adcsTemplateLogic.AddTemplate(template);
                return Json(template);
            }
            catch(Exception e)
            {
                return http.RespondPreconditionFailed(e.Message);
            }
        }
    }
}