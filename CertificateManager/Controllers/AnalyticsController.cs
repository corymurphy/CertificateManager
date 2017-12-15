using CertificateManager.Entities;
using CertificateManager.Logic;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CertificateManager.Controllers
{
    public class AnalyticsController : Controller
    {
        private readonly AnalyticsLogic analytics;
        HttpResponseHandler http;

        public AnalyticsController(AnalyticsLogic analytics)
        {

            this.http = new HttpResponseHandler(this);
            this.analytics = analytics;
        }

        [HttpGet]
        [Route("analytics/certificate-issuance")]
        public JsonResult GetCertificateHistory()
        {

            return http.RespondSuccess();
        }

        [HttpGet]
        [Route("analytics/log-activity")]
        public JsonResult GetLogActivity()
        {

            return http.RespondSuccess();
        }

    }
}