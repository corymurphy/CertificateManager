using CertificateManager.Entities;
using CertificateManager.Logic;
using CertificateManager.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace CertificateManager.Controllers
{
    public class PkiConfigurationCertificateAuthorityController : Controller
    {
        CertificateAuthorityConfigurationLogic caConfigLogic;
        HttpResponseHandler http;
        public PkiConfigurationCertificateAuthorityController(IConfigurationRepository configurationRepository)
        {
            this.caConfigLogic = new CertificateAuthorityConfigurationLogic(configurationRepository);
            this.http = new HttpResponseHandler(this);
        }

        [HttpGet]
        [Route("pki-config/certificate-authorities/private")]
        public JsonResult GetCertificateAuthorities()
        {
            IEnumerable<PrivateCertificateAuthorityConfig> actual = caConfigLogic.GetPrivateCertificateAuthorities();
            return http.RespondSuccess(actual);
        }

        [HttpDelete]
        [Route("pki-config/certificate-authority/private")]
        public JsonResult DeleteCertificateAuthority(Guid id)
        {
            caConfigLogic.DeletePrivateCertificateAuthority(id);
            return http.RespondSuccess();
        }


        [HttpPut]
        [Route("pki-config/certificate-authority/private")]
        public JsonResult UpdateCertificateAuthority(PrivateCertificateAuthorityConfig ca)
        {
            caConfigLogic.UpdatePrivateCertificateAuthority(ca);
            return http.RespondSuccess();
        }

        [HttpPost]
        [Route("pki-config/certificate-authority/private")]
        public JsonResult AddCertificateAuthority(PrivateCertificateAuthorityConfig ca)
        {
            caConfigLogic.AddPrivateCertificateAuthority(ca);
            return http.RespondSuccess(ca);
        }
    }
}