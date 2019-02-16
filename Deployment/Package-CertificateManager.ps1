nuget pack 'CertificateManager.nuspec' -OutputFileNamesWithoutVersion -NoPackageAnalysis -OutputDirectory .
Copy-Item -Path .\CertificateManagerWebApp.nupkg -Destination '\\srv02\repo'