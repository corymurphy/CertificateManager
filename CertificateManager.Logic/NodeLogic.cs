using CertificateManager.Entities;
using CertificateManager.Entities.Enumerations;
using CertificateManager.Entities.Exceptions;
using CertificateManager.Logic.Interfaces;
using CertificateManager.Powershell.Runtime;
using CertificateManager.Repository;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace CertificateManager.Logic
{
    public class NodeLogic
    {
        IPrivateCertificateProcessing privateCertificateProcessing;
        IConfigurationRepository configurationRepository;
        IAuthorizationLogic authorizationLogic;
        ActiveDirectoryIdentityProviderLogic adIdpLogic;
        DnsValidation dnsValidation = new DnsValidation();
        IPowershellEngine powershell;
        IAuditLogic auditLogic;
        string hostIisDiscoveryScript = "HostIISDiscovery";
        string certificateDeploymentScript = "DeployCertificateToNode";
        string iisCertificateRenewal = "RenewIISCertificate";

        CertificateManagementLogic certificateManagement;

        public NodeLogic(IConfigurationRepository configurationRepository, IAuthorizationLogic authorizationLogic, ActiveDirectoryIdentityProviderLogic adIdpLogic, IPowershellEngine powershell, IAuditLogic auditLogic, CertificateManagementLogic certificateManagement, IPrivateCertificateProcessing privateCertificateProcessing)
        {
            this.auditLogic = auditLogic;
            this.powershell = powershell;
            this.configurationRepository = configurationRepository;
            this.authorizationLogic = authorizationLogic;
            this.adIdpLogic = adIdpLogic;
            this.certificateManagement = certificateManagement;
            this.privateCertificateProcessing = privateCertificateProcessing;

        }

        public NodeDetails Get(string id)
        {

            if(string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("Invalid Node");
            }

            Guid validatedId;

            if (!Guid.TryParse(id, out validatedId))
            {
                throw new ArgumentOutOfRangeException("Invalid Node");
            }

            NodeDetails node = configurationRepository.Get<NodeDetails>(validatedId);

            if (node == null)
            {
                throw new ReferencedObjectDoesNotExistException("Node could not be found");
            }

            ActiveDirectoryMetadata idp = adIdpLogic.GetAll()
                .Where(item => item.Id == node.CredentialId)
                .FirstOrDefault();

            node.CredentialDisplayName = idp.Name;
            
            return node;
        }

        public NodeCredentialed GetCredentialedNode(Guid id)
        {

            NodeCredentialed node = configurationRepository.Get<NodeCredentialed>(id);

            if (node == null)
            {
                throw new ReferencedObjectDoesNotExistException("Node could not be found");
            }

            node.CredentialContext = adIdpLogic.GetAll()
                .Where(item => item.Id == node.CredentialId)
                .FirstOrDefault();

            return node;
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
                Hostname = entity.Hostname,
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

        private void RecieveIISCertificateDiscoveryResult(Collection<HostIISCertificateEntity> results, NodeCredentialed node)
        {
            Node storedNode = configurationRepository.Get<Node>(node.Id);

            if(storedNode.ManagedCertificates == null || storedNode.ManagedCertificates.Count == 0)
            {
                storedNode.ManagedCertificates = new List<ManagedCertificate>();
            }

            List<HostIISCertificateEntity> resultsList = results.ToList();

            foreach (HostIISCertificateEntity result in resultsList)
            {
                bool alreadyDiscovered = storedNode.ManagedCertificates.Exists(cert => cert.Thumbprint == result.Thumbprint && cert.ManagedCertificateType == ManagedCertificateType.IIS);

                if (!alreadyDiscovered)
                {
                    ManagedCertificate managedCertificate = new ManagedCertificate()
                    {
                        DiscoveryDate = DateTime.Now,
                        Id = Guid.NewGuid(),
                        Thumbprint = result.Thumbprint,
                        ManagedCertificateType = ManagedCertificateType.IIS,
                        LastRenewal = DateTime.MinValue,
                        X509Content = result.X509Content
                    };

                    storedNode.ManagedCertificates.Add(managedCertificate);
                }
            }

            List<ManagedCertificate> newManagedList = new List<ManagedCertificate>();

            foreach (ManagedCertificate managedCert in storedNode.ManagedCertificates)
            {
                if(resultsList.Exists(x => x.Thumbprint == managedCert.Thumbprint))
                {
                    newManagedList.Add(managedCert);
                }
            }

            storedNode.ManagedCertificates = newManagedList;
            storedNode.CommunicationSuccess = true;
            storedNode.LastCommunication = DateTime.Now;

            configurationRepository.Update<Node>(storedNode);
        }
       
        private void StartIISCertificateDiscoveryTaskAsync(NodeCredentialed node, Dictionary<string, object> cmdletParams, ClaimsPrincipal user)
        {
            Collection<HostIISCertificateEntity> result = null;
            try
            {
                result = powershell.InvokeScriptAsync<HostIISCertificateEntity>(hostIisDiscoveryScript, cmdletParams, user);
                this.RecieveIISCertificateDiscoveryResult(result, node);
            }
            catch(Exception ex)
            {
                string msg = string.Format("An error occured while attempting to run the host iis discovery job: {0}", ex.ToString());
                auditLogic.LogOpsError(user, node.Id.ToString(), EventCategory.PowershellJob, msg);
            }
        }

        public void InvokeIISCertificateDiscovery(Guid nodeId, ClaimsPrincipal user)
        {
            NodeCredentialed node = this.GetCredentialedNode(nodeId);

            Dictionary<string, object> cmdletParams = GetIISCertificateDiscoveryCmdletParams(node);

            Task.Run(() => this.StartIISCertificateDiscoveryTaskAsync(node, cmdletParams, user));
        }

        public Dictionary<string, object> GetIISCertificateDiscoveryCmdletParams(NodeCredentialed node)
        {

            Dictionary<string, object> cmdletParams = new Dictionary<string, object>()
            {
                { "ComputerName", node.Hostname },
                { "Credential", powershell.NewPSCredential(node.CredentialContext.Username, node.CredentialContext.Password) }
            };

            return cmdletParams;
        }

        private void StartCertificateDeployment(NodeCredentialed node, Dictionary<string, object> cmdletParams, ClaimsPrincipal user)
        {
            //Collection<HostIISCertificateEntity> result = null;
            try
            {
                object result = powershell.InvokeScriptAsync<object>(certificateDeploymentScript, cmdletParams, user);

                node.LastCommunication = DateTime.Now;
                node.CommunicationSuccess = true;

                configurationRepository.Update<NodeCredentialed>(node);

                return;
                //result = powershell.InvokeScriptAsync<HostIISCertificateEntity>(hostIisDiscoveryScript, cmdletParams, user);
                //this.RecieveIISCertificateDiscoveryResult(result, node);
            }
            catch (Exception ex)
            {
                string msg = string.Format("An error occured while starting the certificate deployment job {0}", ex.ToString());
                auditLogic.LogOpsError(user, node.Id.ToString(), EventCategory.PowershellJob, msg);
            }
        }

        public void InvokeCertificateDeployment(Guid nodeId, Guid certId, ClaimsPrincipal user)
        {
            DownloadPfxCertificateEntity cert = certificateManagement.GetPfxCertificateContent(certId);

            NodeCredentialed node = this.GetCredentialedNode(nodeId);

            string password = certificateManagement.GetCertificatePassword(certId, user).DecryptedPassword;

            Dictionary<string, object> cmdletParams = new Dictionary<string, object>()
            {
                { "ComputerName", node.Hostname },
                { "Credential", powershell.NewPSCredential(node.CredentialContext.Username, node.CredentialContext.Password) },
                { "CertificateContent", Convert.ToBase64String(cert.Content) },
                { "CertificateKey", password }
            };

            Task.Run(() => this.StartCertificateDeployment(node, cmdletParams, user));

        }

        public void InvokeRenewIISCertificate(Guid nodeId, Guid nodeManagedCertId, ClaimsPrincipal user)
        {
            NodeCredentialed node = this.GetCredentialedNode(nodeId);

            ManagedCertificate managedCertificate = node.ManagedCertificates.Where(x => x.Id == nodeManagedCertId).First();

            X509Certificate2 cert = new X509Certificate2( Convert.FromBase64String( managedCertificate.X509Content ) );

            CreatePrivateCertificateModel entity = new CreatePrivateCertificateModel(cert);

            CreatePrivateCertificateResult newCert = privateCertificateProcessing.CreateCertificateWithPrivateKey(entity, user);

            if(newCert.Status != PrivateCertificateRequestStatus.Success)
            {
                throw new CertificateAuthorityDeniedRequestException("Certificate request could not be processed");
            }

            DownloadPfxCertificateEntity pfx = certificateManagement.GetPfxCertificateContent(newCert.Id);

            string password = certificateManagement.GetCertificatePassword(newCert.Id, user).DecryptedPassword;

            Dictionary<string, object> cmdletParams = new Dictionary<string, object>()
            {
                { "ComputerName", node.Hostname },
                { "Credential", powershell.NewPSCredential(node.CredentialContext.Username, node.CredentialContext.Password) },
                { "CertificateContent", Convert.ToBase64String(pfx.Content) },
                { "CertificateKey", password }
            };

            Task.Run(() => this.StartIISCertificateRenewal(node, cmdletParams, user));

        }


        private void StartIISCertificateRenewal(NodeCredentialed node, Dictionary<string, object> cmdletParams, ClaimsPrincipal user)
        {
            //Collection<HostIISCertificateEntity> result = null;
            try
            {
                object result = powershell.InvokeScriptAsync<object>(iisCertificateRenewal, cmdletParams, user);
                return;
                //result = powershell.InvokeScriptAsync<HostIISCertificateEntity>(hostIisDiscoveryScript, cmdletParams, user);
                //this.RecieveIISCertificateDiscoveryResult(result, node);
            }
            catch (Exception ex)
            {
                string msg = string.Format("An error occured while starting the certificate deployment job {0}", ex.ToString());
                auditLogic.LogOpsError(user, node.Id.ToString(), EventCategory.PowershellJob, msg);
            }
        }

    }
}
