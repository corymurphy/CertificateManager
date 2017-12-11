using CertificateManager.Entities;
using CertificateManager.Logic;
using CertificateManager.Logic.ActiveDirectory;
using CertificateManager.Logic.ActiveDirectory.Interfaces;
using CertificateManager.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CertificateManager.Controllers
{
    public class ActiveDirectoryMetadatasController : Controller
    {
        IConfigurationRepository configurationRepository;
        IActiveDirectoryRepository activeDirectory;
        HttpResponseHandler http;

        public ActiveDirectoryMetadatasController(IConfigurationRepository configurationRepository, IActiveDirectoryRepository activeDirectory)
        {
            this.configurationRepository = configurationRepository;
            this.http = new HttpResponseHandler(this);
            this.activeDirectory = activeDirectory;
        }


        [HttpGet]
        [Route("view/identity-sources/external")]
        public ActionResult ActiveDirectoryMetadatasView()
        {
            return View("View");
        }

        [HttpGet]
        [Route("identity-source/external/query/users")]
        public JsonResult QueryUsers(string query)
        {
            IEnumerable<ActiveDirectoryMetadata> sources = configurationRepository.GetAll<ActiveDirectoryMetadata>();

            List<ActiveDirectoryMetadataAuthPrincipalQueryResultModel> results = new List<ActiveDirectoryMetadataAuthPrincipalQueryResultModel>();
            foreach(ActiveDirectoryMetadata source in sources)
            {
                List<ActiveDirectoryAuthenticablePrincipal> sourceResults = activeDirectory.Search<ActiveDirectoryAuthenticablePrincipal>("anr", query, NamingContext.Default, source);

                if (sourceResults == null)
                    continue;

                if(sourceResults.Count == 1)
                {
                    results.Add(new ActiveDirectoryMetadataAuthPrincipalQueryResultModel()
                    {
                        Domain = source.Domain, 
                        SamAccountName = sourceResults[0].SamAccountName,
                        Id = sourceResults[0].Id,
                        UserPrincipalName = sourceResults[0].UserPrincipalName,
                        DisplayName = sourceResults[0].DisplayName,
                        Name = sourceResults[0].Name,
                        DomainId = source.Id
                    });
                    continue;
                }

                foreach(var user in sourceResults)
                {
                    results.Add(new ActiveDirectoryMetadataAuthPrincipalQueryResultModel()
                    {
                        Domain = source.Domain,
                        SamAccountName = user.SamAccountName,
                        Id = user.Id,
                        UserPrincipalName = user.UserPrincipalName,
                        DisplayName = user.DisplayName,
                        Name = user.Name,
                        DomainId = source.Id
                    });
                    continue;
                }

                

            }

            return http.RespondSuccess(results);
            
        }


        [HttpGet]
        [Route("cm-config/external-identity-sources")]
        public JsonResult GetActiveDirectoryMetadatas()
        {
            List<ActiveDirectoryMetadata> modified = new List<ActiveDirectoryMetadata>();
            IEnumerable<ActiveDirectoryMetadata> actual = configurationRepository.GetAll<ActiveDirectoryMetadata>();

            foreach (ActiveDirectoryMetadata item in actual)
            {
                item.Password = "********";
                modified.Add(item);
            }

            return Json(modified);
        }


        [HttpGet]
        [Route("cm-config/external-identity-sources/domains")]
        public JsonResult GetActiveDirectoryMetadataDomains()
        {
            return Json(configurationRepository.GetAll<ActiveDirectoryMetadataDomains>());
        }



        [HttpDelete]
        [Route("cm-config/external-identity-source")]
        public JsonResult DeleteActiveDirectoryMetadata(ActiveDirectoryMetadata entity)
        {
            configurationRepository.Delete<ActiveDirectoryMetadata>(entity.Id);

            return Json(new { status = "success" });
        }

        [HttpPost]
        [Route("cm-config/external-identity-source")]
        public JsonResult AddActiveDirectoryMetadata(ActiveDirectoryMetadata entity)
        {
            configurationRepository.Insert<ActiveDirectoryMetadata>(entity);

            return Json(new { status = "success" });
        }


        [HttpPut]
        [Route("cm-config/external-identity-source")]
        public JsonResult UpdateActiveDirectoryMetadata(ActiveDirectoryMetadata entity)
        {
            ActiveDirectoryMetadata existing = configurationRepository.Get<ActiveDirectoryMetadata>(entity.Id);
            entity.Password = existing.Password;

            configurationRepository.Update<ActiveDirectoryMetadata>(entity);

            return Json(new { status = "success" });
        }


    }
}