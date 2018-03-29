using CertificateManager.Entities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Security.Claims;

namespace CertificateManager.Logic.Interfaces
{
    public interface IPowershellEngine
    {
        void ValidateSyntax(Script script, ClaimsPrincipal user);

        PSCredential NewPSCredential(string username, string password);

        Collection<T> InvokeScriptAsync<T>(string scriptName, Dictionary<string, object> cmdletParams, ClaimsPrincipal user);
    }
}
