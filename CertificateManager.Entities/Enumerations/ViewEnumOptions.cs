using CertificateServices;
using CertificateServices.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CertificateManager.Entities.Enumerations
{
    public class ViewEnumOptions
    {
        public string[] HashAlgorithOptions { get { return GetHashAlgorithmOptions();  } }
        public string[] CipherAlgorithmOptions { get { return GetCipherAlgorithmOptions(); } }
        public string[] CertificateAuthorityAuthenticationTypeOptions { get { return GetCertificateAuthorityAuthenticationTypeOptions(); } }
        public string[] WindowsApiOptions { get { return GetWindowsApiOptions(); } }
        public string[] KeyUsageOptions { get { return GetKeyUsageOptions(); } }


        private string[] GetHashAlgorithmOptions()
        {
            return Enum.GetNames(typeof(HashAlgorithm)).
                Where(e => e == HashAlgorithm.SHA1.ToString() || e == HashAlgorithm.SHA256.ToString()).ToArray();
        }

        private string[] GetCipherAlgorithmOptions()
        {
            return Enum.GetNames(typeof(CipherAlgorithm));
        }

        private string[] GetCertificateAuthorityAuthenticationTypeOptions()
        {
            return Enum.GetNames(typeof(MicrosoftCertificateAuthorityAuthenticationType));
        }

        private string[] GetWindowsApiOptions()
        {
            return Enum.GetNames(typeof(WindowsApi));
        }

        private string[] GetKeyUsageOptions()
        {
            List<string> options = new List<string>();
            options.Add("None");
            options.Add("ServerAuthentication");
            options.Add("ClientAuthentication");
            options.Add("ServerAuthentication, ClientAuthentication");
            options.Add("DocumentEncryption");
            options.Add("CodeSigning");
            options.Add("CertificateAuthority");
            return options.ToArray();
        }
    }
}
