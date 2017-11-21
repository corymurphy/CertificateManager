using CertificateManager.Logic;
using CertificateManager.Entities;
using CertificateManager.Repository;
using Microsoft.AspNetCore.Mvc;
using CertificateManager.Logic.ConfigurationProvider;

namespace CertificateManager.Controllers
{
    public class InitialSetupController : Controller
    {
        HttpResponseHandler http;
        IWritableOptions<AppSettings> appSettings;

        public InitialSetupController(IWritableOptions<AppSettings> appSettings)
        {
            this.appSettings = appSettings;
            this.http = new HttpResponseHandler(this);
        }


        [HttpGet]
        [Route("initial-setup")]
        public ActionResult InitialSetup()
        {
            //appSettings.Update(options => options.DatastoreRootPath = "a");
            SecretKeyProvider secretKeyProvider = new SecretKeyProvider();
            ViewBag.EncryptionKey = secretKeyProvider.NewSecret(32);
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

            initialSetupLogic.SetConfiguration(config);
            
            return RedirectToAction("Login", "Authentication");
        }

    }
}