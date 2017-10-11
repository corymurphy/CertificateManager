using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CertificateManager.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        
        [HttpGet("~/")]
        [Route("home/index")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("secure")]
        public JsonResult Secure()
        {
            return Json(new { status = "success" });
        }
    }
}