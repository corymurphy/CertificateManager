using CertificateManager.Entities;
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
        [Route("general-config/audit-config")]
        public JsonResult GetAuditConfig()
        {
            return http.RespondSuccess(configurationRepository.GetAppConfig());
        }

    }
}