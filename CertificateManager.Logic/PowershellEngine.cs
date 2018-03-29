using CertificateManager.Entities;
using CertificateManager.Logic.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Language;
using System.Security;
using System.Security.Claims;

namespace CertificateManager.Logic
{
    public class PowershellEngine : IPowershellEngine
    {
        IAuditLogic auditLogic;
        IScriptManagementLogic scriptManagement;

        public PowershellEngine(IAuditLogic auditLogic, IScriptManagementLogic scriptManagement)
        {
            this.scriptManagement = scriptManagement;
            this.auditLogic = auditLogic;
        }

        public PSCredential NewPSCredential(string username, string plaintext)
        {
            SecureString ss = new SecureString();
            foreach(char c in plaintext)
            {
                ss.AppendChar(c);
            }

            PSCredential cred = new PSCredential(username, ss);
            return cred;
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

        public Collection<T> InvokeScriptAsync<T>(string scriptName, Dictionary<string, object> cmdletParams, ClaimsPrincipal user)
        {
            using (PowerShell powerShell = PowerShell.Create())
            {
                Script script = scriptManagement.GetByName(scriptName, user);

                powerShell.AddScript(script.Code);

                powerShell.Invoke();

                powerShell.Commands.Clear();

                powerShell.AddCommand(scriptName);

                foreach(var psParam in cmdletParams)
                {
                    powerShell.AddParameter(psParam.Key, psParam.Value);
                }

                return powerShell.Invoke<T>();
            }
        }
    }
}
