using CertificateManager.Entities;
using CertificateManager.Logic;
using CertificateManager.Repository;
using CertificateServices.ActiveDirectory;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace CertificateManager.Controllers
{
    public class ExternalIdentitySourcesController : Controller
    {
        IConfigurationRepository configurationRepository;
        HttpResponseHandler http;

        public ExternalIdentitySourcesController(IConfigurationRepository configurationRepository)
        {
            this.configurationRepository = configurationRepository;
            this.http = new HttpResponseHandler(this);
        }


        [HttpGet]
        [Route("view/identity-sources/external")]
        public ActionResult ExternalIdentitySourcesView()
        {
            return View("View");
        }

        [HttpGet]
        [Route("identity-source/external/query/users")]
        public JsonResult QueryUsers(string query)
        {
            IEnumerable<ExternalIdentitySource> sources = configurationRepository.GetAll<ExternalIdentitySource>();

            List<ExternalIdentitySourceAuthPrincipalQueryResultModel> results = new List<ExternalIdentitySourceAuthPrincipalQueryResultModel>();
            foreach(var source in sources)
            {
                ActiveDirectoryRepository activeDirectory = new ActiveDirectoryRepository(source.Domain, source.Domain, source.Username, source.Password);

                List<ActiveDirectoryAuthenticablePrincipal> sourceResults = activeDirectory.Search<ActiveDirectoryAuthenticablePrincipal>("anr", query, NamingContext.Default);

                if (sourceResults == null)
                    continue;

                if(sourceResults.Count == 1)
                {
                    results.Add(new ExternalIdentitySourceAuthPrincipalQueryResultModel()
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
                    results.Add(new ExternalIdentitySourceAuthPrincipalQueryResultModel()
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
        public JsonResult GetExternalIdentitySources()
        {
            List<ExternalIdentitySource> modified = new List<ExternalIdentitySource>();
            IEnumerable<ExternalIdentitySource> actual = configurationRepository.GetAll<ExternalIdentitySource>();

            foreach (ExternalIdentitySource item in actual)
            {
                item.Password = "********";
                modified.Add(item);
            }

            return Json(modified);
        }


        [HttpGet]
        [Route("cm-config/external-identity-sources/domains")]
        public JsonResult GetExternalIdentitySourceDomains()
        {
            return Json(configurationRepository.GetAll<ExternalIdentitySourceDomains>());
        }



        [HttpDelete]
        [Route("cm-config/external-identity-source")]
        public JsonResult DeleteExternalIdentitySource(ExternalIdentitySource entity)
        {
            configurationRepository.Delete<ExternalIdentitySource>(entity.Id);

            return Json(new { status = "success" });
        }

        [HttpPost]
        [Route("cm-config/external-identity-source")]
        public JsonResult AddExternalIdentitySource(ExternalIdentitySource entity)
        {
            configurationRepository.Insert<ExternalIdentitySource>(entity);

            return Json(new { status = "success" });
        }


        [HttpPut]
        [Route("cm-config/external-identity-source")]
        public JsonResult UpdateExternalIdentitySource(ExternalIdentitySource entity)
        {
            ExternalIdentitySource existing = configurationRepository.Get<ExternalIdentitySource>(entity.Id);
            entity.Password = existing.Password;

            configurationRepository.Update<ExternalIdentitySource>(entity);

            return Json(new { status = "success" });
        }


    }
}