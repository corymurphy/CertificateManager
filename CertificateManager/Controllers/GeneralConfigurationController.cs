using CertificateManager.Entities;
using CertificateManager.Entities.Models;
using CertificateManager.Logic;
using CertificateManager.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CertificateManager.Controllers
{
    [Authorize]
    public class GeneralConfigurationController : Controller
    {
        IConfigurationRepository configurationRepository;
        HttpResponseHandler http;
        public GeneralConfigurationController(IConfigurationRepository configurationRepository)
        {
            this.http = new HttpResponseHandler(this);
            this.configurationRepository = configurationRepository;
        }

        [HttpGet]
        [Route("view/general-config")]
        public ActionResult GeneralConfigView()
        {
            return View("GeneralConfiguration");
        }

        [HttpPut]
        [Route("general-config/settings")]
        public JsonResult SetSettings(SettingsModel model)
        {
            AppConfig appConfig = configurationRepository.GetAppConfig();
            appConfig.CachePeriod = model.CachePeriod;
            appConfig.JwtCertificateId = new System.Guid("642b706b-4664-4296-9a0d-58869a62db42");
            configurationRepository.SetAppConfig(appConfig);

            return http.RespondSuccess();
        }

        [HttpPut]
        [Route("general-config/audit-config")]
        public JsonResult SetAuditConfig(AuditConfigurationModel model)
        {
            AppConfig appConfig = configurationRepository.GetAppConfig();
            appConfig.OperationsLoggingState = model.OperationsLoggingState;
            appConfig.SecurityAuditingState = model.SecurityAuditingState;
            configurationRepository.SetAppConfig(appConfig);

            return http.RespondSuccess();
        }

        [HttpGet]
        [Route("app-config")]
        public JsonResult GetAppConfig()
        {
            return http.RespondSuccess(configurationRepository.GetAppConfig());
        }

    }
}