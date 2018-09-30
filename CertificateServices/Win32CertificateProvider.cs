using CERTENROLLLib;
using CertificateServices.Exceptions;
using CertificateServices.Interfaces;
using CertificateServices.InteropStructures;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;

namespace CertificateServices
{
    public class Win32CertificateProvider : ICertificateProvider
    {
        #region private fields
        private const int SUCCESS = 0;
        private const int ALG_FLAGS_NONE = 0;
        private const int CC_DEFAULTCONFIG = 0;
        private const int CC_UIPICKCONFIG = 0x1;
        private const int XCN_CRYPT_OID_INFO_PUBKEY_ANY = 0;
        private const uint CERT_KEY_PROV_INFO_PROP_ID = 0x2;
        private const int XCN_NCRYPT_ALLOW_EXPORT_FLAG = 0x1;
        private const int NCRYPT_MACHINE_KEY_FLAG = 0x00000020;
        private const int XCN_CRYPT_PUBKEY_ALG_OID_GROUP_ID = 0x3;
        private const int XCN_NCRYPT_ALLOW_PLAINTEXT_EXPORT_FLAG = 0x2;

        private const string BCRYPT_RSA_ALGORITHM = "RSA";
        private const string BCRYPT_ECDH_P256_ALGORITHM = "ECDH_P256";
        private const string BCRYPT_ECDH_P384_ALGORITHM = "ECDH_P384";
        private const string BCRYPT_ECDH_P521_ALGORITHM = "ECDH_P521";
        private const string BCRYPT_ECDSA_P521_ALGORITHM = "ECDSA_P521";
        private const string BCRYPT_ECDSA_P256_ALGORITHM = "ECDSA_P256";
        private const string BCRYPT_ECDSA_P384_ALGORITHM = "ECDSA_P384";
        private const string GetPrivateKeyUniqueNameProperty = "Unique Name";

        private SecretKeyProvider secret;
        private CertificateRequestValidation requestValidation;
        private ICngNativeProvider cngProvider;
        #endregion region


        #region constructors
        public Win32CertificateProvider()
        {
            secret = new SecretKeyProvider();
            cngProvider = new CngNativeProviderProxy();
            requestValidation = new CertificateRequestValidation();
        }

        public Win32CertificateProvider(ICngNativeProvider cngProvider)
        {
            this.cngProvider = cngProvider ?? throw new ArgumentNullException(nameof(cngProvider));
            secret = new SecretKeyProvider();
            requestValidation = new CertificateRequestValidation();
        }
        #endregion


        #region public methods


        /// <summary>
        /// This method is useful if you generated the csr on another machine and would like to validate/or sign the csr yourself.
        /// If you use this method, the private key will not be returned and it is the responsibility of the caller to re-unite the keypair. 
        /// </summary>
        /// <param name="csr"></param>
        /// <returns></returns>
        public CertificateRequest InitializeFromEncodedCsr(string csr)
        {
            bool decoded = false;
            string cmcMsg = string.Empty;
            string pkcs10Msg = string.Empty;
            CertificateRequest deserializedCsr = null;

            try
            {
                deserializedCsr = DecodePkcs10(csr);
                decoded = true;
            }
            catch (Exception e)
            {
                pkcs10Msg = e.Message;
            }

            if (!decoded)
            {
                try
                {
                    deserializedCsr = DecodeCmc(csr);
                    decoded = true;
                }
                catch (Exception e)
                {
                    cmcMsg = e.Message;
                }
            }

            if (!decoded)
                throw new CryptographicException(BuildCsrDecodeExceptionMessage(pkcs10Msg, cmcMsg));

            return deserializedCsr;
        }

        /// <summary>
        /// Creates a new public and private key pair and the encoded csr is returned back in the CertificateRequest object. 
        /// For security reasons, the private key is kept on the machine that this method is invoked on.
        /// The next step is to have a certificate authority sign the csr and the result should be provided to the InstallIssuedCertificate method.
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="cipher"></param>
        /// <param name="keysize"></param>
        /// <param name="api"></param>
        /// <param name="protocol"></param>
        /// <returns></returns>
        public CertificateRequest CreateCsrKeyPair(CertificateSubject subject, CipherAlgorithm cipher, int keysize, WindowsApi api, SigningRequestProtocol protocol)
        {
            if (!requestValidation.IsValidWindowsApiForCipherAlgorithm(cipher, api))
                throw new AlgorithmNotSupportedByProviderException("The cryptography provider specified does not support the specified cipher algorithm");

            CX509PrivateKey privateKey = CreatePrivateKey(cipher, keysize, api);

            return CreateCsrFromPrivateKey(subject, cipher, keysize, privateKey);
        }

        /// <summary>
        /// Given a certificate, this will return back the Cng private key.
        /// You will have to use this method if the PrivateKey method on X509Certificate2 returns null or throws an exception
        /// if the X509Certificate2.HasPrivateKey property returns true.
        /// This situation indicates that the key is stored using Cng and not CryptoApi.
        /// The reason this happens is that .NET -le 4.7 does not provide an interface for Cng so Win32 interop must be used instead.
        /// The default constructor for this class will instantiate an instance of CngNativeProviderProxy which uses the win32 native apis
        /// For testing, dependency injection can be used to provide a mock of CngNativeProviderProxy via ICngNativeProvider
        /// </summary>
        /// <param name="cert"></param>
        /// <returns></returns>
        public CngKey GetCngKey(X509Certificate2 cert)
        {
            ValidateCertificateArgument(cert);

            IntPtr providerHandle = IntPtr.Zero;
            IntPtr keyHandle = IntPtr.Zero;

            try
            {
                CRYPT_KEY_PROV_INFO keyProvider = GetPrivateKeyProvider(cert);

                providerHandle = OpenKeyVault(keyProvider);

                keyHandle = OpenKey(keyProvider, providerHandle);

                uint keyNameSize = GetKeyNameBufferSize(keyHandle);

                string keyName = GetKeyName(keyHandle, keyNameSize);

                return InitializeCngKey(keyName);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                FreeCngHandles(providerHandle, keyHandle);
            }
        }

        /// <summary>
        /// Given a certificate, this will return information about how the private key is stored.
        /// If the certificate doesn't have a private key, an exception will be thrown.
        /// </summary>
        /// <param name="cert"></param>
        /// <returns></returns>
        public PrivateKeyStorageMetadata GetPrivateKeyStorageMetadata(X509Certificate2 cert)
        {
            ValidateCertificateArgument(cert);

            CRYPT_KEY_PROV_INFO provider = this.GetPrivateKeyProvider(cert);
            return new PrivateKeyStorageMetadata()
            {
                ContainerName = provider.pwszContainerName,
                ProviderName = provider.pwszProvName,
                ProviderType = provider.dwProvType
            };
        }

        /// <summary>
        /// The methods assumes that the csr that is provided was generated on this machine and the private keys were not deleted from the REQUEST store.
        /// The signed certificate is consumed and re-united with the private key and the private key is returned with the X509Certificate2
        /// </summary>
        /// <param name="cert"></param>
        /// <returns></returns>
        public X509Certificate2 InstallIssuedCertificate(string cert)
        {
            if (string.IsNullOrWhiteSpace(cert))
                throw new ArgumentNullException(nameof(cert), "Win32CertificateProvider.InstallIssuedCertificate - signed certificate cannot be null or empty");

            IX509Enrollment enrollment = (IX509Enrollment)Activator.CreateInstance(Type.GetTypeFromProgID("X509Enrollment.CX509Enrollment"));
            try
            {
                enrollment.Initialize(X509CertificateEnrollmentContext.ContextMachine);
                enrollment.InstallResponse(InstallResponseRestrictionFlags.AllowUntrustedRoot, cert, EncodingType.XCN_CRYPT_STRING_BASE64, null);
            }
            catch (Exception e)
            {
                throw new UnableToInstallCertificateToCertificateStoreException(e.Message);
            }

            string pwd = secret.NewSecret(16);
            string pfx = enrollment.CreatePFX(pwd, PFXExportOptions.PFXExportChainWithRoot, EncodingType.XCN_CRYPT_STRING_BASE64);
            return new X509Certificate2(Convert.FromBase64String(pfx), pwd, X509KeyStorageFlags.Exportable | X509KeyStorageFlags.UserKeySet);
        }


        /// <summary>
        /// Creates a self signed certificate given the parameters. 
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="cipher"></param>
        /// <param name="keysize"></param>
        /// <param name="api"></param>
        /// <returns></returns>
        public X509Certificate2 CreateSelfSignedCertificate(CertificateSubject subject, CipherAlgorithm cipher, int keysize, WindowsApi api)
        {
            CX509PrivateKey privateKey = CreatePrivateKey(cipher, keysize);
            CX509CertificateRequestCertificate pkcs10 = NewCertificateRequestCrc(subject, privateKey);
            pkcs10.Issuer = pkcs10.Subject;
            pkcs10.NotBefore = DateTime.Now.AddDays(-1);
            pkcs10.NotAfter = DateTime.Now.AddYears(20);
            var sigoid = new CObjectId();
            var alg = new Oid("SHA256");
            sigoid.InitializeFromValue(alg.Value);
            pkcs10.SignatureInformation.HashAlgorithm = sigoid;
            pkcs10.Encode();

            CX509Enrollment enrollment = new CX509Enrollment();

            enrollment.InitializeFromRequest(pkcs10);

            string csr = enrollment.CreateRequest(EncodingType.XCN_CRYPT_STRING_BASE64);
            InstallResponseRestrictionFlags restrictionFlags = InstallResponseRestrictionFlags.AllowUntrustedCertificate;
            enrollment.InstallResponse(restrictionFlags, csr, EncodingType.XCN_CRYPT_STRING_BASE64, string.Empty);

            string pwd = secret.NewSecret(16);
            string pfx = enrollment.CreatePFX(pwd, PFXExportOptions.PFXExportChainWithRoot, EncodingType.XCN_CRYPT_STRING_BASE64);
            return new X509Certificate2(Convert.FromBase64String(pfx), pwd);
        }

        #endregion


        #region private methods
        
        private string GetDefaultPrivateKeySecurityDescriptor()
        {
            string contextSid = WindowsIdentity.GetCurrent().User.ToString();
            return "D:P(A;;0xd01f01ff;;;CO)(A;;0xd01f01ff;;;SY)(A;;0xd01f01ff;;;BA)(A;;0xd01f01ff;;;" + contextSid + ")";
        }

        private CX509PrivateKey CreatePrivateKey(CipherAlgorithm cipher, int keysize, WindowsApi api = WindowsApi.CryptoApi)
        {
            CX509PrivateKey privateKey = new CX509PrivateKey
            {
                ProviderName = GetWindowsApiProviderName(api)
            };

            switch (api)
            {
                case WindowsApi.Cng:
                    privateKey.KeySpec = X509KeySpec.XCN_AT_NONE;
                    privateKey.LegacyCsp = false;
                    break;
                case WindowsApi.CryptoApi:
                    privateKey.KeySpec = X509KeySpec.XCN_AT_KEYEXCHANGE;
                    break;
                default:
                    throw new AlgorithmNotSupportedByProviderException("Cryptographic provider is not valid");
            }

            privateKey.Algorithm = GetPrivateKeyAlgorithm(cipher, keysize);
            privateKey.Length = keysize;
            privateKey.ExportPolicy = X509PrivateKeyExportFlags.XCN_NCRYPT_ALLOW_EXPORT_FLAG;
            privateKey.KeyUsage = X509PrivateKeyUsageFlags.XCN_NCRYPT_ALLOW_ALL_USAGES;
            privateKey.MachineContext = true;
            privateKey.CspInformations = GetCspInformation(api);
            privateKey.SecurityDescriptor = GetDefaultPrivateKeySecurityDescriptor();
            privateKey.Create();

            return privateKey;
        }

        private CertificateRequest DecodePkcs10(string csr)
        {
            IX509CertificateRequestPkcs10 pkcs10 = (IX509CertificateRequestPkcs10)Activator.CreateInstance(Type.GetTypeFromProgID("X509Enrollment.CX509CertificateRequestPkcs10"));
            pkcs10.InitializeDecode(csr, EncodingType.XCN_CRYPT_STRING_BASE64_ANY);
            CertificateSubject subject = BuildSubject(pkcs10.Subject);
            return ImproveDeserializedCsrFidelity(new CertificateRequest(subject, SigningRequestProtocol.Pkcs10, false), pkcs10.PublicKey);
        }

        private CObjectId GetPrivateKeyAlgorithm(CipherAlgorithm cipher, int keySize)
        {

            CObjectId privateKeyAlgorithm = new CObjectId();


            if (cipher == CipherAlgorithm.ECDH)
            {
                Oid cryptoAlgorithm;
                switch (keySize)
                {
                    case 256:
                        cryptoAlgorithm = new Oid(BCRYPT_ECDH_P256_ALGORITHM);
                        break;
                    case 384:
                        cryptoAlgorithm = new Oid(BCRYPT_ECDH_P384_ALGORITHM);
                        break;
                    case 521:
                        cryptoAlgorithm = new Oid(BCRYPT_ECDH_P521_ALGORITHM);
                        break;
                    default:
                        cryptoAlgorithm = new Oid(BCRYPT_ECDH_P256_ALGORITHM);
                        break;
                }
                privateKeyAlgorithm.InitializeFromValue(cryptoAlgorithm.Value);
                return privateKeyAlgorithm;
            }

            if (cipher == CipherAlgorithm.ECDSA)
            {
                Oid cryptoAlgorithm;
                switch (keySize)
                {
                    case 256:
                        cryptoAlgorithm = new Oid(BCRYPT_ECDSA_P256_ALGORITHM);
                        break;
                    case 384:
                        cryptoAlgorithm = new Oid(BCRYPT_ECDSA_P384_ALGORITHM);
                        break;
                    case 521:
                        cryptoAlgorithm = new Oid(BCRYPT_ECDSA_P521_ALGORITHM);
                        break;
                    default:
                        cryptoAlgorithm = new Oid(BCRYPT_ECDSA_P256_ALGORITHM);
                        break;
                }
                privateKeyAlgorithm.InitializeFromValue(cryptoAlgorithm.Value);
                return privateKeyAlgorithm;
            }

            if (cipher == CipherAlgorithm.RSA)
            {
                privateKeyAlgorithm.InitializeFromAlgorithmName
                    (
                        CERTENROLLLib.ObjectIdGroupId.XCN_CRYPT_PUBKEY_ALG_OID_GROUP_ID,
                        CERTENROLLLib.ObjectIdPublicKeyFlags.XCN_CRYPT_OID_INFO_PUBKEY_ANY,
                        ALG_FLAGS_NONE,
                        "RSA"
                    );
                return privateKeyAlgorithm;
            }

            throw new AlgorithmNotSupportedByProviderException("Unsupported private key cryprographic algorithm specified");

        }

        private CCspInformations GetCspInformation(WindowsApi api)
        {
            CCspInformations cspList = new CCspInformations();
            CCspInformation csp = new CCspInformation();
            csp.InitializeFromName(GetWindowsApiProviderName(api));
            cspList.Add(csp);
            return cspList;
        }

        private CX509Extension GetQualifiedSan(List<string> san)
        {
            CAlternativeNames altNames = new CAlternativeNames();
            CObjectId sanOid = new CObjectId();
            CX509ExtensionAlternativeNames encodedAltNames = new CX509ExtensionAlternativeNames();
            sanOid.InitializeFromValue("2.5.29.17");

            foreach (string strAltName in san)
            {
                CAlternativeName altName = (CAlternativeName)Activator.CreateInstance(Type.GetTypeFromProgID("X509Enrollment.CAlternativeName"));
                altName.InitializeFromString(AlternativeNameType.XCN_CERT_ALT_NAME_DNS_NAME, strAltName);
                altNames.Add(altName);
                altName = null;
            }
            encodedAltNames.InitializeEncode(altNames);
            return (CX509Extension)encodedAltNames;
        }

        private CX500DistinguishedName GetEncodedSubject(CertificateSubject subject)
        {
            CX500DistinguishedName certDn = new CX500DistinguishedName();
            certDn.Encode(subject.ToString(), X500NameFlags.XCN_CERT_NAME_STR_NONE);
            return certDn;
        }

        private CX509Extension GetKeyUsage()
        {
            CX509ExtensionKeyUsage keyUsage = new CX509ExtensionKeyUsage();
            keyUsage.InitializeEncode(
                CERTENROLLLib.X509KeyUsageFlags.XCN_CERT_DIGITAL_SIGNATURE_KEY_USAGE |
                CERTENROLLLib.X509KeyUsageFlags.XCN_CERT_NON_REPUDIATION_KEY_USAGE |
                CERTENROLLLib.X509KeyUsageFlags.XCN_CERT_KEY_ENCIPHERMENT_KEY_USAGE |
                CERTENROLLLib.X509KeyUsageFlags.XCN_CERT_DATA_ENCIPHERMENT_KEY_USAGE
            );
            return (CX509Extension)keyUsage;
        }

        private CipherAlgorithm GetCipherFromOid(string oid)
        {
            //1.2.840.113549.1.1
            //1.2.840.113549.1.1.1

            if (oid == "1.2.840.113549.1.1.1")
                return CipherAlgorithm.RSA;

            //if (oid == "1.2.840.10045.2.1")
            //    return CipherAlgorithm.ECC;

            throw new UnsupportedCipherAlgorithmException(string.Format("oid {0} is not a know pkcs public key encipherment algorithm", oid));
        }

        private string BuildCsrDecodeExceptionMessage(string pkcs10Msg, string cmcMsg)
        {
            return string.Format
                    (
                        @"  
                            Unable to determine the correct type of certificate signing request. Pkcs10 and Cmc decoding were attempted. Review failure results below
                            
                            Pkcs10: {0}
                            Cmc: {1}
                        ",
                        pkcs10Msg, cmcMsg
                    );
        }

        private string GetWindowsApiProviderName(WindowsApi api)
        {
            if (api == WindowsApi.Cng)
                return "Microsoft Software Key Storage Provider";

            if (api == WindowsApi.CryptoApi)
                return "Microsoft Enhanced RSA and AES Cryptographic Provider";

            throw new Exception("Unknown cryptographic provider");
        }

        private string ComputeSubjectKeyIdentifier(CX509PublicKey publicKey)
        {
            if (publicKey == null)
                throw new ArgumentNullException("Win32CertificateProvider.ComputeSubjectKeyIdentifier - null public key was provided");

            return publicKey.ComputeKeyIdentifier(KeyIdentifierHashAlgorithm.SKIHashSha1, EncodingType.XCN_CRYPT_STRING_HEX).
                        Trim().Replace(" ", "").Replace(Environment.NewLine, "").Trim();
        }

        private CertificateSubject BuildSubject(CX500DistinguishedName subject)
        {
            if (subject == null)
                throw new CryptographicException("Win32CertificateProvider.BuildSubject - Subject from a decoded request cannot be null");
            return CertificateSubject.CreateFromDistinguishedName(subject.Name);
        }

        private CertificateRequest DecodeCmc(string csr)
        {
            IX509CertificateRequestCertificate cmc = (IX509CertificateRequestCertificate)Activator.CreateInstance(Type.GetTypeFromProgID("X509Enrollment.CX509CertificateRequestCertificate"));
            cmc.InitializeDecode(csr, EncodingType.XCN_CRYPT_STRING_BASE64_ANY);
            CertificateSubject subject = BuildSubject(cmc.Subject);
            return ImproveDeserializedCsrFidelity(new CertificateRequest(subject, SigningRequestProtocol.Pkcs10, false), cmc.PublicKey);
        }

        private CertificateRequest ImproveDeserializedCsrFidelity(CertificateRequest csr, CX509PublicKey publicKey)
        {
            csr.SubjectKeyIdentifier = ComputeSubjectKeyIdentifier(publicKey);
            csr.KeySize = publicKey.Length;
            csr.CipherAlgorithm = GetCipherFromOid(publicKey.Algorithm.Value);
            return csr;
        }

        private CX509CertificateRequestPkcs10 NewCertificateRequestPkcs10(CertificateSubject subject, CX509PrivateKey privateKey)
        {
            CX509CertificateRequestPkcs10 pkcs10 = new CX509CertificateRequestPkcs10();

            pkcs10.InitializeFromPrivateKey(X509CertificateEnrollmentContext.ContextMachine, privateKey, "");

            if (subject.ContainsSubjectAlternativeName)
            {
                pkcs10.X509Extensions.Add(GetQualifiedSan(subject.SubjectAlternativeName));
            }

            pkcs10.X509Extensions.Add(GetKeyUsage());

            pkcs10.Subject = GetEncodedSubject(subject);

            return pkcs10;
        }

        private CX509CertificateRequestCertificate NewCertificateRequestCrc(CertificateSubject subject, CX509PrivateKey privateKey)
        {
            CX509CertificateRequestCertificate crc = new CX509CertificateRequestCertificate();

            crc.InitializeFromPrivateKey(X509CertificateEnrollmentContext.ContextMachine, privateKey, "");

            crc.X509Extensions.Add(GetQualifiedSan(subject.SubjectAlternativeName));

            crc.X509Extensions.Add(GetKeyUsage());

            crc.Subject = GetEncodedSubject(subject);



            return crc;
        }

        private string BuildEncodedCsr(CX509CertificateRequestPkcs10 pkcs10)
        {
            CX509Enrollment enrollment = new CX509Enrollment();
            enrollment.InitializeFromRequest(pkcs10);
            return enrollment.CreateRequest(EncodingType.XCN_CRYPT_STRING_BASE64);
        }

        private CertificateRequest CreateCsrFromPrivateKey(CertificateSubject subject, CipherAlgorithm cipher, int keysize, CX509PrivateKey privateKey)
        {

            CertificateRequest csr = new CertificateRequest(subject, cipher, keysize);

            CX509CertificateRequestPkcs10 pkcs10 = NewCertificateRequestPkcs10(csr.Subject, privateKey);

            csr.SubjectKeyIdentifier = GetSubjectKeyIdentifier(pkcs10);

            csr.EncodedCsr = BuildEncodedCsr(pkcs10);

            return csr;
        }

        private string GetSubjectKeyIdentifier(CX509CertificateRequestPkcs10 crc)
        {
            return crc.PublicKey.ComputeKeyIdentifier(KeyIdentifierHashAlgorithm.SKIHashSha1, EncodingType.XCN_CRYPT_STRING_HEX).
                        Trim().Replace(" ", "").Replace(System.Environment.NewLine, "").Trim();
        }

        /// <summary>
        /// After obtaining the unmanaged data about a private key, we must create a managed structure.
        /// </summary>
        /// <param name="providerDataPtr"></param>
        /// <returns></returns>
        private CRYPT_KEY_PROV_INFO GetPrivateKeyProviderStructructureFromPointer(IntPtr providerDataPtr)
        {

            try
            {
                return (CRYPT_KEY_PROV_INFO)Marshal.PtrToStructure(providerDataPtr, typeof(CRYPT_KEY_PROV_INFO));
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Unable to allocate memory for key provider metadata - {0}", e.Message));
            }
            finally
            {
                Marshal.FreeHGlobal(providerDataPtr);
            }
        }

        /// <summary>
        /// Gets information about how the private key is stored. 
        /// This will determine if the key is stored using 
        /// CryptoApi or CryptoApiNextGen (Cng)
        /// 
        /// CERT_KEY_PROV_INFO_PROP_ID - Indicates that you want to recieve provder information for a given certificate handle
        /// </summary>
        /// <param name="cert"></param>
        private CRYPT_KEY_PROV_INFO GetPrivateKeyProvider(X509Certificate2 cert)
        {
            int providerDataSize = GetPrivateKeyProviderBufferSize(cert);

            IntPtr providerDataPtr = Marshal.AllocHGlobal(providerDataSize);

            bool result = cngProvider.CertGetCertificateContextProperty(cert.Handle, CERT_KEY_PROV_INFO_PROP_ID, providerDataPtr, ref providerDataSize);

            if (result)
            {
                return GetPrivateKeyProviderStructructureFromPointer(providerDataPtr);
            }
            else
            {
                Marshal.FreeHGlobal(providerDataPtr);
                throw new Exception("The keyset exists, but there was an error while retrieving the data");
            }
        }

        /// <summary>
        /// Before we can obtain information about a private key, we must allocate memory to store the data.
        /// This method will determine how large of a buffer to allocate
        /// </summary>
        /// <param name="cert"></param>
        /// <returns></returns>
        private int GetPrivateKeyProviderBufferSize(X509Certificate2 cert)
        {
            int providerDataSize = 0;

            bool result = cngProvider.CertGetCertificateContextProperty(cert.Handle, CERT_KEY_PROV_INFO_PROP_ID, IntPtr.Zero, ref providerDataSize);

            if (result)
            {
                return providerDataSize;
            }
            else
            {
                throw new Exception("Could not retrieve data about the private keys provider given the certificate. This might indicate that the current context does not have access to the private key");

            }
        }

        /// <summary>
        /// Opens the specified key provider and returns back a handle to the opened provider
        /// </summary>
        /// <param name="keyProvider"></param>
        /// <returns></returns>
        private IntPtr OpenKeyVault(CRYPT_KEY_PROV_INFO keyProvider)
        {
            int result = int.MaxValue;
            IntPtr providerHandle = IntPtr.Zero;

            result = cngProvider.NCryptOpenStorageProvider(ref providerHandle, keyProvider.pwszProvName, 0);

            switch (result)
            {
                case SUCCESS:
                    return providerHandle;
                default:
                    throw new Exception(string.Format("Unable to open provider - {0}", result));
            }
        }

        /// <summary>
        /// Opens a private key and returns a handle to the opened key.
        /// </summary>
        /// <param name="keyProv"></param>
        /// <param name="providerHandle"></param>
        /// <returns></returns>
        private IntPtr OpenKey(CRYPT_KEY_PROV_INFO keyProv, IntPtr providerHandle)
        {

            IntPtr keyHandle = IntPtr.Zero;

            int result = cngProvider.NCryptOpenKey(providerHandle, out keyHandle, keyProv.pwszContainerName, 0, NCRYPT_MACHINE_KEY_FLAG);

            switch (result)
            {
                case SUCCESS:
                    return keyHandle;
                default:
                    throw new Exception(string.Format("Unable to open key - {0}", result));
            }

        }

        private uint GetKeyNameBufferSize(IntPtr keyHandle)
        {
            uint keyNameSize = 0;

            int result = cngProvider.NCryptGetProperty(keyHandle, GetPrivateKeyUniqueNameProperty, null, 0, ref keyNameSize, 0);

            switch (result)
            {
                case SUCCESS:
                    return keyNameSize;
                default:
                    throw new Exception(string.Format("Unable to open key - {0}", result));
            }
        }

        private string GetKeyName(byte[] keyName)
        {
            return System.Text.Encoding.Unicode.GetString(keyName);
        }

        private string GetKeyName(IntPtr keyHandle, uint keyNameSize)
        {
            byte[] keyName = new byte[keyNameSize];

            int result = cngProvider.NCryptGetProperty(keyHandle, GetPrivateKeyUniqueNameProperty, keyName, keyName.Length, ref keyNameSize, 0);

            switch (result)
            {
                case SUCCESS:
                    return GetKeyName(keyName);
                default:
                    throw new Exception(string.Format("Unable to open key - {0}", result));
            }
        }

        private CngKey InitializeCngKey(string keyName)
        {
            return CngKey.Open(keyName, CngProvider.MicrosoftSoftwareKeyStorageProvider, CngKeyOpenOptions.MachineKey);
        }

        private void FreeCngHandles(IntPtr providerHandle, IntPtr keyHandle)
        {
            if (keyHandle != IntPtr.Zero)
                cngProvider.NCryptFreeObject(keyHandle);

            if (providerHandle != IntPtr.Zero)
                cngProvider.NCryptFreeObject(providerHandle);
        }

        private void ValidateCertificateArgument(X509Certificate2 cert)
        {
            if (cert == null) { throw new ArgumentNullException(nameof(cert)); }
            if (!cert.HasPrivateKey) { throw new PrivateKeyDoesNotExistException("The provided certificate does not contain a private key stored on this machine context."); }
        }

        #endregion
    }
}