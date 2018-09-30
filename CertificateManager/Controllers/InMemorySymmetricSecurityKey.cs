namespace CertificateManager.Controllers
{
    internal class InMemorySymmetricSecurityKey
    {
        private object buffer;

        public InMemorySymmetricSecurityKey(object buffer)
        {
            this.buffer = buffer;
        }
    }
}