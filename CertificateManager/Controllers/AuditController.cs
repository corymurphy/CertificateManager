using CertificateManager.Logic;
using CertificateManager.Logic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CertificateManager.Controllers
{
    [Authorize]
    public class AuditController : Controller
    {
        IAuditLogic audit;
        HttpResponseHandler http;

        public AuditController(IAuditLogic audit)
        {
            this.audit = audit;
            http = new HttpResponseHandler(this);
        }

        [HttpGet]
        [Route("view/logs")]
        public ActionResult ViewLogs()
        {
            return View("Logs");
        }

        [HttpGet]
        [Route("logs")]
        public JsonResult GetLogs()
        {
            return http.RespondSuccess(audit.GetAllEvents());
        }
    }
}