using System;
using System.Collections.Generic;

namespace CertificateServices.ActiveDirectory.Entities
{
    [EntitySchemaClass(ActiveDirectorySchemaClass.PkiCertificateTemplate)]
    public class AdcsCertificateTemplate
    {
        [DirectoryAttributeMapping("name")]
        public string Name { get; set; }

        [DirectoryAttributeMapping("pKIDefaultCSPs")]
        public string[] CertificateServiceProviders { get; set; }

        [DirectoryAttributeMapping("pKIExtendedKeyUsage")]
        public string ExtendedKeyUsage { get; set; }

        [DirectoryAttributeMapping("msPKI-Minimal-Key-Size")]
        public string MinimumKeySize { get; set; }

        [DirectoryAttributeMapping("objectGuid")]
        public Guid ObjectGuid { get; set; }
    }
}
