using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CertificateManager.Controllers
{
    [Authorize]
    public class AuditController : Controller
    {
        [HttpGet]
        [Route("view/logs")]
        public ActionResult ViewLogs()
        {
            return View("Logs");
        }
    }
}