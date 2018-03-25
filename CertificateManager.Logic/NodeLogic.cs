using CertificateManager.Entities;
using CertificateManager.Logic.Interfaces;
using CertificateManager.Repository;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace CertificateManager.Logic
{
    public class NodeLogic
    {
        IConfigurationRepository configurationRepository;
        IAuthorizationLogic authorizationLogic;
        ActiveDirectoryIdentityProviderLogic adIdpLogic;
        DnsValidation dnsValidation = new DnsValidation();

        public NodeLogic(IConfigurationRepository configurationRepository, IAuthorizationLogic authorizationLogic, ActiveDirectoryIdentityProviderLogic adIdpLogic)
        {
            this.configurationRepository = configurationRepository;
            this.authorizationLogic = authorizationLogic;
            this.adIdpLogic = adIdpLogic;
        }

        public Node Get(string id)
        {
            Guid validatedId;

            if (!Guid.TryParse(id, out validatedId))
            {
                throw new Exception("Invalid Node");
            }
            else
            {
                return configurationRepository.Get<Node>(validatedId);
            }
        }

        public void Add(AddNodesEntity entity, ClaimsPrincipal user)
        { 
            if(!dnsValidation.ValidateDnsName(entity.Hostname))
            {
                throw new Exception("Invalid hostname provided");
            }


            if(string.IsNullOrWhiteSpace(entity.CredentialId))
            {
                throw new Exception("Credential requested does not exist");
            }


            if(!adIdpLogic.AdIdpExists(entity.CredentialId))
            {
                throw new Exception("Credential requested does not exist");
            }

            Node node = new Node()
            {
                Id = Guid.NewGuid(),
                DiscoveryType = Entities.Enumerations.DiscoveryType.Manual,
                LastCommunication = DateTime.MinValue,
                CredentialId = Guid.Parse(entity.CredentialId)
            };

            configurationRepository.Insert<Node>(node);
        }

        public void Delete(Node node, ClaimsPrincipal user)
        {
            configurationRepository.Delete<Node>(node.Id);
        }
        
        public void Update(Node node, ClaimsPrincipal user)
        {
            configurationRepository.Update<Node>(node);
        }

        public IEnumerable<Node> All(ClaimsPrincipal user)
        {
            return configurationRepository.GetAll<Node>();
        }
        //public void Discover()
    }
}
