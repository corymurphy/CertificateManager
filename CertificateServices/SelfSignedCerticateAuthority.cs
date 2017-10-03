using CERTCLILib;
using CERTENROLLLib;
using CertificateServices.Enumerations;
using CertificateServices.Interfaces;
using System;

namespace CertificateServices
{
    public class SelfSignedCerticateAuthority : ICertificateAuthorityPrivate
    {
        private ICertificateProvider provider;
        public SelfSignedCerticateAuthority(ICertificateProvider provider)
        {
            this.provider = provider;
        }

        public string CommonName { get { return "SelfSignedCertificateAuthority"; } }

        public CertificateAuthorityRequestResponse Sign(CertificateRequest csr, string templateName, KeyUsage keyusage)
        {
            ICertRequest3 objCertRequest = (ICertRequest3)Activator.CreateInstance(Type.GetTypeFromProgID("CertificateAuthority.Request"));
            return null;
        }


    }
}
