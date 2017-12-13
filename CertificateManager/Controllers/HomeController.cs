using CertificateManager.Logic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CertificateManager.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        HttpResponseHandler http;

        public HomeController()
        {
            http = new HttpResponseHandler(this);
        }

        [HttpGet("~/")]
        [Route("home/index")]
        public IActionResult Index()
        {
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