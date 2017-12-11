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
        //AdcsTemplateLogic AdcsTemplateLogic;
        //IConfigurationRepository configurationRepository;
        //IRuntimeConfigurationState runtimeConfigurationState;
        AdcsTemplateLogic adcsTemplateLogic;
        HttpResponseHandler http;
        DataTransformation dataTransform;

        public PkiConfigurationAdcsTemplateController(AdcsTemplateLogic templateLogic)
        {
            //this.configurationRepository = configurationRepository;
            //this.runtimeConfigurationState = runtimeConfigurationState;
            //this.adcsTemplateLogic = new AdcsTemplateLogic(configurationRepository, null);
            this.adcsTemplateLogic = templateLogic;
            this.http = new HttpResponseHandler(this);
            this.dataTransform = new DataTransformation();
        }

        [HttpGet]
        [Route("pki-config/templates")]
        public JsonResult GetTemplates()
        {
            return http.RespondSuccess(adcsTemplateLogic.GetTemplates());
        }

        [HttpDelete]
        [Route("pki-config/template")]
        public JsonResult DeletePkiTemplate(Guid id)
        {
            adcsTemplateLogic.DeleteTemplate(id);
            return http.RespondSuccess();
        }

        [HttpPut]
        [Route("pki-config/template")]
        public JsonResult UpdateAdcsTemplate(AdcsTemplateUpdateModel updateEntity)
        {
            return http.RespondSuccess(adcsTemplateLogic.UpdateTemplate(updateEntity));
        }

        [HttpPost]
        [Route("pki-config/template")]
        public JsonResult AddAdcsTemplate(AdcsTemplate template)
        {
            return http.RespondSuccess(adcsTemplateLogic.AddTemplate(template));    
        }
    }
}