using CertificateManager.Entities;
using CertificateManager.Logic;
using CertificateManager.Logic.Interfaces;
using CertificateManager.Repository;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CertificateManager.Controllers
{
    public class PendingCertificatesController : Controller
    {
        private ICertificateRepository certificateRepository;
        private IConfigurationRepository configurationRepository;
        private IAuthorizationLogic authorizationLogic;
        private HttpResponseHandler http;
        private IAuditLogic audit;

        public PendingCertificatesController(ICertificateRepository certRepo, IConfigurationRepository configRepo, IAuthorizationLogic authorizationLogic, IAuditLogic auditLogic)
        {
            this.certificateRepository = certRepo;
            this.configurationRepository = configRepo;
            this.authorizationLogic = authorizationLogic;
            this.http = new HttpResponseHandler(this);
            this.audit = auditLogic;
        }


        [HttpGet]
        [Route("certificate/request/pending")]
        public JsonResult GetAllPendingCertificates()
        {
            return http.RespondSuccess(certificateRepository.GetAll<PendingCertificate>());
        }

        [HttpDelete]
        [Route("certificate/request/pending/{id:guid}")]
        public JsonResult DenyPendingCertificate(Guid id)
        {
            certificateRepository.Delete<PendingCertificate>(id);
            return http.RespondSuccess();
        }

        [HttpPost]
        [Route("certificate/request/pending/{id:guid}")]
        public JsonResult IssuePendingCertificate(Guid id)
        {
            return http.RespondSuccess();
            //return http.RespondSuccess(certificateRepository.GetAll<PendingCertificate>());
        }

        [Route("view/certificate/request/pending")]
        [HttpGet]
        public ActionResult ViewAllPendingCertificates()
        {
            return View();
        }
    }
}
