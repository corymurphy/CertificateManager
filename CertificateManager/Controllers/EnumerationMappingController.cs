using CertificateManager.Entities.Enumerations;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CertificateManager.Controllers
{
    public class EnumerationMappingController : Controller
    {
        [HttpGet]
        [Route("view/enum-mapping/hashAlgorithm")]
        public JsonResult GetHashAlgorithmOptions()
        {
            ViewEnumOptions enumOptions = new ViewEnumOptions();
            return Json(enumOptions.HashAlgorithOptions);
        }

        [HttpGet]
        [Route("view/enum-mapping")]
        public JsonResult GetEnumMapping()
        {
            ViewEnumOptions enumOptions = new ViewEnumOptions();
            return Json(enumOptions);
        }
    }
}