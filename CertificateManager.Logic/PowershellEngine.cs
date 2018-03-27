using CertificateManager.Entities;
using CertificateManager.Logic.Interfaces;
using System;
using System.Linq;
using System.Management.Automation.Language;
using System.Security.Claims;

namespace CertificateManager.Logic
{
    public class PowershellEngine : IPowershellEngine
    {
        IAuditLogic auditLogic;

        public PowershellEngine(IAuditLogic auditLogic)
        {
            this.auditLogic = auditLogic;
        }

        public void ValidateSyntax(Script script, ClaimsPrincipal user)
        {
            Token[] token = null;

            ParseError[] errors = null;

            ScriptBlockAst result = Parser.ParseInput(script.Code, out token, out errors);

            if(errors != null && errors.Length > 0)
            {
                string msg = string.Concat(Environment.NewLine, errors.Select(error => error.Message));

                auditLogic.LogOpsError(user, script.Name, EventCategory.JobError, msg);

                throw new System.Exception("Error while parsing script");
            }
        }


    }
}
