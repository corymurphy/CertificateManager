using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CertificateManager.Repository;
using CertificateManager.Entities;
using CertificateManager.Entities.Enumerations;
using System.Collections.Generic;
using System.Linq;
using LiteDB;

namespace CertificateManager.IntegrationTests
{
    [TestClass]
    public class UnitTest1
    {
        private const string authenticablePrincipalCollectionName = "usr";
        private const string path = @"D:\db\config.db";
        [TestMethod]
        public void Test1()
        {
            string upn = "cmurphy";
            Guid domainId = new Guid("2cb9c0d8-0a6c-4943-821d-1119df6d3e58");
            Guid cmurphyId = new Guid("b64ec359-fb8d-4de9-81d5-df59e2f24454");

            LiteDbConfigurationRepository repo =  new LiteDbConfigurationRepository(path);
            //var result = repo.ExternalIdentitySourceExists(domainId);

            var allUsers = repo.GetAuthenticablePrincipals();

            var b = repo.UserPrincipalNameExists(upn);
            var a = repo.UserPrincipalNameExists(upn, cmurphyId);

            //var a = repo.GetExternalIdentitySources();

            LiteDatabase db = new LiteDatabase(path);
            LiteCollection<AuthenticablePrincipal> col = db.GetCollection<AuthenticablePrincipal>(authenticablePrincipalCollectionName);

            var d = col.Find(Query.Not(Query.EQ("Id", cmurphyId)));
            //var d = col.Find(Query.Not("Id", cmurphyId));
            var c = col.Exists(Query.Not("Id", cmurphyId));


            Query query = Query.Or(
                    Query.In("AlternativeUserPrincipalNames", upn),
                    Query.EQ("UserPrincipalName", upn)
                );

            var e = col.Find(query);
            var eany = e.Any();
            var e2 = e.Where(user => user.Id == cmurphyId).Any();

            query = Query.Or(
                    Query.In("AlternativeUserPrincipalNames", "derp"),
                    Query.EQ("UserPrincipalName", "derp")
                );

            var f = col.Find(query);
            var fany = f.Any();
            
            var f2 = f.Where(user => user.Id == cmurphyId).Any();

            Console.Write("");
        //    Guid userId = new Guid("b64ec359 -fb8d-4de9-81d5-df59e2f24454");

        //    using (LiteDatabase db = new LiteDatabase(@"D:\db\config.db"))
        //    {
        //        LiteCollection<AuthenticablePrincipal> col = db.GetCollection<AuthenticablePrincipal>("usr");
        //        AuthenticablePrincipal user = col.FindById(userId);

        //        Console.Write(user);
        //        user.AlternativeUserPrincipalNames = new List<string>();
        //        user.AlternativeUserPrincipalNames.Add("cory.murphy@fakedomain.com");
        //        user.AlternativeUserPrincipalNames.Add("cmurphy@fakedomain.com");

        //        col.Update(user);
        //    }

        }

        [TestMethod]
        public void Test()
        {
            Guid id = new Guid("afa19bf8-562f-4744-baf1-ad04ac085874");
            LiteDbConfigurationRepository config = new LiteDbConfigurationRepository(@"D:\db\config.db");
            AuthenticablePrincipal principal = config.GetAuthenticablePrincipal(id);
            //IEnumerable<SecurityRole> memberOf = config.GetAuthenticablePrincipalMemberOf(principal.Id);


            using (LiteDatabase db = new LiteDatabase(@"D:\db\config.db"))
            {
                LiteCollection<SecurityRole> col = db.GetCollection<SecurityRole>("secroles");
                var d = col.FindAll();
                var c = col.FindAll().Where(role => role.Member.Contains(id));
                var a = col.Find( Query.In("Member", id ) );
                var b = col.Find(Query.Contains("Member", id.ToString()));
                throw new Exception();
            }


            throw new Exception();
        }



        [TestMethod]
        public void TestMethod1()
        {
            RuntimeCacheRepository repo = new RuntimeCacheRepository(@"D:\db\runtimecache.db");
            
            var a = repo.ConfigurationAlertExists(AlertType.ApplicationStartedSuccessfully);

            //repo.InsertConfigurationAlert<ConfigurationAlert>(new ConfigurationAlert()
            //{
            //    AlertSeverity = AlertSeverity.Information,
            //    AlertState = AlertState.Open,
            //    AlertType = AlertType.ApplicationStartedSuccessfully,
            //    Created = DateTime.Now,
            //    Id = Guid.NewGuid(),
            //    Message = "Application Started Successfully"
            //});

            LiteDbConfigurationRepository config = new LiteDbConfigurationRepository(@"D:\db\config.db");

            IEnumerable<AdcsTemplate> found = config.GetAdcsTemplates();

            var b = found.Any();
            var c = repo.ConfigurationAlertExists(AlertType.NoTemplatesConfigured);

            //runtimeConfigurationState.ClearAlert(AlertType.NoTemplatesConfigured);

            if (!b && !c)
            {
                Console.Write("here");
            }
            repo.InsertConfigurationAlert(new ConfigurationAlert()
            {
                Created = DateTime.Now,
                AlertState = AlertState.Open,
                AlertSeverity = AlertSeverity.Error,
                AlertType = AlertType.NoTemplatesConfigured,
                Id = Guid.NewGuid(),
                Message = "There must be at least one template before certificate manager can issue new certificates."
            });
            var results = repo.GetConfigurationAlerts();

            Console.Write("derp");
        }
    }
}
