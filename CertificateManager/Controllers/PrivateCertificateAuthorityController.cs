using CertificateManager.Entities;
using CertificateManager.Logic;
using CertificateManager.Logic.Interfaces;
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
        private IAuthorizationLogic authorizationLogic;
        private PrivateCertificateProcessing processor;
        private HttpResponseHandler http;
        private AuditLogic audit;

        public PrivateCertificateAuthorityController(ICertificateRepository certRepo, IConfigurationRepository configRepo, ICertificateProvider certProvider, IAuthorizationLogic authorizationLogic, AuditLogic auditLogic)
        {
            this.certificateRepository = certRepo;
            this.configurationRepository = configRepo;
            this.certificateProvider = certProvider;
            this.authorizationLogic = authorizationLogic;
            this.http = new HttpResponseHandler(this);
            this.audit = auditLogic;

        }

        [HttpPost]
        [Route("ca/private/certificate/request/issue-pending/{id:guid}")]
        [SwaggerResponse(HttpStatusCode.OK, Description = "Certificate was successfully issued with privte key included in response.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The certificate request contained invalid data. Refer to the response message for details.")]
        public JsonResult IssuePendingCertificate(Guid id)
        {
            PrivateCertificateProcessing processor = new PrivateCertificateProcessing(certificateRepository, configurationRepository, certificateProvider, authorizationLogic, User);

            CreatePrivateCertificateResult result = processor.IssuePendingCertificate(id);

            return http.RespondSuccess(result);
        }


        [HttpPost]
        [Route("ca/private/certificate/request/includeprivatekey")]
        [SwaggerResponse(HttpStatusCode.OK, Description = "Certificate was successfully issued with privte key included in response.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The certificate request contained invalid data. Refer to the response message for details.")]
        public JsonResult CreateCertificate(CreatePrivateCertificateModel model)
        {
            PrivateCertificateProcessing processor = new PrivateCertificateProcessing(certificateRepository, configurationRepository, certificateProvider, authorizationLogic, User);

            CreatePrivateCertificateResult result = processor.CreateCertificateWithPrivateKey(model);

            return http.RespondSuccess(result);
        }

        [HttpPost]
        [Route("ca/private/certificate/request/publickeyonly")]
        [SwaggerResponse(HttpStatusCode.OK, Description = "Certificate was successfully signed by the certificate authority.")]
        public JsonResult SignCertificaste(SignPrivateCertificateModel model)
        {
            PrivateCertificateProcessing processor = new PrivateCertificateProcessing(certificateRepository, configurationRepository, certificateProvider, authorizationLogic, User);

            SignPrivateCertificateResult result = processor.SignCertificate(model);

            return http.RespondSuccess(result);
        }

        [HttpGet]
        [Route("ca/private/certificates/request/views/new")]
        public ActionResult NewCertificateView()
        {
            return View("New");
        }
    }
}