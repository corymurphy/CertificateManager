using CertificateManager.Repository;
using CertificateServices;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace CertificateManager.Controllers
{
    public class PkiConfigurationCertificateAuthorityController : Controller
    {
        IConfigurationRepository configurationRepository;

        public PkiConfigurationCertificateAuthorityController(IConfigurationRepository configurationRepository)
        {
            this.configurationRepository = configurationRepository;
        }

        [HttpGet]
        [Route("pki-config/certificate-authorities/private")]
        public JsonResult GetCertificateAuthorities()
        {
            List<MicrosoftCertificateAuthorityOptions> modified = new List<MicrosoftCertificateAuthorityOptions>();
            IEnumerable<MicrosoftCertificateAuthorityOptions> actual = configurationRepository.GetPrivateCertificateAuthorities();

            foreach(MicrosoftCertificateAuthorityOptions item in actual)
            {
                item.Password = "********";
                modified.Add(item);
            }

            return Json(modified);
        }

        [HttpDelete]
        [Route("pki-config/certificate-authority/private")]
        public JsonResult DeleteCertificateAuthority(Guid id)
        {
            configurationRepository.DeletePrivateCertificateAuthority(id);

            return Json(new { status = "success" });
        }


        [HttpPut]
        [Route("pki-config/certificate-authority/private")]
        public JsonResult UpdateCertificateAuthority(MicrosoftCertificateAuthorityOptions ca)
        {
            MicrosoftCertificateAuthorityOptions existingCa = configurationRepository.GetPrivateCertificateAuthority(ca.Id);

            ca.Password = existingCa.Password;
            ca.Id = existingCa.Id;

            configurationRepository.UpdatePrivateCertificateAuthority(ca);
            return Json(new { status = "success" });
        }

        [HttpPost]
        [Route("pki-config/certificate-authority/private")]
        public JsonResult AddCertificateAuthority(MicrosoftCertificateAuthorityOptions ca)
        {
            ca.Id = Guid.NewGuid();
            configurationRepository.Insert<MicrosoftCertificateAuthorityOptions>(ca);
            return Json(ca);
        }
    }
}