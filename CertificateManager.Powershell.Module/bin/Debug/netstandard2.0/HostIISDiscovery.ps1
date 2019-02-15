function HostIISDiscovery
{
    param
    (
        [pscredential]$Credential,
        [string]$ComputerName
    )

    try
    {
        $entityLib = Get-ChildItem -Path bin -Filter 'CertificateManager.Powershell.Runtime.dll' -Recurse | Select -First 1;
        Add-Type -Path $entityLib.FullName -ErrorAction Stop | Out-Null;
    
        $session = New-PSSession -ComputerName $ComputerName -Credential $Credential -ErrorAction Stop;
    
        $sb = {

            Import-Module -Name 'WebAdministration';
            $sslBindings = Get-ChildItem -Path 'IIS:\SslBindings' -Recurse | Select *;
    
            return $sslBindings;
        }
    
        $results = Invoke-Command -ScriptBlock $sb -Session $session -ErrorAction Stop;
    
    
        $list = New-Object 'System.Collections.Generic.List[CertificateManager.Powershell.Runtime.HostIISCertificateEntity]';
    
        foreach($cert in $results)
        {
            $entity = New-Object 'CertificateManager.Powershell.Runtime.HostIISCertificateEntity';
            $entity.Thumbprint = $cert.Thumbprint;
            $entity.ApplicationId = $cert.ApplicationId
            $entity.Path = $cert.PSPath;

            $certPath = "Cert:\LocalMachine\{0}\{1}" -f $cert.Store,$cert.Thumbprint;

            $resolvedCert = Get-Item -Path $certPath;
            
            $exportType = [System.Security.Cryptography.X509Certificates.X509ContentType]::Cert;

            $entity.X509Content = [System.Convert]::ToBase64String($resolvedCert.Export($exportType));

            $list.Add($entity);
        }
    
        return $list;
    }
    catch
    {
        throw $Error;
    }
}