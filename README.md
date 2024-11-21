# CertificateManager
Web app for managing Active Directory Certificate Services infrastructure

# About

Certificate Manager is a Web App and RESTful API (with powershell module) for solving certificate management issues. CertManager can easily integrate with a deployment automation system such as Octopus Deploy to automtically issue certificates elimiating certificate management overhead. It was specifically designed to work with ADCS. It will issue certificates, automatically renew certificates (Windows and IIS are supported at this time, but Linux support is coming), and store certificates with private keys. 

Ideally, private keys would only be stored on a node where they are needed to perform cryptogrphic operations, however; managing certificates in an enterprise might require more management and governance and a decentralized system can be difficult to maintain. Certificate Manager intends to address this problem.

# How to Deploy Certificate Manager

* Certificate Manager is packaged in a single nuget package that can be deployed to IIS
* [Install .NET Core IIS Server Hosting](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-aspnetcore-2.0.9-windows-hosting-bundle-installer)
* Run the following powershell to automatically deploy CertificateManager to your server. Supply value for *ComputerName*, you will be prompted for a credential
```powershell
$ComputerName = 'web03.certmgr.local'; [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12;Invoke-WebRequest -Uri:'https://github.com/corymurphy/CertificateManager/blob/master/Deployment/Deploy-CertificateManager.ps1?raw=true' -OutFile:$('{0}\Deploy-CertificateManager.ps1' -f $env:TEMP);$cmdlet =  $('{0}\Deploy-CertificateManager.ps1' -f $env:TEMP); . $cmdlet -ComputerName:$ComputerName -Credential:(Get-Credential);
```
* If the above cmdlet does not work, ensure that PSRemoting works to your web server.
* Navigate to `https://hostname/initial-setup` and follow the initial setup instructions.
