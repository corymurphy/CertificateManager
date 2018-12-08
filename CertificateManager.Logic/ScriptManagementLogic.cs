using CertificateManager.Entities;
using CertificateManager.Logic.Interfaces;
using CertificateManager.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace CertificateManager.Logic
{
    public class ScriptManagementLogic : IScriptManagementLogic
    {
        IConfigurationRepository configurationRepository;

        public ScriptManagementLogic(IConfigurationRepository configurationRepository, IAuthorizationLogic authorizationLogic)
        {
            this.configurationRepository = configurationRepository;
        }


        public IEnumerable<Script> All(ClaimsPrincipal user)
        {
            return configurationRepository.GetAll<Script>();
        }

        public Script Get(string id, ClaimsPrincipal user)
        {

            Guid validatedId;

            if (!Guid.TryParse(id, out validatedId))
            {
                throw new Exception("Invalid script");
            }

            return configurationRepository.Get<Script>(validatedId);
        }

        public void Add(Script script, ClaimsPrincipal user)
        {


            configurationRepository.Insert<Script>(script);

        }

        public void Delete(string id, ClaimsPrincipal user)
        {
            Guid validatedId;

            if (!Guid.TryParse(id, out validatedId))
            {
                throw new Exception("Invalid script");
            }

            configurationRepository.Delete<Script>(validatedId);
        }

        public void Update(Script script, ClaimsPrincipal user)
        {
            Script existing = configurationRepository.Get<Script>(script.Id);

            existing.Code = script.Code;
            existing.Name = script.Name;

            configurationRepository.Update<Script>(existing);
        }

        public Script GetByName(string name, ClaimsPrincipal user)
        {
            return configurationRepository.Get<Script>(item => item.Name == name).Single();
        }
    }
}
