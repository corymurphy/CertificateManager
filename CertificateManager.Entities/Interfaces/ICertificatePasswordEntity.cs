using System;
using System.Collections.Generic;

namespace CertificateManager.Entities.Interfaces
{
    public interface ICertificatePasswordEntity
    {
        Guid Id { get; }
        List<AccessControlEntry> Acl { get; }
        string PasswordNonce { get; }
        string PfxPassword { get; }
    }
}
