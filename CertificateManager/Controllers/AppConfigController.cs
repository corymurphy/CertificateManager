using CertificateManager.Logic;
using CertificateManager.Entities;
using CertificateManager.Repository;
using Microsoft.AspNetCore.Mvc;

namespace CertificateManager.Controllers
{
    public class AppConfigController : Controller
    {
        IConfigurationRepository configurationRepository;
        HttpResponseHandler http;

        public AppConfigController(IConfigurationRepository configurationRepository)
        {
            this.configurationRepository = configurationRepository;
            this.http = new HttpResponseHandler(this);
        }


        [HttpGet]
        [Route("config")]
        public ActionResult GetAppConfig()
        {
            return http.RespondSuccess(configurationRepository.GetAppConfig());
        }

        [HttpPut]
        [Route("config")]
        public ActionResult SetAppConfig(AppConfig newConfig)
        {
            AppConfig existinConfig = configurationRepository.GetAppConfig();
            existinConfig.JwtValidityPeriod = newConfig.JwtValidityPeriod;
            existinConfig.LocalIdpIdentifier = newConfig.LocalIdpIdentifier;

            configurationRepository.SetAppConfig(existinConfig);

            return http.RespondSuccess();
        }

        [HttpPut]
        [Route("config/local")]
        public JsonResult SetLocalConfig(AppConfig newConfig)
        {
            AppConfig existinConfig = configurationRepository.GetAppConfig();
            existinConfig.LocalLogonEnabled = newConfig.LocalLogonEnabled;
            existinConfig.EmergencyAccessEnabled = newConfig.EmergencyAccessEnabled;
            existinConfig.windowsAuthEnabled = newConfig.windowsAuthEnabled;
            configurationRepository.SetAppConfig(existinConfig);

            return http.RespondSuccess();
        }

    }
}