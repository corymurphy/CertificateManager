using CertificateManager.Entities;
using CertificateManager.Logic;
using CertificateManager.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace CertificateManager.Controllers
{
    public class CertificateDownloadController : Controller
    {
        private const string pfxMimeType = @"application/x-pkcs12";
        private const string cerMimeType = @"application/x-pkcs7-certificates";

        ICertificateRepository certificateRepository;
        IConfigurationRepository configurationRepository;
        EncryptionProvider cipher;

        public CertificateDownloadController(ICertificateRepository certificateRepository, IConfigurationRepository configurationRepository, EncryptionProvider cipher)
        {
            this.configurationRepository = configurationRepository;
            this.certificateRepository = certificateRepository;
            this.cipher = cipher;
        }

        [HttpGet]
        [Route("certificate/{id:guid}/download/pfx/nochain")]
        public FileContentResult DownloadPfxWithoutChain(Guid id)
        {
            DownloadPfxCertificateEntity cert = certificateRepository.Get<DownloadPfxCertificateEntity>(id);


            if(!cert.HasPrivateKey || cert.CertificateStorageFormat != CertificateStorageFormat.Pfx)
            {
                throw new Exception("No private key");
            }

            return new FileContentResult(cert.Content, pfxMimeType)
            {
                FileDownloadName = String.Format("{0}.pfx", cert.Thumbprint)
            };
        }

        [Route("certificate/{id:guid}/download/pfx/includechain")]
        public FileContentResult DownloadPfxWithChain(Guid id)
        {
            DownloadPfxCertificateEntity cert = certificateRepository.Get<DownloadPfxCertificateEntity>(id);


            if (!cert.HasPrivateKey || cert.CertificateStorageFormat != CertificateStorageFormat.Pfx)
            {
                throw new Exception("No private key");
            }

            X509Certificate2 x509 = new X509Certificate2(cert.Content, cipher.Decrypt(cert.PfxPassword, cert.PasswordNonce));

            bool buildResult;
            X509Chain chain = new X509Chain();
            X509Certificate2Collection x509Col = new X509Certificate2Collection();
            try
            {
                chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                buildResult = chain.Build(x509);
            }
            catch
            {
                throw new Exception("DownloadCertificateWithChain - Unable to build certificate chain");
            }

            if (!buildResult)
                throw new Exception("DownloadCertificateWithChain: Failed to build chain");


            byte[] chainBytes = x509Col.Export(X509ContentType.Pkcs12, cipher.Decrypt(cert.PfxPassword, cert.PasswordNonce));

            return new FileContentResult(chainBytes, pfxMimeType)
            {
                FileDownloadName = String.Format("{0}.pfx", cert.Thumbprint)
            };
        }

        [Route("certificate/{id:guid}/download/cer/base64/nochain")]
        public FileContentResult DownloadCerBase64WithoutChain(Guid id)
        {
            DownloadPfxCertificateEntity cert = certificateRepository.Get<DownloadPfxCertificateEntity>(id);

            X509Certificate2 x509;
            switch (cert.CertificateStorageFormat)
            {
                case CertificateStorageFormat.Pfx:
                    x509 = new X509Certificate2(cert.Content, cipher.Decrypt(cert.PfxPassword, cert.PasswordNonce));
                    break;
                case CertificateStorageFormat.Cer:
                    x509 = new X509Certificate2(cert.Content);
                    break;
                default:
                    throw new Exception("TODO: handle this");
            }

            StringBuilder builder = new StringBuilder();
            builder.AppendLine("-----BEGIN CERTIFICATE-----");
            builder.AppendLine(Convert.ToBase64String(x509.Export(X509ContentType.Cert), Base64FormattingOptions.InsertLineBreaks));
            builder.AppendLine("-----END CERTIFICATE-----");
            return new FileContentResult(Encoding.UTF8.GetBytes(builder.ToString()), cerMimeType)
            {
                FileDownloadName = String.Format("{0}.cer", cert.Thumbprint)
            };
        }

        [Route("certificate/{id:guid}/download/cer/base64/includechain")]
        public FileContentResult DownloadCerBase64WithChain(Guid id)
        {
            DownloadPfxCertificateEntity cert = certificateRepository.Get<DownloadPfxCertificateEntity>(id);

            X509Certificate2 x509;
            switch (cert.CertificateStorageFormat)
            {
                case CertificateStorageFormat.Pfx:
                    x509 = new X509Certificate2(cert.Content, cipher.Decrypt(cert.PfxPassword, cert.PasswordNonce));
                    break;
                case CertificateStorageFormat.Cer:
                    x509 = new X509Certificate2(cert.Content);
                    break;
                default:
                    throw new Exception("TODO: handle this");
            }


            bool buildResult;
            X509Chain chain = new X509Chain();
            X509Certificate2Collection x509Col = new X509Certificate2Collection();
            try
            {
                chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                buildResult = chain.Build(x509);
            }
            catch
            {
                throw new Exception("DownloadCertificateWithChain - Unable to build certificate chain");
            }

            if (!buildResult)
                throw new Exception("DownloadCertificateWithChain: Failed to build chain");

            StringBuilder builder = new StringBuilder();
            builder.AppendLine("-----BEGIN PKCS7-----");
            builder.AppendLine(Convert.ToBase64String(x509Col.Export(X509ContentType.Pkcs7), Base64FormattingOptions.InsertLineBreaks));
            builder.AppendLine("-----END PKCS7-----");
            return new FileContentResult(Encoding.UTF8.GetBytes(builder.ToString()), cerMimeType)
            {
                FileDownloadName = String.Format("{0}.p7b", cert.Thumbprint)
            };
        }

        [Route("certificate/{id:guid}/download/cer/binary/nochain")]
        public FileContentResult DownloadCerBinaryWithoutChain(Guid id)
        {
            DownloadPfxCertificateEntity cert = certificateRepository.Get<DownloadPfxCertificateEntity>(id);

            X509Certificate2 x509;
            switch (cert.CertificateStorageFormat)
            {
                case CertificateStorageFormat.Pfx:
                    x509 = new X509Certificate2(cert.Content, cipher.Decrypt(cert.PfxPassword, cert.PasswordNonce));
                    break;
                case CertificateStorageFormat.Cer:
                    x509 = new X509Certificate2(cert.Content);
                    break;
                default:
                    throw new Exception("TODO: handle this");
            }

            return new FileContentResult(x509.Export(X509ContentType.Cert), cerMimeType)
            {
                FileDownloadName = String.Format("{0}.cer", cert.Thumbprint)
            };
        }

        [Route("certificate/{id:guid}/download/cer/binary/includechain")]
        public FileContentResult DownloadCerBinaryWithChain(Guid id)
        {
            DownloadPfxCertificateEntity cert = certificateRepository.Get<DownloadPfxCertificateEntity>(id);

            X509Certificate2 x509;
            switch (cert.CertificateStorageFormat)
            {
                case CertificateStorageFormat.Pfx:
                    x509 = new X509Certificate2(cert.Content, cipher.Decrypt(cert.PfxPassword, cert.PasswordNonce));
                    break;
                case CertificateStorageFormat.Cer:
                    x509 = new X509Certificate2(cert.Content);
                    break;
                default:
                    throw new Exception("TODO: handle this");
            }


            bool buildResult;
            X509Chain chain = new X509Chain();
            X509Certificate2Collection x509Col = new X509Certificate2Collection();
            try
            {
                chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                buildResult = chain.Build(x509);
            }
            catch
            {
                throw new Exception("DownloadCertificateWithChain - Unable to build certificate chain");
            }

            if (!buildResult)
            {
                throw new Exception("DownloadCertificateWithChain: Failed to build chain");
            }
                


            return new FileContentResult(x509Col.Export(X509ContentType.Pkcs7), cerMimeType)
            {
                FileDownloadName = String.Format("{0}.p7b", cert.Thumbprint)
            };
        }
    }
}