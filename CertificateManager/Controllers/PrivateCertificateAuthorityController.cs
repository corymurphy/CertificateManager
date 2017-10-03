using CertificateManager.Entities;
using CertificateManager.Logic;
using CertificateManager.Repository;
using CertificateServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Net;
using System.Net.Http;

namespace CertificateManager.Controllers
{
    public class PrivateCertificateAuthorityController : Controller
    {
        private ICertificateRepository certificateRepository;
        private IConfigurationRepository configurationRepository;
        private ICertificateProvider certificateProvider;
        private HttpResponseHandler response;

        public PrivateCertificateAuthorityController(ICertificateRepository certRepo, IConfigurationRepository configRepo, ICertificateProvider certProvider)
        {
            this.certificateRepository = certRepo;
            this.configurationRepository = configRepo;
            this.certificateProvider = certProvider;
            this.response = new HttpResponseHandler(this);
        }

       
        [HttpPost]
        [Route("ca/private/certificate/request/includeprivatekey")]
        [SwaggerResponse(HttpStatusCode.OK, Description = "Certificate was successfully issued with privte key included in response.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The certificate request contained invalid data. Refer to the response message for details.")]
        public JsonResult CreateCertificate(CreatePrivateCertificateModel model)
        {
            PrivateCertificateProcessing processor = new PrivateCertificateProcessing(certificateRepository, configurationRepository, certificateProvider);

            CreatePrivateCertificateResult result;
            try
            {
                result = processor.CreateCertificateWithPrivateKey(model);
            }
            catch(Exception e)
            {
                return response.RespondBadRequest(e.Message);
            }

            return Json(new { id = result.Id, message = result.Message });
        }

        [HttpPost]
        [Route("ca/private/certificate/request/publickeyonly")]
        [SwaggerResponse(HttpStatusCode.OK, Description = "Certificate was successfully signed by the certificate authority.")]
        public JsonResult SignCertificaste(SignPrivateCertificateModel model)
        {

            PrivateCertificateProcessing processor = new PrivateCertificateProcessing(certificateRepository, configurationRepository, certificateProvider);

            SignPrivateCertificateResult result;
            try
            {
                result = processor.SignCertificate(model);
            }
            catch (Exception e)
            {
                return response.RespondBadRequest(e.Message);
            }
            return null;
        }

        [HttpGet]
        [Route("ca/private/certificates/request/views/new")]
        public ActionResult NewCertificateView()
        {
            return View("New");
        }
    }
}