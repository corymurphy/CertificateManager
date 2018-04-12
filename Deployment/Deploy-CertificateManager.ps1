param( [string]$ComputerName, [PSCredential]$Credential )

function Get-SetCertificateManagerAclScript
{

    $script = {
        param
        (
            [String]$InstallPath
        )

        if(-not (Test-Path -Path:$InstallPath))
        {
            throw 'Install path not found';
        }       

        $inheritance = ([System.Security.AccessControl.InheritanceFlags]::ContainerInherit -bor [System.Security.AccessControl.InheritanceFlags]::ObjectInherit);

        $systemFullControlAceArgs = @(
            (New-Object -TypeName:'System.Security.Principal.NTAccount' -ArgumentList:@('NT AUTHORITY\SYSTEM')),
            [System.Security.AccessControl.FileSystemRights]::FullControl,
            $inheritance,
            [System.Security.AccessControl.PropagationFlags]::None,
            [System.Security.AccessControl.AccessControlType]::Allow
        )
        $systemFullControlAce = New-Object -TypeName:'System.Security.AccessControl.FileSystemAccessRule' -ArgumentList:$systemFullControlAceArgs

        $adminFullControlAceArgs = @(
            (New-Object -TypeName:'System.Security.Principal.NTAccount' -ArgumentList:@('BUILTIN\Administrators')),
            [System.Security.AccessControl.FileSystemRights]::FullControl,
            $inheritance,
            [System.Security.AccessControl.PropagationFlags]::None,
            [System.Security.AccessControl.AccessControlType]::Allow
        )
        $adminFullControlAce = New-Object -TypeName:'System.Security.AccessControl.FileSystemAccessRule' -ArgumentList:$adminFullControlAceArgs

        $appPoolReadExecuteAceArgs = @(
            (New-Object -TypeName:'System.Security.Principal.NTAccount' -ArgumentList:@('IIS APPPOOL\CertificateManager')),
            [System.Security.AccessControl.FileSystemRights]::ReadAndExecute,
            $inheritance,
            [System.Security.AccessControl.PropagationFlags]::None,
            [System.Security.AccessControl.AccessControlType]::Allow
        )
        $appPoolReadExecuteAce = New-Object -TypeName:'System.Security.AccessControl.FileSystemAccessRule' -ArgumentList:$appPoolReadExecuteAceArgs


        $acl = [System.Security.AccessControl.DirectorySecurity]::new();
        
        $acl.SetOwner( (New-Object -TypeName:'System.Security.Principal.NTAccount' -ArgumentList:@('BUILTIN\Administrators')) );

        $acl.AddAccessRule($adminFullControlAceArgs);
        $acl.AddAccessRule($systemFullControlAceArgs);
        $acl.AddAccessRule($appPoolReadExecuteAce);

        Set-Acl -Path:$InstallPath -AclObject:$acl;

    }
    return $script;
}


function New-CertificateManagerIISConfiguration
{
    param
    (
        $InstallPath,
        $HostName,
        $Thumbprint
    )
    $start = Get-Location;

    Set-Location $env:TEMP | Out-Null;

    $result = CertificateManagerIISConfiguration -InstallPath:$InstallPath -HostName:$HostName -Thumbprint:$Thumbprint;

    Set-Location $start | Out-Null;

    return $result;
}


function Install-CertificateManagerRequiredModuled
{
    param($Session)

    $moduleSourcePath = [System.IO.Path]::Combine($env:TEMP, 'xWebAdministration');

    Save-Module -Name 'xWebAdministration' -Path $env:TEMP -Force

    Copy-Item -Path:$moduleSourcePath -Destination:'C:\Program Files\WindowsPowerShell\Modules' -ToSession:$Session -Force -Recurse;

    Remove-Item -Path:$moduleSourcePath -Force -Recurse -Confirm:$false;
}


configuration CertificateManagerIISConfiguration
{
    param
    (
        [String]$InstallPath,
        [string]$HostName,
        [string]$Thumbprint,
        [string]$NodeName
    )

    Import-DscResource -ModuleName 'PSDesiredStateConfiguration';
    Import-DscResource -ModuleName 'xWebAdministration';

    WindowsFeature IIS {
        Ensure = "Present"
        Name = "Web-Server"
    }

    $iisFeatures = @( "Web-Mgmt-Tools", "Web-Static-Content", "Web-Http-Errors", "Web-Stat-Compression", "Web-Windows-Auth" );


    foreach($feature in $iisFeatures)
    {
        WindowsFeature $feature
        {
            Ensure = "Present"
            Name = $feature
            DependsOn='[WindowsFeature]IIS'
        }
    }

    xWebAppPool CertMgrAppPool
    {
        Name = 'CertificateManager'
    }

    xWebsite NewWebsite
    {
        Ensure          = "Present"
        Name            = 'CertificateManager'
        State           = "Started"
        PhysicalPath    = $InstallPath
        BindingInfo     = @( MSFT_xWebBindingInformation
                             {
                               Protocol              = "https"
                               Port                  = 443
                               CertificateThumbprint = $Thumbprint
                               CertificateStoreName  = "My"
                               HostName = $HostName
                             }
                         )
        ApplicationPool = 'CertificateManager'
        #DependsOn       = "[File]WebContent"
    }
}

function Deploy-CertificateManager
{
    param
    (
        [Parameter(mandatory=$true)][ValidateNotNullOrEmpty()][string]$ComputerName,
        [Parameter(mandatory=$true)][PSCredential]$Credential,
        [Parameter(mandatory=$false)][string]$WebsiteHostname = 'certificatemanager.local',
        [Parameter(mandatory=$false)][ValidateNotNullOrEmpty()][string]$InstallPath = 'c:\Program Files\CertificateManager',
        [Parameter(mandatory=$false)][ValidateNotNullOrEmpty()][string]$PackageUri = 'https://github.com/corymurphy/CertificateManager/blob/master/Deployment/CertificateManagerWebApp.nupkg?raw=true'
    )

    begin
    {
        [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12;
        $packageName = 'CertificateManagerWebApp.nupkg';
        $packageZipName = 'CertificateManagerWebApp.zip';
        $packagePath = [System.IO.Path]::Combine($env:TEMP, $packageName);
        $serverTempDirectory = 'c:\windows\temp';
        $ConfigurationName = 'CertificateManagerIISConfiguration';
        $serverConfigPath = ([IO.Path]::Combine($serverTempDirectory, $ConfigurationName));

        $downloadArgs = @{
            Uri = $PackageUri;
            OutFile = $packagePath;
        };

        $newCertScript = {
            param($HostName)
            New-SelfSignedCertificate -DnsName:$HostName;
        }

        Install-Module -Name 'xWebAdministration' -Scope CurrentUser -ErrorAction:Stop;

        $setupScript = { 
            param
            (
                $PackageName, 
                $PackageZipName, 
                $ServerTempDirectory, 
                $InstallPath, 
                $ConfigurationPath, 
                $WebsiteHostname,
                $AclConfigScript
            )

            $PackagePath = [IO.Path]::Combine($ServerTempDirectory, $PackageName);
            $PackageZipPath = [IO.Path]::Combine($ServerTempDirectory, $PackageZipName);

            Remove-Item -Path $PackageZipPath -Force;
            Rename-Item -Path $PackagePath -NewName $PackageZipName -Force;
            Expand-Archive -Path $PackageZipPath -DestinationPath $InstallPath -Force;

            $job = Start-DscConfiguration $ConfigurationPath;

            $StopWatch = New-Object -TypeName System.Diagnostics.Stopwatch

            while( ($job | Get-Job).State -eq 'Running' )
            {
                Start-Sleep 1;

                if($StopWatch.Elapsed.TotalSeconds -gt 300)
                {
                    throw 'IIS Configuration did not complete within 5 minutes. Please rerun the deployment.'
                }
            }

            if( ($job | Get-Job).State -ne 'Completed' )
            {
                $result = $job | Receive-Job;
                throw 'IIS Configuration failed {0}{1}' -f [System.Environment]::NewLine,$result;
            }

            Import-Module -Name 'WebAdministration' -ErrorAction 'Stop';

            Set-ItemProperty -Path 'IIS:\AppPools\CertificateManager' -Name 'managedRuntimeVersion' -Value [string]::Empty -Force;


            Invoke-Command -ScriptBlock:$AclConfigScript -ArgumentList:@($InstallPath);
        }

        $createDscFolderScript = { 
            param($serverTempDirectory, $configurationname) 
            New-Item -Path $serverTempDirectory -Name $configurationname -ItemType Directory -Force | Out-Null;
        }
    }
    
    process
    {
        $session = New-PSSession -ComputerName $ComputerName -Credential $Credential -ErrorAction 'Stop';

        $cert = Invoke-Command -ScriptBlock:$newCertScript -ArgumentList @($WebsiteHostname) -Session:$session;

        $configurationPath = New-CertificateManagerIISConfiguration -InstallPath:$InstallPath -HostName:$WebsiteHostname -Thumbprint:$cert.Thumbprint;

        Install-CertificateManagerRequiredModuled -Session:$Session;

        Invoke-WebRequest @downloadArgs | Out-Null;

        Copy-Item -ToSession $session -Path $packagePath -Destination $serverTempDirectory -Force;

        Invoke-Command -ScriptBlock $createDscFolderScript -ArgumentList @($serverTempDirectory, $ConfigurationName) -Session $session;

        Copy-Item -ToSession $session -Path $configurationPath.FullName -Destination ([IO.Path]::Combine($ServerTempDirectory, $configurationname)) -Force;
        
        $aclConfigScript = Get-SetCertificateManagerAclScript;

        $setupArgs = @(
            $PackageName,
            $PackageZipName,
            $ServerTempDirectory,
            $InstallPath,
            $serverConfigPath,
            $WebsiteHostname,
            $aclConfigScript
        );
        
        Invoke-Command -ScriptBlock $setupScript -ArgumentList $setupArgs -Session $session;
    }
}


Deploy-CertificateManager -ComputerName:$ComputerName -Credential:$Credential;