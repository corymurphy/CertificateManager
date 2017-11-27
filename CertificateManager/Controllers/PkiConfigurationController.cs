using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CertificateManager.Controllers
{
    [Authorize]
    public class PkiConfigurationController : Controller
    {
        [HttpGet]
        [Route("view/pki-config")]
        public ActionResult PkiConfigurationView()
        {
            return View("View");
        }
    }
}