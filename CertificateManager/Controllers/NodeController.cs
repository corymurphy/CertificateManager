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
    public class NodeController : Controller
    {
        NodeLogic nodeLogic;
        IConfigurationRepository configurationRepository;
        HttpResponseHandler http;

        public NodeController(IConfigurationRepository configurationRepository, NodeLogic nodeLogic)
        {
            this.nodeLogic = nodeLogic;
            this.configurationRepository = configurationRepository;
            this.http = new HttpResponseHandler(this);
        }

        [HttpGet]
        [Route("view/node/{id:guid}")]
        public ActionResult ViewNode(Guid id)
        {
            ViewBag.NodeId = id;
            return View();
        }

        [HttpGet]
        [Route("view/nodes")]
        public ActionResult ViewNodes()
        {
            return View();
        }


        [HttpGet]
        [Route("nodes")]
        public JsonResult GetNodes()
        {
            return http.RespondSuccess(nodeLogic.All(User));
        }

        [HttpGet]
        [Route("node/{id:guid}")]
        public JsonResult GetNode(Guid id)
        {
            Node node = nodeLogic.Get(id.ToString());
            
            return http.RespondSuccess(node);
        }

        [HttpDelete]
        [Route("node")]
        public JsonResult DeleteNode(Node node)
        {
            nodeLogic.Delete(node, User);
            return http.RespondSuccess();
        }

        [HttpPost]
        [Route("node")]
        public JsonResult AddNode(AddNodesEntity node)
        {
            nodeLogic.Add(node, User);
            return http.RespondSuccess();
        }

        [HttpPut]
        [Route("node")]
        public JsonResult UpdateNode(Node node)
        {
            nodeLogic.Update(node, User);
            return http.RespondSuccess();
        }
    }
}