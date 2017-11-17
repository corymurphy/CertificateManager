using CertificateManager.Logic;
using CertificateManager.Entities;
using CertificateManager.Repository;
using Microsoft.AspNetCore.Mvc;
using CertificateManager.Logic.ConfigurationProvider;

namespace CertificateManager.Controllers
{
    public class InitialSetupController : Controller
    {
        IRuntimeConfigurationState runtimeConfigurationState;
        IConfigurationRepository configurationRepository;
        HttpResponseHandler http;
        IWritableOptions<AppSettings> appSettings;

        public InitialSetupController(IConfigurationRepository configurationRepository, IRuntimeConfigurationState runtimeConfigurationState, IWritableOptions<AppSettings> appSettings)
        {
            this.appSettings = appSettings;
            this.runtimeConfigurationState = runtimeConfigurationState;
            this.configurationRepository = configurationRepository;
            this.http = new HttpResponseHandler(this);
        }


        [HttpGet]
        [Route("initial-setup")]
        public ActionResult InitialSetup()
        {








            appSettings.Update(options => options.DatastoreRootPath = "a");
            SecretKeyProvider secretKeyProvider = new SecretKeyProvider();
            ViewBag.EncryptionKey = secretKeyProvider.NewSecret(32);
            return View();
        }

        [HttpPost]
        [Route("initial-setup")]
        public ActionResult SetInitialConfig(InitialSetupConfigModel config)
        {
            runtimeConfigurationState.InitialSetupComplete = true;
            return RedirectToAction("Login", "Authentication");
            //return null;
        }

        [HttpPut]
        [Route("initial-setup")]
        public JsonResult SetLocalConfig(AppConfig newConfig)
        {
            return null;
        }

    }
}