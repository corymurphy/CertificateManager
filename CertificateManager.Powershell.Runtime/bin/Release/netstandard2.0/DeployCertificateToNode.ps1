function DeployCertificateToNode
{
    param
    (
        [pscredential]$Credential,
        [string]$ComputerName,
        [string]$CertificateContent,
        [string]$CertificateKey
    )

    try
    {
        $session = New-PSSession -ComputerName $ComputerName -Credential $Credential -ErrorAction Stop;

        $sb = {
            param($CertificateContent, $CertificateKey)
    
            $personal = [System.Security.Cryptography.X509Certificates.StoreName]::My;
            $storeLoc = [System.Security.Cryptography.X509Certificates.StoreLocation]::LocalMachine;
    
            $store = New-Object -TypeName 'System.Security.Cryptography.X509Certificates.X509Store' -ArgumentList $personal,$storeLoc;
    
            $access = [System.Security.Cryptography.X509Certificates.OpenFlags]::ReadWrite;
    
            $store.Open($access);
    
            $storageFlags = [System.Security.Cryptography.X509Certificates.X509KeyStorageFlags]::PersistKeySet -bor [System.Security.Cryptography.X509Certificates.X509KeyStorageFlags]::MachineKeySet;
            $content = [System.Convert]::FromBase64String($CertificateContent);
            $certArgs = @( $content,$CertificateKey,$storageFlags );
            $cert = New-Object -TypeName 'System.Security.Cryptography.X509Certificates.X509Certificate2' -ArgumentList $certArgs;
    
            $store.Add($cert);
        }
    
        Invoke-Command -ScriptBlock $sb -Session $session -ArgumentList @($CertificateContent, $CertificateKey) -ErrorAction Stop | Out-Null;
    }
    catch
    {
        throw $Error;
    }

}