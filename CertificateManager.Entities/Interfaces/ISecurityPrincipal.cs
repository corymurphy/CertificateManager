using CertificateManager.Entities.Enumerations;
using System;

namespace CertificateManager.Entities.Interfaces
{
    public interface ISecurityPrincipal
    {
        string Name { get; }
        Guid Id { get; }
        IdentityType IdentityType { get; }
    }
}
