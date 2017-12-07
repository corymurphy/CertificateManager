using CertificateManager.Entities;
using CertificateManager.Logic;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CertificateManager.Controllers
{
    public class CertificatesController : Controller
    {

        HttpResponseHandler http;
        CertificateManagementLogic certificateManagementLogic;

        public CertificatesController(CertificateManagementLogic certificateManagementLogic)
        {
            this.certificateManagementLogic = certificateManagementLogic;
            this.http = new HttpResponseHandler(this);
        }

        [HttpGet]
        [Route("certificate/{id:guid}")]
        public JsonResult GetCertificate(Guid id)
        {
            return http.RespondSuccess(certificateManagementLogic.GetCertificate(id));
        }

        [HttpDelete]
        [Route("certificate/{certId:guid}/acl/{aceId:guid}")]
        public JsonResult DeleteCertificateAce(Guid certId, Guid aceId)
        {
            certificateManagementLogic.DeleteCertificateAce(certId, aceId);
            return http.RespondSuccess();
        }

        [HttpPut]
        [Route("certificate/{id:guid}/acl")]
        public JsonResult AddCertificateAce(Guid id, [FromBody]AddCertificateAceEntity entity)
        {         
            return http.RespondSuccess(certificateManagementLogic.AddCertificateAce(id, entity));
        }

        [HttpGet]
        [Route("certificate/{id:guid}/password")]
        public JsonResult GetCertificatePassword(Guid id)
        {
            return http.RespondSuccess(certificateManagementLogic.GetCertificatePassword(id, User));
        }

        [HttpPut]
        [Route("certificate/{id:guid}/password")]
        public JsonResult ResetCertificatePassword(Guid id)
        {
            certificateManagementLogic.ResetCertificatePassword(id, User);
            return http.RespondSuccess();
        }

        [Route("view/certificate/{id:guid}")]
        public ActionResult ViewCertificate(Guid id)
        {
            ViewBag.CertificateId = id;
            return View("View");
        }

        [Route("view/certificates")]
        public ActionResult ViewAllCertificates()
        {
            return View("ViewAllCertificates");
        }

        [Route("certificates")]
        public JsonResult AllCertificates()
        {
            return http.RespondSuccess(certificateManagementLogic.GetAllCertificates());
        }

        [HttpDelete]
        [Route("certificate")]
        public JsonResult DeleteCertificate(Guid id)
        {
            return null;
        }



    }
}