using CertificateManager.Logic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CertificateManager.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly AnalyticsLogic analytics;
        HttpResponseHandler http;

        public HomeController(AnalyticsLogic analytics)
        {
            http = new HttpResponseHandler(this);
            this.analytics = analytics;
        }

        [HttpGet("~/")]
        [Route("home/index")]
        public IActionResult Index()
        {
            ViewBag.CertificateHistory = analytics.GetCertificateHistory();
            ViewBag.LogHistory = analytics.GetLogHistory();

            return View();
        }

        [Route("sandbox")]
        public IActionResult Sandbox()
        {
            return View();
        }

        [Route("secure")]
        public JsonResult Secure()
        {
            return http.RespondSuccess( JsonConvert.SerializeObject(User));
        }
    }
}