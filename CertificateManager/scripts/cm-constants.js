var CmOptions = {
    hashAlgorithmOptions: [
        { Name: "SHA1", Id: 0, Display: "SHA1 (Insecure)" },
        { Name: "SHA256", Id: 1, Display: "SHA256 (Recommended)", Primary: true },
        { Name: "SHA512", Id: 2, Display: "SHA512 (Most Secure)" }
    ],
    cipherOptions: [
        { Name: "RSA", Id: 0, Display: "RSA (TLS / More Support)", Primary: true },
        { Name: "ECDH", Id: 1, Display: "ECDH (TLS / Most Secure)" },
        { Name: "ECDSA", Id: 2, Display: "ECDSA (Uncommon)" }
    ],

    keyUsageOptions: [
        { Name: "None", Id: 0, Primitive: true, Display: "None" },
        { Name: "ServerAuthentication", Id: 1, Primitive: true, Display: "ServerAuthentication", Primary: true },
        { Name: "ClientAuthentication", Id: 2, Primitive: true, Display: "ClientAuthentication" },
        { Name: "ServerAuthentication, ClientAuthentication", Id: 3, Primitive: false, Display: "ServerAuthentication, ClientAuthentication" },
        { Name: "DocumentEncryption", Id: 4, Primitive: true, Display: "DocumentEncryption" },
        { Name: "CodeSigning", Id: 8, Primitive: true, Display: "CodeSigning" },
        { Name: "CertificateAuthority", Id: 16, Primitive: true, Display: "CertificateAuthority" },
        { Name: "Undetermined", Id: 32, Primitive: true, Display: "Undetermined" }

    ],
    windowsApiOptions: [
        { Name: "Cng", Id: 1, Display: "CryptoApi Next Generation (Most Secure)", Primary: true },
        { Name: "CryptoApi", Id: 0, Display: "CryptoApi (More Support)" }
    ],
    //windowsApiOptions: ["Cng", "CryptoApi"],
    authenticationTypeOptions: [
        { Name: "UsernamePassword", Id: 0, Display: "basic", Primary: true },
        { Name: "WindowsKerberos", Id: 1, Display: "kerberos" }
    ],
    ExternalIdentitySourceType: [
        { Name: "ActiveDirectoryIwa", Id: 0, Display: "ActiveDirectoryIwa" },
        { Name: "ActiveDirectoryBasic", Id: 1, Display: "ActiveDirectoryBasic" },
    ],

    LocalIdentityProviderId: "02abeb4c-e0b6-4231-b836-268aa40c3f1c"
}