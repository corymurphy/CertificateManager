
namespace CertificateServices
{
    public class AccessDeniedLocalMachineCertificateStore : System.Exception
    {
        public AccessDeniedLocalMachineCertificateStore(string message) : base(message) { }
    }
}