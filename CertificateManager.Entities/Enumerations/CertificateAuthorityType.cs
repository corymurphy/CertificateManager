using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertificateManager.Entities.Enumerations
{
    /// <summary>
    /// The certificate authroity type defines what kind of CA the certificate was issued by.
    /// Public - This is widely trusted public CA like DigiCert, GeoTrust, Symantec, etc
    /// Public certificate authorities usually charge for each certificate
    /// Private - This is a self hosted certificate authority that an organization might own.
    /// Private certificate authorities might be Microsoft Active Directory Certificate Services, 
    /// and generally do not cost money to issue certificates.
    /// </summary>
    public enum CertificateAuthorityType { Private, Public }
}
