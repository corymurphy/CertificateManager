# CertificateManager
Web app for managing Active Directory Certificate Services infrastructure

# About

Certificate Manager is a Web App and RESTful API (with powershell module) for solving certificate management issues. CertManager can easily integrate with a deployment automation system such as Octopus Deploy to automtically issue certificates elimiating certificate management overhead. It was specifically designed to work with ADCS. It will issue certificates, automatically renew certificates (Windows and IIS are supported at this time, but Linux support is coming), and store certificates with private keys. 

Ideally, private keys would only be stored on a node where they are needed to perform cryptogrphic operations, however; managing certificates in an enterprise might require more management and governance and a decentralized system can be difficult to maintain. Certificate Manager intends to address this problem.

# How to Deploy Certificate Manager

⋅⋅* Certificate Manager is packaged in a single nuget package that can be deployed to IIS
⋅⋅* Run this cmdlet after you've downloaded the nuget package to deploy to IIS
⋅⋅* After you've run the cmdlet, open the web app that was created and follow the initial setup instructions