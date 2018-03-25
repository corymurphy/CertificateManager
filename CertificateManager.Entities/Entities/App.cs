using CertificateManager.Entities.Enumerations;
using System;

namespace CertificateManager.Entities.Entities
{
    public class App
    {
        public Guid Id { get; set; }
        public AppType AppType { get; set; }
        
    }
}
