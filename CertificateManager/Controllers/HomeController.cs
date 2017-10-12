using CertificateManager.Logic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        [Route("secure")]
        public JsonResult Secure()
        {
            return http.RespondSuccess(User);
        }
    }
}