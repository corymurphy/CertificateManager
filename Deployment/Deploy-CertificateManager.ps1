function New-CertificateManagerIISConfiguration
{
    $start = Get-Location;

    Set-Location $env:TEMP;

    $result = CertificateManagerIISConfiguration;

    Set-Location $start;

    return $result;
}

configuration CertificateManagerIISConfiguration
{
    param
    (
        [string]$NodeName
    )

    Import-DscResource -ModuleName 'PSDesiredStateConfiguration'

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
}

function Deploy-CertificateManager
{
    param
    (
        [Parameter(mandatory=$true)][ValidateNotNullOrEmpty()][string]$ComputerName,
        [Parameter(mandatory=$true)][PSCredential]$Credential,
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

        $configurationname = 'CertificateManagerIISConfiguration';

        $downloadArgs = @{
            Uri = $PackageUri;
            OutFile = $packagePath;
        };

        $configurationPath = New-CertificateManagerIISConfiguration;

        $setupScript = { 
            param($PackageName, $PackageZipName, $ServerTempDirectory, $InstallPath, $ConfigurationPath) 
            #ConfigurationFilePath = [IO.Path]::Combine($ServerTempDirectory, $configurationname, $ConfigurationFileName);
            $PackagePath = [IO.Path]::Combine($ServerTempDirectory, $PackageName);
            $PackageZipPath = [IO.Path]::Combine($ServerTempDirectory, $PackageZipName);

            Remove-Item -Path $PackageZipPath -Force;
            Rename-Item -Path $PackagePath -NewName $PackageZipName -Force;
            Expand-Archive -Path $PackageZipPath -DestinationPath $InstallPath -Force;

            $job = Start-DscConfiguration $ConfigurationPath;

            while( ($job | Get-Job).Status -eq 'Running' )
            {
                Start-Sleep 1;                
            }

            throw $($job | Get-Job);
            

        }

        $createDscFolderScript = { 
            param($serverTempDirectory, $configurationname) 
            New-Item -Path $serverTempDirectory -Name $configurationname -ItemType Directory -Force | Out-Null;
        }
    }
    
    process
    {
        $session = New-PSSession -ComputerName $ComputerName -Credential $Credential;
        $result = Invoke-WebRequest @downloadArgs;
        $ConfigurationFileName = $configurationPath.Name;
        $ConfigurationPath = ([IO.Path]::Combine($serverTempDirectory, $configurationname));

        Copy-Item -ToSession $session -Path $packagePath -Destination $serverTempDirectory -Force;

        Invoke-Command -ScriptBlock $createDscFolderScript -ArgumentList @($serverTempDirectory, $configurationname) -Session $session;

        Copy-Item -ToSession $session -Path $configurationPath.FullName -Destination $ConfigurationPath -Force;
        
        $setupArgs = @(
            $PackageName,
            $PackageZipName,
            $ServerTempDirectory,
            $InstallPath,
            $ConfigurationPath
        );
        
        Invoke-Command -ScriptBlock $setupScript -ArgumentList $setupArgs -Session $session;
    }
}

Deploy-CertificateManager -ComputerName $ComputerName -Credential $Credential