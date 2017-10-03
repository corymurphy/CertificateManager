using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertificateServices
{
    public class PrivateKeyStorageMetadata
    {
        public string ProviderName { get; set; }
        public string ContainerName { get; set; }

        public uint ProviderType { get; set; }

        public WindowsApi WindowsApi
        {
            get
            {
                if (ProviderType == 0)
                    return WindowsApi.Cng;
                else
                    return WindowsApi.CryptoApi;
            }
        }
    }
}
