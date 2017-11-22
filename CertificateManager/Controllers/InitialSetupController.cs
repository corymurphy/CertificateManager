using CertificateManager.Entities;
using CertificateManager.Logic;
using CertificateManager.Logic.ConfigurationProvider;
using CertificateManager.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace CertificateManager.Controllers
{
    public class InitialSetupController : Controller
    {
        IApplicationLifetime application;
        HttpResponseHandler http;
        IWritableOptions<AppSettings> appSettings;

        public InitialSetupController(IWritableOptions<AppSettings> appSettings, IApplicationLifetime application)
        {
            this.application = application;
            this.appSettings = appSettings;
            this.http = new HttpResponseHandler(this);
        }

        [HttpGet]
        [Route("initial-setup/complete")]
        public ActionResult Complete()
        {
            return View();
        }


        [HttpGet]
        [Route("initial-setup")]
        public ActionResult InitialSetup()
        {
           
            SecretKeyProvider secretKeyProvider = new SecretKeyProvider();
            ViewBag.EncryptionKey = secretKeyProvider.NewSecretBase64(32);
            ViewBag.EmergencyAccessKey = secretKeyProvider.NewSecret(32);

            return View();
        }

        [HttpPost]
        [Route("initial-setup")]
        public ActionResult SetInitialConfig(InitialSetupConfigModel config)
        {
            appSettings.Update(setting => setting.DatastoreRootPath = config.DatastoreRootPath);

            DatabaseLocator dbLocator = new DatabaseLocator(config.DatastoreRootPath);

            LiteDbConfigurationRepository configurationRepository = new LiteDbConfigurationRepository(dbLocator.GetConfigurationRepositoryConnectionString());

            InitialSetupLogic initialSetupLogic = new InitialSetupLogic(configurationRepository);

            initialSetupLogic.Complete(config);
            
            return RedirectToAction("Complete", "InitialSetup");
        }

        [Route("initial-setup/restart-app")]
        public JsonResult RestartApp()
        {
            application.StopApplication();
            return http.RespondSuccess();
        }

    }
}