using CertificateManager.Entities;
using CertificateManager.Logic;
using CertificateManager.Logic.Interfaces;
using CertificateManager.Repository;
using Microsoft.AspNetCore.Mvc;

namespace CertificateManager.Controllers
{
    public class PendingCertificatesController : Controller
    {
        private ICertificateRepository certificateRepository;
        private IConfigurationRepository configurationRepository;
        private IAuthorizationLogic authorizationLogic;
        private HttpResponseHandler http;
        private AuditLogic audit;

        public PendingCertificatesController(ICertificateRepository certRepo, IConfigurationRepository configRepo, IAuthorizationLogic authorizationLogic, AuditLogic auditLogic)
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

        [Route("view/certificate/request/pending")]
        [HttpGet]
        public ActionResult ViewAllPendingCertificates()
        {
            return View();
        }
    }
}
