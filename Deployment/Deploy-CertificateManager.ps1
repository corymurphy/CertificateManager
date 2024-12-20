param( [string]$ComputerName, [PSCredential]$Credential )

function Initialize-LocalRequiredModules
{
    param
    (
        [System.Collections.Generic.List[string]]$Modules
    )
    Set-PSRepository -Name 'PSGallery' -InstallationPolicy 'Trusted';

    foreach($module in $Modules)
    {
        New-Variable -Name:'count' -Value:0 -Force;

        $count = Get-Module -Name:$module -ListAvailable | Measure-Object | Select-Object -ExpandProperty Count

        if($count -eq 0)
        {
            Install-Module -Name:$module -Scope:'CurrentUser' -Force;
        }
        elseif($count -eq 1)
        {
            Update-Module -Name:$module;
        }
        elseif($count -gt 1)
        {
            $pattern = $("{0}*" -f $env:USERPROFILE);

            Get-Module -FullyQualifiedName:$module -ListAvailable | Where {$_.Path -like $pattern } | Uninstall-Module

            # Install-Module -Name:$module -Scope:'CurrentUser' -Force;
        }
        else
        {
            throw "unexpected module count value";
        }
    }

}

#Initialize-LocalRequiredModules -Modules:@(,@('xWebAdministration', 'xPSDesiredStateConfiguration'));

function Remove-ModuleMultipleVersions
{
    param
    (
        [string]$Name
    )

    $modules = Get-Module -Name:$Name -ListAvailable

    if( $null -eq $modules )
    {
        return;
    }

    if($modules.Count -gt 1)
    {
        $modules | Uninstall-Module -Force;
    }
}

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

        $appPoolWriteAceArgs = @(
            (New-Object -TypeName:'System.Security.Principal.NTAccount' -ArgumentList:@('IIS APPPOOL\CertificateManager')),
            [System.Security.AccessControl.FileSystemRights]::Write,
            $inheritance,
            [System.Security.AccessControl.PropagationFlags]::None,
            [System.Security.AccessControl.AccessControlType]::Allow
        )
        $appPoolWriteAce = New-Object -TypeName:'System.Security.AccessControl.FileSystemAccessRule' -ArgumentList:$appPoolWriteAceArgs

        $appPoolWriteFileAceArgs = @(
            (New-Object -TypeName:'System.Security.Principal.NTAccount' -ArgumentList:@('IIS APPPOOL\CertificateManager')),
            [System.Security.AccessControl.FileSystemRights]::Write,
            [System.Security.AccessControl.InheritanceFlags]::None,
            [System.Security.AccessControl.PropagationFlags]::None,
            [System.Security.AccessControl.AccessControlType]::Allow
        )
        $appPoolWriteFileAce = New-Object -TypeName:'System.Security.AccessControl.FileSystemAccessRule' -ArgumentList:$appPoolWriteFileAceArgs


        $acl = [System.Security.AccessControl.DirectorySecurity]::new();
        
        $acl.SetOwner( (New-Object -TypeName:'System.Security.Principal.NTAccount' -ArgumentList:@('BUILTIN\Administrators')) );

        $acl.AddAccessRule($systemFullControlAce);
        $acl.AddAccessRule($adminFullControlAce);
        $acl.AddAccessRule($appPoolReadExecuteAce);

        $acl.SetAccessRuleProtection($true, $true);

        Set-Acl -Path:$InstallPath -AclObject:$acl;

        $dbDirectory = ( [System.IO.Path]::Combine($InstallPath, 'db') );
        $dbAcl = Get-Acl -Path:$dbDirectory

        $dbAcl.AddAccessRule($appPoolWriteAce);
        Set-Acl -Path:$dbDirectory -AclObject:$dbAcl;
        

        $logDirectory = ( [System.IO.Path]::Combine($InstallPath, 'logs') );
        $logAcl = Get-Acl -Path:$logDirectory;
        $logAcl.AddAccessRule($appPoolWriteAce);
        Set-Acl -Path:$logDirectory -AclObject:$logAcl;

    }
    return $script;
}


function New-CertificateManagerIISConfiguration
{
    param
    (
        $InstallPath,
        $HostName,
        $Thumbprint,
        [string]$HostingPackagePath
    )
    $start = Get-Location;

    Set-Location $env:TEMP | Out-Null;

    $result = CertificateManagerIISConfiguration -InstallPath:$InstallPath -HostName:$HostName -Thumbprint:$Thumbprint -HostingPackagePath:$HostingPackagePath;

    Set-Location $start | Out-Null;

    return $result;
}

function Get-ModuleLatestVersion
{
    param
    (
        [string]$Name
    )

    Add-Type -AssemblyName System.Linq;

    $availableModules = Get-Module -Name:$Name -ListAvailable;

    $directoryMatch = $env:USERPROFILE

    foreach($module in $availableModules)
    {
        if($module.ModuleBase.StartsWith($directoryMatch))
        {
            return $module.Version.ToString();
        }
    }

    return ( $availableModules | Select-Object -First 1).Version.ToString();
    # If there is no user module, just return the first one

}

function Install-Chocolatey
{
    param
    (
        [string]$ComputerName,
        [PSCredential]$Credential
    )

    $sb = {
        if ( $null -eq (Get-Command 'choco.exe' -ErrorAction SilentlyContinue) ) {
            [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.SecurityProtocolType]::Tls12;
            iex ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))
        }

        $newPath = $( "{0};C:\ProgramData\chocolatey\bin\" -f $env:PATH );
        [Environment]::SetEnvironmentVariable('PATH', $newPath, [System.EnvironmentVariableTarget]::Machine)
        [Environment]::SetEnvironmentVariable('PATH', $newPath, [System.EnvironmentVariableTarget]::Process)
        [Environment]::SetEnvironmentVariable('PATH', $newPath, [System.EnvironmentVariableTarget]::User)
    }
    Invoke-Command -ScriptBlock:$sb -Credential:$Credential -ComputerName:$ComputerName;
}

function Install-DotNetCoreHosting
{
    param
    (
        [string]$ComputerName,
        [PSCredential]$Credential
    )
    $sb = {
        if ( $null -eq (Get-Command 'choco.exe' -ErrorAction SilentlyContinue) ) {
            #throw 'Chocolatey is not installed, run Install-Chocolatey on this node';
            $newPath = $( "{0};C:\ProgramData\chocolatey\bin\" -f $env:PATH );
            [Environment]::SetEnvironmentVariable('PATH', $newPath, [System.EnvironmentVariableTarget]::Machine)
            [Environment]::SetEnvironmentVariable('PATH', $newPath, [System.EnvironmentVariableTarget]::Process)
            [Environment]::SetEnvironmentVariable('PATH', $newPath, [System.EnvironmentVariableTarget]::User)
        }

        choco install dotnetcore-windowshosting --version 2.2 -y
    }
    Invoke-Command -ScriptBlock:$sb -Credential:$Credential -ComputerName:$ComputerName;
}
function Install-CertificateManagerRequiredModuled
{
    param($Session)

    $xWebAdministrationSourcePath = [System.IO.Path]::Combine($env:TEMP, 'xWebAdministration');
    $xPSDesiredStateConfigurationSourcePath = [System.IO.Path]::Combine($env:TEMP, 'xPSDesiredStateConfiguration');

    Save-Module -Name 'xWebAdministration' -Path $env:TEMP -Force;
    Save-Module -Name 'xPSDesiredStateConfiguration' -Path $env:TEMP -Force;

    Copy-Item -Path:$xWebAdministrationSourcePath -Destination:'C:\Program Files\WindowsPowerShell\Modules' -ToSession:$Session -Force -Recurse;
    Copy-Item -Path:$xPSDesiredStateConfigurationSourcePath -Destination:'C:\Program Files\WindowsPowerShell\Modules' -ToSession:$Session -Force -Recurse;

    Remove-Item -Path:$xWebAdministrationSourcePath -Force -Recurse -Confirm:$false;
    Remove-Item -Path:$xPSDesiredStateConfigurationSourcePath -Force -Recurse -Confirm:$false;
}



function Initialize-RequiredModules
{
    
    param
    (
        [System.Management.Automation.Runspaces.PSSession]$Session,
        [System.Collections.Generic.List[string]]$Modules
    )

    $sb = {
        param ([System.Collections.Generic.List[string]]$ModulesArg)

        Set-PSRepository -Name 'PSGallery' -InstallationPolicy 'Trusted';

        Install-PackageProvider -Name NuGet -MinimumVersion 2.8.5.201 -Force;

        foreach($module in $ModulesArg)
        {
            New-Variable -Name:'count' -Value:0 -Force;

            $count = Get-Module -Name:$module -ListAvailable | Measure-Object | Select-Object -ExpandProperty Count

            if($count -eq 0)
            {
                Install-Module -Name:$module -Scope:'AllUsers' -Force;
            }
            elseif($count -eq 1)
            {
                Update-Module -Name:$module;
            }
            elseif($count -gt 1)
            {
                Get-Module -Name:$module -ListAvailable | Uninstall-Module -Force;
                Install-Module -Name:$module -Scope:'AllUsers' -Force;
            }
            else
            {
                throw "unexpected module count value";
            }
        }
    }
    Invoke-Command -ScriptBlock:$sb -Session:$Session -ArgumentList @(,$Modules)
}

function Get-ModuleLatest 
{
    param
    (
        [string]$Name
    )

    $availibleModules = Get-Module -ListAvailable -Name $Name;

    $versionList = New-Object 'System.Collections.Generic.List[object]' -ArgumentList ( ,@($availibleModules.Version) )

    $latestVersion = [System.Linq.Enumerable]::Max($versionList)
    
    return ( $availibleModules | where Version -eq $latestVersion | select -First 1 )
}

configuration CertificateManagerIISConfiguration
{
    param
    (
        [String]$InstallPath,
        [string]$HostName,
        [string]$Thumbprint,
        [string]$HostingPackagePath
    )

    Get-ModuleLatest 'xWebAdministration' | Import-Module -Force

    Import-DscResource -ModuleName 'PSDesiredStateConfiguration';
    Import-DscResource -ModuleName 'xPSDesiredStateConfiguration';
    Import-DscResource -ModuleName 'xWebAdministration';

    WindowsFeature IIS 
    {
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

    <#
        choco install dotnetcore-windowshosting
            Package InstallDotNetCoreWindowsHosting
            {
                Ensure = "Present"
                Path = $HostingPackagePath
                Arguments = "/q /norestart"
                Name = "DotNetCore"
                #52EB917D-6633-4063-BBDE-A57FA2E51F32
                #"2E6AD27D-9060-324F-AB1B-7C0F837583B3" - package x64
                ProductId = "52EB917D-6633-4063-BBDE-A57FA2E51F32"
                DependsOn = '[WindowsFeature]IIS'
            }
    #>

    xWebAppPool CertMgrAppPool
    {
        Name = 'CertificateManager'
        DependsOn = '[WindowsFeature]IIS'
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
                               HostName = "*"
                             }
                         )
        ApplicationPool = 'CertificateManager'
        DependsOn = '[WindowsFeature]IIS'
    }

}


function Get-CertificateManagerWebApplicationPackageHttp
{
    param
    (
        [Parameter(mandatory=$false)][ValidateNotNullOrEmpty()][System.Uri]$PackageUri,
        [Parameter(mandatory=$false)][ValidateNotNullOrEmpty()][string]$PackagePath
    )
    $downloadArgs = @{
        Uri = $PackageUri;
        OutFile = $PackagePath;
    };
    Invoke-WebRequest @downloadArgs | Out-Null;
}

function Get-CertificateManagerWebApplicationPackageFile
{
    param
    (
        [Parameter(mandatory=$false)][ValidateNotNullOrEmpty()][System.Uri]$PackageUri,
        [Parameter(mandatory=$false)][ValidateNotNullOrEmpty()][string]$PackagePath
    )

    Copy-Item -Path:($PackageUri.AbsolutePath) -Destination:$PackagePath -Force;

}


function Get-CertificateManagerWebApplicationPackage
{
    param
    (
        [Parameter(mandatory=$false)][ValidateNotNullOrEmpty()][System.Uri]$PackageUri,
        [Parameter(mandatory=$false)][ValidateNotNullOrEmpty()][string]$PackagePath
    )

    switch($PackageUri.Scheme)
    {
        "https" { Get-CertificateManagerWebApplicationPackageHttp -PackageUri:$PackageUri -PackagePath:$PackagePath; }
        "file" { Get-CertificateManagerWebApplicationPackageFile -PackageUri:$PackageUri -PackagePath:$PackagePath; }
        default { throw "PackageUri is not valid. https and file allowed." }
    }
}

function Add-CertificateManagerKeyStorePermissions
{

    $inheritance = ([System.Security.AccessControl.InheritanceFlags]::ContainerInherit -bor [System.Security.AccessControl.InheritanceFlags]::ObjectInherit);

    $appPoolReadExecuteAceArgs = @(
        (New-Object -TypeName:'System.Security.Principal.NTAccount' -ArgumentList:@('IIS APPPOOL\CertificateManager')),
        [System.Security.AccessControl.FileSystemRights]::ReadAndExecute,
        $inheritance,
        [System.Security.AccessControl.PropagationFlags]::InheritOnly,
        [System.Security.AccessControl.AccessControlType]::Allow
    )
    $appPoolReadExecuteAce = New-Object -TypeName:'System.Security.AccessControl.FileSystemAccessRule' -ArgumentList:$appPoolReadExecuteAceArgs

    $appPoolWriteAceArgs = @(
        (New-Object -TypeName:'System.Security.Principal.NTAccount' -ArgumentList:@('IIS APPPOOL\CertificateManager')),
        [System.Security.AccessControl.FileSystemRights]::Write,
        $inheritance,
        [System.Security.AccessControl.PropagationFlags]::InheritOnly,
        [System.Security.AccessControl.AccessControlType]::Allow
    )
    $appPoolWriteAce = New-Object -TypeName:'System.Security.AccessControl.FileSystemAccessRule' -ArgumentList:$appPoolWriteAceArgs


    $appPoolModifyAceArgs = @(
        (New-Object -TypeName:'System.Security.Principal.NTAccount' -ArgumentList:@('IIS APPPOOL\CertificateManager')),
        [System.Security.AccessControl.FileSystemRights]::FullControl,
        $inheritance,
        [System.Security.AccessControl.PropagationFlags]::None,
        [System.Security.AccessControl.AccessControlType]::Allow
    )
    $appPoolModifyFilesAce = New-Object -TypeName:'System.Security.AccessControl.FileSystemAccessRule' -ArgumentList:$appPoolModifyAceArgs

    $cryptoPath = [System.IO.Path]::Combine($env:ProgramData,'Microsoft\Crypto');

    $cryptoAcl = Get-Acl -Path:$cryptoPath;

    $cryptoAcl.AddAccessRule($appPoolReadExecuteAce);
    $cryptoAcl.AddAccessRule($appPoolWriteAce);
    $cryptoAcl.AddAccessRule($appPoolModifyFilesAce);

    Set-Acl -Path:$cryptoPath -AclObject:$cryptoAcl;
    

    $rsaAcl = Get-Acl -Path:'C:\ProgramData\Microsoft\Crypto\RSA';
    $rsaAcl.AddAccessRule($appPoolReadExecuteAce);
    $rsaAcl.AddAccessRule($appPoolWriteAce);
    $rsaAcl.AddAccessRule($appPoolModifyFilesAce);
    Set-Acl -Path:'C:\ProgramData\Microsoft\Crypto\RSA' -AclObject:$rsaAcl;

    $keysAcl = Get-Acl -Path:'C:\ProgramData\Microsoft\Crypto\Keys';
    $keysAcl.AddAccessRule($appPoolReadExecuteAce);
    $keysAcl.AddAccessRule($appPoolWriteAce);
    $keysAcl.AddAccessRule($appPoolModifyFilesAce);
    Set-Acl -Path:'C:\ProgramData\Microsoft\Crypto\Keys' -AclObject:$keysAcl;


    $regAcl = Get-Acl 'HKLM:\SOFTWARE\Microsoft\SystemCertificates'

    <#
        'System.Security.AccessControl.RegistryAccessRule'

    System.Security.AccessControl.RegistryAccessRule new(string identity, System.Security.AccessControl.RegistryRights
registryRights, System.Security.AccessControl.InheritanceFlags inheritanceFlags,
System.Security.AccessControl.PropagationFlags propagationFlags, System.Security.AccessControl.AccessControlType type)
    #>





    $cryptoPath1 = [System.IO.Path]::Combine($env:ProgramData,'Microsoft\Crypto\RSA\MachineKeys');

    $cryptoAcl1 = Get-Acl -Path:$cryptoPath1;

    $cryptoAcl1.AddAccessRule($appPoolReadExecuteAce);
    $cryptoAcl1.AddAccessRule($appPoolWriteAce);
    $cryptoAcl1.AddAccessRule($appPoolModifyFilesAce);

    Set-Acl -Path:$cryptoPath1 -AclObject:$cryptoAcl1;


    $cryptoPath2 = [System.IO.Path]::Combine($env:ProgramData,'Microsoft\Crypto\RSA\MachineKeys');

    $cryptoAcl2 = Get-Acl -Path:$cryptoPath2;

    $cryptoAcl2.AddAccessRule($appPoolReadExecuteAce);
    $cryptoAcl2.AddAccessRule($appPoolWriteAce);
    $cryptoAcl2.AddAccessRule($appPoolModifyFilesAce);

    Set-Acl -Path:$cryptoPath2 -AclObject:$cryptoAcl2;


    $regAceArgs = @(
        (New-Object -TypeName:'System.Security.Principal.NTAccount' -ArgumentList:@('IIS APPPOOL\CertificateManager')),
        [System.Security.AccessControl.RegistryRights]::FullControl,
        $inheritance,
        [System.Security.AccessControl.PropagationFlags]::InheritOnly,
        [System.Security.AccessControl.AccessControlType]::Allow
    )

    $regAce = New-Object -TypeName 'System.Security.AccessControl.RegistryAccessRule' -ArgumentList:$regAceArgs;

    $regAcl.AddAccessRule($regAce)

    Set-Acl -Path:'HKLM:\SOFTWARE\Microsoft\SystemCertificates' -AclObject:$regAcl


}


function Deploy-CertificateManager
{
    param
    (
        [Parameter(mandatory=$true)][ValidateNotNullOrEmpty()][string]$ComputerName,
        [Parameter(mandatory=$true)][PSCredential]$Credential,
        [Parameter(mandatory=$false)][string]$WebsiteHostname = 'certificatemanager.local',
        [Parameter(mandatory=$false)][ValidateNotNullOrEmpty()][string]$InstallPath = 'c:\Program Files\CertificateManager',
        [Parameter(mandatory=$false)][ValidateNotNullOrEmpty()][string]$PackageUri = 'https://github.com/corymurphy/CertificateManager/blob/master/Deployment/CertificateManagerWebApp.nupkg?raw=true',
        [Parameter(mandatory=$false)][ValidateNotNullOrEmpty()][string]$DotNetCoreHostingPackageUri = 'https://download.microsoft.com/download/8/D/A/8DA04DA7-565B-4372-BBCE-D44C7809A467/DotNetCore.2.0.6-1-WindowsHosting.exe'
    )

    begin
    {
        [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12;
        $packageName = 'CertificateManagerWebApp.nupkg';
        $packageZipName = 'CertificateManagerWebApp.zip';
        $hostingPackageName = 'DotNetCore.2.0.6-1-WindowsHosting.exe';
        $packagePath = [System.IO.Path]::Combine($env:TEMP, $packageName);
        $serverTempDirectory = 'c:\windows\temp';
        $ConfigurationName = 'CertificateManagerIISConfiguration';
        $serverConfigPath = ([IO.Path]::Combine($serverTempDirectory, $ConfigurationName));
        $hostingBundlePath = [System.IO.Path]::Combine($env:TEMP, $hostingPackageName);

        $downloadArgs = @{
            Uri = $PackageUri;
            OutFile = $packagePath;
        };


        $downloadDotNetCoreHostingArgs = @{
            Uri = $DotNetCoreHostingPackageUri;
            OutFile = $hostingBundlePath;
        }

        $newCertScript = {
            param($HostName)
            New-SelfSignedCertificate -DnsName:$HostName;
        }

        # Install-Module -Name 'xWebAdministration' -Scope CurrentUser -Confirm:$false -ErrorAction:Stop -WarningAction:SilentlyContinue -Force;
        # Install-Module -Name 'xPSDesiredStateConfiguration' -Scope CurrentUser -Confirm:$false -ErrorAction:Stop -WarningAction:SilentlyContinue -Force;

        $setupScript = { 
            param
            (
                $PackageName, 
                $PackageZipName, 
                $ServerTempDirectory, 
                $InstallPath, 
                $ConfigurationPath, 
                $WebsiteHostname
            )

            
            $PackagePath = [IO.Path]::Combine($ServerTempDirectory, $PackageName);
            $PackageZipPath = [IO.Path]::Combine($ServerTempDirectory, $PackageZipName);

            Remove-Item -Path $PackageZipPath -Force -ErrorAction SilentlyContinue;
            Rename-Item -Path $PackagePath -NewName $PackageZipName -Force -ErrorAction SilentlyContinue;
            Expand-Archive -Path $PackageZipPath -DestinationPath $InstallPath -Force;

            New-Item -Path:$InstallPath -Name 'db' -ItemType:'Directory' -Force | Out-Null;
            New-Item -Path:$InstallPath -Name:'logs' -ItemType:'Directory' -Force | Out-Null;

            $job = Start-DscConfiguration -Path:$ConfigurationPath -Force;

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
                throw $result;
                #throw 'IIS Configuration failed {0}{1}' -f [System.Environment]::NewLine,$result;
            }

            Import-Module -Name 'WebAdministration' -ErrorAction 'Stop';

            Set-ItemProperty -Path 'IIS:\AppPools\CertificateManager' -Name 'managedRuntimeVersion' -Value '' -Force;

            Set-ItemProperty -Path:'IIS:\AppPools\CertificateManager' -Name:"processModel.loadUserProfile" -Value "True" -Force;
            #Invoke-Command -ScriptBlock:$AclConfigScript -ArgumentList:@($InstallPath);
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

        $configurationPath = New-CertificateManagerIISConfiguration -InstallPath:$InstallPath -HostName:$WebsiteHostname -Thumbprint:$cert.Thumbprint -HostingPackagePath:( [IO.Path]::Combine($ServerTempDirectory, $hostingPackageName) );

        # Install-CertificateManagerRequiredModuled -Session:$Session;

        Get-CertificateManagerWebApplicationPackage -PackageUri:$PackageUri -PackagePath:$packagePath;

        #Invoke-WebRequest @downloadDotNetCoreHostingArgs | Out-Null;

        Copy-Item -ToSession:$session -Path:$packagePath -Destination:$serverTempDirectory -Force;

        #Copy-Item -ToSession:$session -Path:$hostingBundlePath -Destination:$serverTempDirectory -Force;

        Initialize-RequiredModules -Session:$session -Modules:@('xWebAdministration');

        Invoke-Command -ScriptBlock $createDscFolderScript -ArgumentList @($serverTempDirectory, $ConfigurationName) -Session $session;

        Copy-Item -ToSession $session -Path $configurationPath.FullName -Destination ([IO.Path]::Combine($ServerTempDirectory, $configurationname)) -Force;
    
        $setupArgs = @(
            $PackageName,
            $PackageZipName,
            $ServerTempDirectory,
            $InstallPath,
            $serverConfigPath,
            $WebsiteHostname
        );
        
        Invoke-Command -ScriptBlock $setupScript -ArgumentList $setupArgs -Session $session;

        Install-Chocolatey -Credential:$Credential -ComputerName:$ComputerName;
        Install-DotNetCoreHosting -Credential:$Credential -ComputerName:$ComputerName;

        Invoke-Command -ScriptBlock:([scriptblock](Get-SetCertificateManagerAclScript)) -ArgumentList:@($InstallPath) -Session:$session;
        Invoke-Command -ScriptBlock:${function:Add-CertificateManagerKeyStorePermissions} -Session:$session;
    }
}

Deploy-CertificateManager -ComputerName:$ComputerName -Credential:$Credential;
