using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CertificateManager.Repository;
using CertificateManager.Logic;
using CertificateServices.Interfaces;
using CertificateManager.Entities;

namespace CertificateManager.Controllers
{
    public class CertificatesController : Controller
    {

        ICertificateRepository certificateRepository;
        IConfigurationRepository configurationRepository;
        DataTransformation dataTransformation;
        SecretKeyProvider secrets;
        HttpResponseHandler http;

        public CertificatesController(ICertificateRepository certificateRepository, IConfigurationRepository configurationRepository)
        {
            this.configurationRepository = configurationRepository;
            this.certificateRepository = certificateRepository;
            this.dataTransformation = new DataTransformation();
            this.secrets = new SecretKeyProvider();
            this.http = new HttpResponseHandler(this);
        }

        [HttpGet]
        [Route("certificate/{id:guid}")]
        public JsonResult GetCertificate(Guid id)
        {
            GetCertificateEntity cert = certificateRepository.GetCertificate<GetCertificateEntity>(id);

            return Json(cert);
        }

        [Route("view/certificate/{id:guid}")]
        public ActionResult ViewCertificate(Guid id)
        {
            ViewBag.CertificateId = id;
            return View("View");
        }

        [Route("view/certificates")]
        public ActionResult ViewAllCertificates()
        {
            return View("ViewAllCertificates");
        }

        [Route("certificates")]
        public JsonResult AllCertificates()
        {
            return http.RespondSuccess(certificateRepository.FindAllCertificates());
        }

        [HttpDelete]
        [Route("certificate")]
        public JsonResult DeleteCertificate(Guid id)
        {
            return null;
        }



    }
}