using CertificateManager.Entities;
using System.Collections.Generic;
using System.Security.Claims;

namespace CertificateManager.Logic.Interfaces
{
    public interface IScriptManagementLogic
    {
        IEnumerable<Script> All(ClaimsPrincipal user);

        Script Get(string id, ClaimsPrincipal user);

        Script GetByName(string name, ClaimsPrincipal user);

        void Add(Script script, ClaimsPrincipal user);

        void Delete(string id, ClaimsPrincipal user);

        void Update(Script script, ClaimsPrincipal user);
    }
}
