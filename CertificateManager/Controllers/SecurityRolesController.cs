using CertificateManager.Entities;
using CertificateManager.Logic;
using CertificateManager.Repository;
using Microsoft.AspNetCore.Mvc;
using System;


namespace CertificateManager.Controllers
{
    public class SecurityRolesController : Controller
    {
        IConfigurationRepository configurationRepository;
        HttpResponseHandler http;
        RoleManagementLogic roleManagement;

        public SecurityRolesController(IConfigurationRepository configurationRepository)
        {
            this.configurationRepository = configurationRepository;
            this.http = new HttpResponseHandler(this);
            this.roleManagement = new RoleManagementLogic(configurationRepository);
        }

        [HttpGet]
        [Route("view/security/roles")]
        public ActionResult RolesView()
        {
            return View("View");
        }

        [HttpGet]
        [Route("view/security/role/{id:guid}")]
        public ActionResult ViewRole(Guid id)
        {
            ViewBag.RoleId = id;
            return View("ViewRole");
        }

        [HttpGet]
        [Route("security/role/{id:guid}")]
        public JsonResult GetRole(Guid id)
        {
            return http.RespondSuccess(roleManagement.GetRole(id));
        }

        [HttpGet]
        [Route("security/roles")]
        public JsonResult GetAllRoles()
        {
            return http.RespondSuccess(roleManagement.GetRoles());
        }

        [HttpPost]
        [Route("security/role")]
        public JsonResult AddRole(SecurityRole entity)
        {
            return http.RespondSuccess(roleManagement.AddRole(entity));
        }

        [HttpDelete]
        [Route("security/role")]
        public JsonResult DeleteRole(SecurityRole entity)
        {
            roleManagement.DeleteRole(entity);
            return http.RespondSuccess();
        }
    
        [HttpPut]
        [Route("security/role")]
        public JsonResult UpdateRole(SecurityRole entity)
        {
            roleManagement.UpdateRole(entity);
            return http.RespondSuccess();
        }

        [HttpGet]
        [Route("security/role/{id:guid}/members")]
        public JsonResult GetRoleMembers(Guid id)
        {
            return http.RespondSuccess(roleManagement.GetRoleMembers(id));
        }

        [HttpDelete]
        [Route("security/role/{roleId:guid}/member/{memberId:guid}")]
        public JsonResult DeleteRoleMember(Guid roleId, Guid memberId)
        {
            roleManagement.DeleteRoleMember(roleId, memberId);
            return http.RespondSuccess();
        }

        [HttpPost]
        [Route("security/role/{roleId:guid}/member/{memberId:guid}")]
        public JsonResult AddRoleMember(Guid roleId, Guid memberId)
        {
            return http.RespondSuccess(roleManagement.AddRoleMember(roleId, memberId));
        }
    }
}