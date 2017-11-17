using CertificateManager.Entities.Enumerations;
using CertificateServices;
using CertificateServices.Enumerations;

namespace CertificateManager.Entities
{
    public class InitialSetupConfigModel
    {

   
        public string DatastoreRootPath { get; set; }
        public string LocalAdminPassword { get; set; }
        public string EmergencyAccessKey { get; set; }
        public string EncryptionKey { get; set; }



        public bool SetupLdap { get; set; }
        public string AdServiceAccountPassword { get; set; }
        public string AdServiceAccountUsername { get; set; }
        public string AdSearchBase { get; set; }
        public string AdServer { get; set; }
        public string AdName { get; set; }
        public bool AdUseAppPoolIdentity { get; set; }
        //public ExternalIdentitySourceType ExternalIdentitySourceType { get; set; }



        public string AdcsServerName { get; set; }
        public string AdcsCommonName { get; set; }
        public HashAlgorithm AdcsHashAlgorithm { get; set; }


        public WindowsApi AdcsTemplateWindowsApi { get; set; }
        public KeyUsage AdcsTemplateKeyUsage { get; set; }
        public string AdcsTemplateName { get; set; }
        public CipherAlgorithm AdcsTemplateCipher { get; set; }

    }
}
