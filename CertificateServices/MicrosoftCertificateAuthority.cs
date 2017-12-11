using CERTCLILib;
using CertificateServices.Enumerations;
using System;

namespace CertificateServices
{
    public class MicrosoftCertificateAuthority : ICertificateAuthorityPrivate
    {
        private const int CR_IN_BASE64 = 0x1;
        private const int CR_IN_FORMATANY = 0;
        private const int CR_IN_PKCS10 = 0x100;
        private const int CR_DISP_ISSUED = 0x3;
        private const int CR_DISP_UNDER_SUBMISSION = 0x5;
        private const int CR_OUT_BASE64 = 0x1;
        private const int CR_DISP_DENIED = 0x2;
        private const int CR_DISP_ERROR = 0x6;
        private const int CR_OUT_CHAIN = 0x100;

        private string username;
        private string password;
        private string authRealm;
        private MicrosoftCertificateAuthorityAuthenticationType AuthenticationType;
        public MicrosoftCertificateAuthority(MicrosoftCertificateAuthorityOptions options)
        {
            AuthenticationType = options.AuthenticationType;

            if(AuthenticationType == MicrosoftCertificateAuthorityAuthenticationType.UsernamePassword)
            {
                this.username = options.Username;
                this.password = options.Password;
                this.authRealm = options.AuthenticationRealm;
            }

            this.ServerName = options.ServerName;
            this.CommonName = options.CommonName;
        }

        public MicrosoftCertificateAuthority(string serverName, string commonName)
        {
            ValidateParams(serverName, commonName);
            //TODO: validate that the certificate authority supports the template
            this.ServerName = serverName;
            this.CommonName = commonName;
        }

        public string ServerName { get; private set; }
        public string CommonName { get; private set; }
        public string TemplateName { get; private set; }

        private void ValidateParams(string serverName, string commonName)
        {
            if (string.IsNullOrWhiteSpace(serverName))
                throw new ArgumentNullException(nameof(serverName));

            if (string.IsNullOrWhiteSpace(commonName))
                throw new ArgumentNullException(nameof(commonName));
        }

        public CertificateAuthorityRequestResponse Sign(CertificateRequest csr, string templateName, KeyUsage keyusage = KeyUsage.ServerAuthentication)
        {

            ICertRequest3 objCertRequest = (ICertRequest3)Activator.CreateInstance(Type.GetTypeFromProgID("CertificateAuthority.Request"));

            string templateArg = $"CertificateTemplate:{templateName}";
            string serverArg = String.Format(@"{0}\{1}", ServerName, CommonName);


            int requestStatus = 0x0;
            try
            {
                if(AuthenticationType == MicrosoftCertificateAuthorityAuthenticationType.UsernamePassword)
                {
                    using (WindowsImpersonation context = new WindowsImpersonation(username, authRealm, password))
                    {
                        requestStatus = objCertRequest.Submit((CR_IN_BASE64 | CR_IN_FORMATANY), csr.EncodedCsr, templateArg, serverArg);
                    }
                }
                else
                {
                    requestStatus = objCertRequest.Submit((CR_IN_BASE64 | CR_IN_FORMATANY), csr.EncodedCsr, templateArg, serverArg);
                }
            }
            catch(Exception e)
            {

                requestStatus = 0x6;
            }

            switch (requestStatus)
            {
                case CR_DISP_ISSUED:
                    return new CertificateAuthorityRequestResponse(objCertRequest.GetRequestId(), CertificateRequestStatus.Issued, CommonName, objCertRequest.GetCertificate(CR_OUT_BASE64));
                case CR_DISP_UNDER_SUBMISSION:
                    return new CertificateAuthorityRequestResponse(objCertRequest.GetRequestId(), CertificateRequestStatus.Pending, CommonName);
                case CR_DISP_DENIED:
                    return new CertificateAuthorityRequestResponse(CertificateRequestStatus.Denied, CommonName);
                case CR_DISP_ERROR:
                    return new CertificateAuthorityRequestResponse(CertificateRequestStatus.Error, CommonName);
                default:
                    return new CertificateAuthorityRequestResponse(CertificateRequestStatus.Error, CommonName);
            }
        }

        private int SubmitRequest(CertificateRequest csr, string templateName)
        {
            ICertRequest3 objCertRequest = (ICertRequest3)Activator.CreateInstance(Type.GetTypeFromProgID("CertificateAuthority.Request"));

            string templateArg = $"CertificateTemplate:{templateName}";
            string serverArg = String.Format(@"{0}\{1}", ServerName, CommonName);

            return objCertRequest.Submit((CR_IN_BASE64 | CR_IN_FORMATANY), csr.EncodedCsr, templateArg, serverArg);
        }

        private int SubmitRequestUsernamePassword(CertificateRequest csr, string templateName)
        {
            ICertRequest3 objCertRequest = (ICertRequest3)Activator.CreateInstance(Type.GetTypeFromProgID("CertificateAuthority.Request"));

            string templateArg = $"CertificateTemplate:{templateName}";
            string serverArg = String.Format(@"{0}\{1}", ServerName, CommonName);

            return objCertRequest.Submit((CR_IN_BASE64 | CR_IN_FORMATANY), csr.EncodedCsr, templateArg, serverArg);
        }

        private int SubmitRequestWindowsAuthentication(CertificateRequest csr, string templateName)
        {
            ICertRequest3 objCertRequest = (ICertRequest3)Activator.CreateInstance(Type.GetTypeFromProgID("CertificateAuthority.Request"));

            string templateArg = $"CertificateTemplate:{templateName}";
            string serverArg = String.Format(@"{0}\{1}", ServerName, CommonName);

            return objCertRequest.Submit((CR_IN_BASE64 | CR_IN_FORMATANY), csr.EncodedCsr, templateArg, serverArg);
        }

        private bool caSupportsTemplate() { throw new NotImplementedException(); }
        private bool caSupportsHashAlgorithm() { throw new NotImplementedException(); }
    }
}
