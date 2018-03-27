using CertificateManager.Entities;
using CertificateManager.Logic;
using CertificateManager.Logic.ActiveDirectory;
using CertificateManager.Logic.ActiveDirectory.Interfaces;
using CertificateManager.Logic.Interfaces;
using CertificateManager.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace CertificateManager.Controllers
{
    public class ScriptController : Controller
    {
        ScriptManagementLogic scriptLogic;
        IConfigurationRepository configurationRepository;
        HttpResponseHandler http;
        IPowershellEngine powershell;

        public ScriptController(IConfigurationRepository configurationRepository, ScriptManagementLogic scriptLogic, IPowershellEngine powershell)
        {
            this.powershell = powershell;
            this.scriptLogic = scriptLogic;
            this.configurationRepository = configurationRepository;
            this.http = new HttpResponseHandler(this);
        }

        [HttpGet]
        [Route("view/script/{id:guid}")]
        public ActionResult ViewScript(Guid id)
        {
            ViewBag.NodeId = id;
            return View();
        }

        [HttpGet]
        [Route("view/scripts")]
        public ActionResult ViewScripts()
        {
            return View();
        }


        [HttpGet]
        [Route("scripts")]
        public JsonResult GetScripts()
        {
            var nodes = scriptLogic.All(User);
            return http.RespondSuccess(nodes);
        }

        [HttpGet]
        [Route("script/{id:guid}")]
        public JsonResult GetScript(Guid id)
        {
            Script node = scriptLogic.Get(id.ToString(), User);
            
            return http.RespondSuccess(node);
        }

        [HttpDelete]
        [Route("script")]
        public JsonResult Delete(Script script)
        {
            scriptLogic.Delete(script.Id.ToString(), User);
            return http.RespondSuccess();
        }

        [HttpPost]
        [Route("script")]
        public JsonResult Add(Script script)
        {
            scriptLogic.Add(script, User);
            return http.RespondSuccess();
        }

        [HttpPut]
        [Route("script")]
        public JsonResult Update(Script script)
        {
            powershell.ValidateSyntax(script, User);
            scriptLogic.Update(script, User);
            return http.RespondSuccess();
        }
    }
}