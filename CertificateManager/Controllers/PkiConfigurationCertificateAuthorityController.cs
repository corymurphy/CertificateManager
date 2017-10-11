using CertificateManager.Entities;
using CertificateManager.Logic;
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
        HttpResponseHandler http;
        public PkiConfigurationCertificateAuthorityController(IConfigurationRepository configurationRepository)
        {
            this.configurationRepository = configurationRepository;
            this.http = new HttpResponseHandler(this);
        }

        [HttpGet]
        [Route("pki-config/certificate-authorities/private")]
        public JsonResult GetCertificateAuthorities()
        {
            IEnumerable<PrivateCertificateAuthorityConfig> actual = configurationRepository.GetPrivateCertificateAuthorities();
            return http.RespondSuccess(actual);
        }

        [HttpDelete]
        [Route("pki-config/certificate-authority/private")]
        public JsonResult DeleteCertificateAuthority(Guid id)
        {
            configurationRepository.DeletePrivateCertificateAuthority(id);
            return http.RespondSuccess();
        }


        [HttpPut]
        [Route("pki-config/certificate-authority/private")]
        public JsonResult UpdateCertificateAuthority(PrivateCertificateAuthorityConfig ca)
        {
            PrivateCertificateAuthorityConfig existingCa = configurationRepository.GetPrivateCertificateAuthority(ca.Id);

            ca.Id = existingCa.Id;

            configurationRepository.UpdatePrivateCertificateAuthority(ca);
            return http.RespondSuccess();
        }

        [HttpPost]
        [Route("pki-config/certificate-authority/private")]
        public JsonResult AddCertificateAuthority(PrivateCertificateAuthorityConfig ca)
        {
            ca.Id = Guid.NewGuid();
            configurationRepository.InsertPrivateCertificateAuthorityConfig(ca);
            //configurationRepository.Insert<PrivateCertificateAuthorityConfig>(ca);
            return http.RespondSuccess(ca);
        }
    }
}