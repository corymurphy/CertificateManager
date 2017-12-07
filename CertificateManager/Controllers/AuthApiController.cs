using CertificateManager.Logic;
using CertificateManager.Entities;
using CertificateManager.Repository;
using Microsoft.AspNetCore.Mvc;

namespace CertificateManager.Controllers
{
    public class AuthApiController : Controller
    {
        ICertificateRepository certificateRepository;
        IConfigurationRepository configurationRepository;
        HttpResponseHandler http;

        public AuthApiController(IConfigurationRepository configurationRepository, ICertificateRepository certificateRepository)
        {
            this.configurationRepository = configurationRepository;
            this.certificateRepository = certificateRepository;
            this.http = new HttpResponseHandler(this);
        }


        [HttpGet]
        [Route("identity-sources/local-authapi/trustedcertificates")]
        public ActionResult GetTrustedCertificates()
        {
            return http.RespondSuccess(configurationRepository.GetAll<AuthApiCertificate>());
        }


        [HttpPost]
        [Route("identity-sources/local-authapi/trustedcertificate")]
        public ActionResult InsertTrustedCertificates(AuthApiCertificate entity)
        {
            Certificate cert = certificateRepository.Get<Certificate>(entity.Id);

            entity.HasPrivateKey = cert.HasPrivateKey;
            entity.Thumbprint = cert.Thumbprint;
            entity.DisplayName = cert.DisplayName;

            configurationRepository.Insert<AuthApiCertificate>(entity);
            return http.RespondSuccess(entity);
        }


        [HttpPut]
        [Route("identity-sources/local-authapi/trustedcertificate")]
        public ActionResult UpdateTrustedCertificates(AuthApiCertificate entity)
        {
            configurationRepository.Update<AuthApiCertificate>(entity);
            return http.RespondSuccess();
        }

        [HttpDelete]
        [Route("identity-sources/local-authapi/trustedcertificate")]
        public ActionResult DeleteTrustedCertificates(AuthApiCertificate entity)
        {
            configurationRepository.Delete<AuthApiCertificate>(entity.Id);
            return http.RespondSuccess();
        }
    }
}