using CertificateManager.Logic;
using CertificateManager.Repository;
using Microsoft.AspNetCore.Mvc;

namespace CertificateManager.Controllers
{
    public class SearchCertificatesController : Controller
    {
        ICertificateRepository certificateRepository;
        IConfigurationRepository configurationRepository;
        HttpResponseHandler http;

        public SearchCertificatesController(ICertificateRepository certificateRepository, IConfigurationRepository configurationRepository)
        {
            this.configurationRepository = configurationRepository;
            this.certificateRepository = certificateRepository;
            this.http = new HttpResponseHandler(this);
        }

        [HttpGet]
        [Route("view/certificates/search")]
        public ActionResult SearchCertificatesView()
        {
            return View("View");
        }

        [HttpGet]
        [Route("certificates/search")]
        public JsonResult SearchCertificates(string query)
        {

            return Json( new { data = certificateRepository.FindCertificates(query) } );

            //DownloadPfxCertificateEntity cert = certificateRepository.GetCertificate<DownloadPfxCertificateEntity>(id);


            //if(!cert.HasPrivateKey || cert.CertificateStorageFormat != CertificateStorageFormat.Pfx)
            //{
            //    throw new Exception("No private key");
            //}

            //return new FileContentResult(Convert.FromBase64String(cert.Content), pfxMimeType)
            //{
            //    FileDownloadName = String.Format("{0}.pfx", cert.Thumbprint)
            //};
        }

        [HttpGet]
        [Route("certificates/search/summary")]
        public JsonResult SearchCertificateSummary(string query)
        {
            if(!string.IsNullOrWhiteSpace(query))
                return http.RespondSuccess(certificateRepository.FindCertificatesAmbiguousNameResolution(query));
            else
                return http.RespondSuccess(certificateRepository.FindCertificates(query));
        }
    }
}