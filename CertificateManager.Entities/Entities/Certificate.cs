using CertificateManager.Entities.Attributes;
using CertificateManager.Entities.Enumerations;
using CertificateManager.Entities.Interfaces;
using CertificateServices;
using CertificateServices.Enumerations;
using System;
using System.Collections.Generic;

namespace CertificateManager.Entities
{
    [Repository("Certificate")]
    public class Certificate : ICertificatePasswordEntity
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public CertificateSubject Subject { get; set; }
        public CertificateStorageFormat CertificateStorageFormat { get; set; }
        public string Content { get; set; }
        public string SubjectKeyIdentifier { get; set; }
        public string Thumbprint { get; set; }
        public CertificateAuthorityType CertificateAuthorityType { get; set; }
        public DateTime ValidTo { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime IssuedOn { get; set; }
        public DateTime RequestedOn { get; set; }
        public string SigningAlgorithm { get; set; }
        public CipherAlgorithm CipherAlgorithm { get; set; }
        public WindowsApi WindowsApi { get; set; }
        public HashAlgorithm HashAlgorithm { get; set; }
        public string RequestingUser { get; set; }
        public string FilePath { get; set; }
        public bool HasPrivateKey { get; set; }
        public string TemplateName { get; set; }
        public string CertificateAuthorityName { get; set; }
        public string CertificateAuthorityServerName { get; set; }
        public CertificateRequestStatus Status { get; set; }
        public CertificateRequestStatus ExternalCertificateStatus { get; set; }
        public string PfxPassword { get; set; }
        public int KeySize { get; set; }
        public string EncodedRequest { get; set; }
        public string EncodedCertificate { get; set; }
        public string PasswordNonce { get; set; }
        public string NotificationEmail { get; set; }
        public bool Archive { get; set; }
        public DateTime ArchiveDate { get; set; }
        public string PreviousPfxPassword { get; set; }
        public string PreviousPasswordNonce { get; set; }
        public string PreviousFilePath { get; set; }
        public bool IsCertificateAuthority { get; set; }
        public List<AccessControlEntry> Acl { get; set; }
        public KeyUsage KeyUsage { get; set; }
    }
}
