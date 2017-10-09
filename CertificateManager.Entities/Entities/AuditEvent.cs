using System;

namespace CertificateManager.Entities
{
    public class AuditEvent
    {
        public string Target { get; set; }
        public string UserPrincipalName { get; set; }
        public Guid UserId { get; set; }
        public EventResult EventResult { get; set; }
        public EventCategory EventCategory { get; set; }
        public DateTime Time { get; set; }
    }
}
