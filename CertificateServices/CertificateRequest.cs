using CertificateServices.Interfaces;
using System;
using System.Collections.Generic;

namespace CertificateServices
{
    public class CertificateRequest
    {
        public CertificateRequest
            (
                CertificateSubject subject,
                CipherAlgorithm cipher = CipherAlgorithm.ECDH, 
                int keySize = 384
            )
        {
            CertificateRequestValidation requestValidation = new CertificateRequestValidation();

            if (!requestValidation.IsValidKeySize(cipher, keySize))
                throw new KeySizeUnsupportedException(String.Format("The keysize specified '{0}' is not supported by the specified algorithm '{1}'", keySize, cipher));


            Subject = subject ?? throw new ArgumentNullException(nameof(subject));
            CipherAlgorithm = cipher;
            KeySize = keySize;
            ManagedPrivateKey = true;
            SigningRequestProtocol = SigningRequestProtocol.Pkcs10;

        }

        public CertificateRequest
            (
                CertificateSubject subject, 
                SigningRequestProtocol csrProtocol,
                bool managedPrivateKey
            )
        {
            this.Subject = subject;
            this.SigningRequestProtocol = csrProtocol;
            this.ManagedPrivateKey = managedPrivateKey;
        }

        public string SubjectKeyIdentifier { get; set; }

        public CertificateSubject Subject { get; set; }

        public CipherAlgorithm CipherAlgorithm { get; set; }
  
        public SigningRequestProtocol SigningRequestProtocol { get; set; }

        public int KeySize { get; set; }

        public bool ManagedPrivateKey { get; private set; }

        public string EncodedCsr { get; set; }


        
    }
}
