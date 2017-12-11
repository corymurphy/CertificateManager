using CertificateManager.Entities;
using CertificateManager.Entities.Attributes;
using System;

namespace CertificateServices.ActiveDirectory.Entities
{
    [EntitySchemaClass(ActiveDirectorySchemaClass.PkiCertificateTemplate)]
    public class AdcsCertificateTemplate
    {
        private const int CT_FLAG_ENROLLEE_SUPPLIES_SUBJECT = 1;
        private const int CT_FLAG_PEND_ALL_REQUESTS = 2;
        private const int CT_FLAG_EXPORTABLE_KEY = 16;
        private const int CT_FLAG_STRONG_KEY_PROTECTION_REQUIRED = 32;
        private const int CT_FLAG_USE_LEGACY_PROVIDER = 256;
        private const int CT_FLAG_USER_INTERACTION_REQUIRED = 256;

        [DirectoryAttributeMapping("name")]
        public string Name { get; set; }

        [DirectoryAttributeMapping("pKIDefaultCSPs")]
        public string[] CertificateServiceProviders { get; set; }

        [DirectoryAttributeMapping("pKIExtendedKeyUsage")]
        public string ExtendedKeyUsage { get; set; }

        [DirectoryAttributeMapping("msPKI-Minimal-Key-Size")]
        public int MinimumKeySize { get; set; }

        [DirectoryAttributeMapping("objectGuid")]
        public Guid ObjectGuid { get; set; }

        [DirectoryAttributeMapping("msPKI-Certificate-Name-Flag")]
        public int CertificateNameFlag { get; set; }

        [DirectoryAttributeMapping("msPKI-Private-Key-Flag")]
        public int PrivateKeyFlag { get; set; }

        [DirectoryAttributeMapping("msPKI-Enrollment-Flag")]
        public int EnrollmentFlag { get; set; }

        [DirectoryAttributeMapping("msPKI-Template-Schema-Version")]
        public int SchemaVersion { get; set; }

        [DirectoryAttributeMapping("msPKI-RA-Application-Policies")]
        public string ApplicationPolicies { get; set; }

        public CipherAlgorithm Cipher
        {
            get
            {
                if(string.IsNullOrWhiteSpace(this.ApplicationPolicies))
                {
                    return CipherAlgorithm.RSA;
                }

                if(this.ApplicationPolicies.Contains("ECDH"))
                {
                    return CipherAlgorithm.ECDH;
                }

                if (this.ApplicationPolicies.Contains("ECDSA"))
                {
                    return CipherAlgorithm.ECDSA;
                }

                if (this.ApplicationPolicies.Contains("RSA"))
                {
                    return CipherAlgorithm.RSA;
                }

                return CipherAlgorithm.RSA;
            }
        }

        public WindowsApi WindowsApi
        {
            get
            {
                if(SchemaVersion <= 2)
                {
                    return WindowsApi.CryptoApi;
                }
                else
                {
                    return WindowsApi.Cng;
                }
            }
        }

        public bool AllowsClientProvidedSubject()
        {

            if( (this.CertificateNameFlag | CT_FLAG_ENROLLEE_SUPPLIES_SUBJECT) == this.CertificateNameFlag)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool AllowsPrivateKeyExport()
        {
            if ((this.PrivateKeyFlag | CT_FLAG_EXPORTABLE_KEY) == this.PrivateKeyFlag)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool RequiresStrongKeyProtection()
        {
            if ((this.PrivateKeyFlag | CT_FLAG_STRONG_KEY_PROTECTION_REQUIRED) == this.PrivateKeyFlag)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool PendAllRequests()
        {
            if ((this.EnrollmentFlag | CT_FLAG_PEND_ALL_REQUESTS) == this.EnrollmentFlag)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool RequireUserInteraction()
        {
            if ((this.EnrollmentFlag | CT_FLAG_USER_INTERACTION_REQUIRED) == this.EnrollmentFlag)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool LegacySchema()
        {
            if(this.SchemaVersion <= 2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
