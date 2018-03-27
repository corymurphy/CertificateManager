using CertificateManager.Entities;
using System.Security.Claims;

namespace CertificateManager.Logic.Interfaces
{
    public interface IPowershellEngine
    {
        void ValidateSyntax(Script script, ClaimsPrincipal user);
    }
}
