using CertificateManager.Entities.Enumerations;
using System;
using System.Collections.Generic;

namespace CertificateManager.Entities
{
    public class Node
    {
        public string DisplayName { get; set; }
        public Guid Id { get; set; }
        public Guid CredentialId { get; set; }
        public string Hostname { get; set; }
        public CredentialType CredentialType { get; set; }
        public List<Guid> ManagedApps { get; set; }
        public DiscoveryType DiscoveryType { get; set; }
        public DateTime LastCommunication { get; set; }
        public bool CommunicationSuccess { get; set; }
        public List<ManagedCertificate> ManagedCertificates { get; set; }
    }

   
}
