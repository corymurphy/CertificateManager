using System;

namespace CertificateManager.Entities
{
    public class AuditEvent
    {
        public Guid Id { get; set; }
        public string TargetDescription { get; set; }
        public string Target { get; set; }
        public string UserDisplay { get; set; }
        public Guid UserId { get; set; }
        public EventResult EventResult { get; set; }
        public EventCategory EventCategory { get; set; }
        public DateTime Time { get; set; }
    }
}
