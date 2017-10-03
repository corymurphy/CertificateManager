using System;

namespace CertificateManager.Entities.Exceptions
{
    public class MergeRequiresMergeTargetException : Exception
    {
        public MergeRequiresMergeTargetException(string message) : base(message) { }
    }
}
