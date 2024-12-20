﻿using CertificateManager.Entities;
using CertificateManager.Logic;
using CertificateManager.Repository;
using Microsoft.AspNetCore.Mvc;
using System;

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
            var nodes = nodeLogic.All(User);
            return http.RespondSuccess(nodes);
        }

        [HttpGet]
        [Route("node/{id:guid}")]
        public JsonResult GetNode(Guid id)
        {
            NodeDetails node = nodeLogic.Get(id.ToString());
            
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

        [HttpPost]
        [Route("node/{id:guid}/discovery/iis")]
        public JsonResult InvokeIISCertificateDiscovert(Guid id)
        {
            nodeLogic.InvokeIISCertificateDiscovery(id, User);

            return http.RespondSuccess();
        }

        [HttpPost]
        [Route("node/{nodeId:guid}/deploy/{certId:guid}")]
        public JsonResult InvokeCertificateDeployment(Guid nodeId, Guid certId)
        {
            nodeLogic.InvokeCertificateDeployment(nodeId, certId, User);
            return http.RespondSuccess();
        }


        [HttpPost]
        [Route("node/{nodeId:guid}/renew/{managedCertId:guid}")]
        public JsonResult InvokeRenewIISCertificate(Guid nodeId, Guid managedCertId)
        {
            nodeLogic.InvokeRenewIISCertificate(nodeId, managedCertId, User);
            return http.RespondSuccess();
        }

        [HttpDelete]
        [Route("node/{nodeId:guid}/managedcertificates")]
        public JsonResult ResetManagedCertificatesState(Guid nodeId)
        {
            nodeLogic.ResetManagedCertificateState(nodeId, User);
            return http.RespondSuccess();
        }
    }
}