using System;

namespace CertificateManager.Entities.Interfaces
{
    public interface ILoggableEntity
    {
        string GetId();
        string GetDescription();
    }
}
