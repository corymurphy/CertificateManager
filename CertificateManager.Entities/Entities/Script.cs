using CertificateManager.Entities.Attributes;
using System;

namespace CertificateManager.Entities
{
    [Repository("script")]
    public class Script
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
