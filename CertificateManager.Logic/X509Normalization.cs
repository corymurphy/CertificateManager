using CertificateServices.Enumerations;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace CertificateManager.Logic
{
    public class X509Normalization
    {
        DataTransformation dataTransformation = new DataTransformation();

        public KeyUsage GetKeyUsage(X509Certificate2 cert)
        {
            if (cert == null || cert.Handle == IntPtr.Zero)
                throw new ArgumentNullException(nameof(cert));

            if (cert.Extensions == null)
                return KeyUsage.None;

            KeyUsage keyUsage = KeyUsage.None;

            foreach (var extension in cert.Extensions)
            {
                X509EnhancedKeyUsageExtension ekus = extension as X509EnhancedKeyUsageExtension;

                if (ekus != null)
                {
                    foreach(var eku in ekus.EnhancedKeyUsages)
                    {
                        if(EnhancedKeyUsage.ContainsKey(eku.Value))
                        {
                            keyUsage = SetFlag(keyUsage, EnhancedKeyUsage[eku.Value]);
                        }
                    }
                }

                ekus = null;


                X509BasicConstraintsExtension basicConstraintsExtension = extension as X509BasicConstraintsExtension;

                if(basicConstraintsExtension != null)
                {
                    if(basicConstraintsExtension.CertificateAuthority == true)
                    {
                        keyUsage = SetFlag(keyUsage, KeyUsage.CertificateAuthority);
                    }
                }

                basicConstraintsExtension = null;
            }

            return keyUsage;




            
        }

        private KeyUsage SetFlag(KeyUsage current, KeyUsage additional)
        {

            if (current.HasFlag(KeyUsage.None))
            {
                current = current & ~KeyUsage.None;
            }

            return current | additional;
        }

        private X509EnhancedKeyUsageExtension GetEnhancedKeyUsageExtension(X509Certificate2 cert)
        {
            foreach (var extension in cert.Extensions)
            {
                X509EnhancedKeyUsageExtension eku = extension as X509EnhancedKeyUsageExtension;

                if (eku != null)
                {

                }
            }

            throw new Exception("Certificate does not contain Enhanced Key Usage Extension");
        }

        private Dictionary<string, KeyUsage> EnhancedKeyUsage = new Dictionary<string, KeyUsage>()
        {
            { "1.3.6.1.5.5.7.3.1" , KeyUsage.ServerAuthentication },
            { "1.3.6.1.5.5.7.3.2" , KeyUsage.ClientAuthentication },
            { "1.3.6.1.4.1.311.80.1" , KeyUsage.DocumentEncryption },
            { "1.3.6.1.5.5.7.3.3" , KeyUsage.CodeSigning }
            
            //Certificate Authority Certificate is determined by Basic Attributes
            //{ "1.3.6.1.5.5.7.3.2" , KeyUsage.CertificateAuthority },

        };
    }
}
