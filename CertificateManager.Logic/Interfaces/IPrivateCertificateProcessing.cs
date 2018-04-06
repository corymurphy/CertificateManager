using CertificateManager.Entities;
using System;
using System.Security.Claims;

namespace CertificateManager.Logic.Interfaces
{
    public interface IPrivateCertificateProcessing
    {
        SignPrivateCertificateResult SignCertificate(SignPrivateCertificateModel model, ClaimsPrincipal user);

        CreatePrivateCertificateResult CreateCertificateWithPrivateKey(CreatePrivateCertificateModel model, ClaimsPrincipal user);

        CreatePrivateCertificateResult IssuePendingCertificate(Guid id, ClaimsPrincipal user);
    }
}
