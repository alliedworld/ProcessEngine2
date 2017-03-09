using System;
using System.Collections.Generic;
using System.Linq;
using KlaudWerk.ProcessEngine;
using KlaudWerk.ProcessEngine.Definition;
using KlaudWerk.ProcessEngine.Persistence;
using KlaudWerk.ProcessEngine.Persistence.Test;
using MongoDB.Driver;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Klaudwerk.ProcessEngine.Persistence.Mongo.Test
{
    [TestFixture]
    public class TestProcessDefinitionPersistence:TestProcessDefinitionPersistenceBase
    {
        IMongoClient _client = new MongoClient();
        IMongoDatabase _database;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            _client=new MongoClient();
            _database = _client.GetDatabase("workflows");
        }

        [TestFixtureTearDown]
        public void FixtureTeardown()
        {
            _client.DropDatabase("workflows");
        }
        [SetUp]
        public void SetUp()
        {

        }
        [TearDown]
        public void TearDown()
        {
            _database.GetCollection<ProcessDefinitionPersistence>(MongoProcessDefinitionPersistenceService.CollectionName).DeleteMany(p => true);
        }

        [Test]
        public void TestCreateProcessDefinition()
        {
            var collection = _database.GetCollection<ProcessDefinitionPersistence>(MongoProcessDefinitionPersistenceService.CollectionName);
            Assert.IsNotNull(collection);
            Assert.AreEqual(0,collection.Count(pd=>true));
            OnCreateAndListProcessDefinition(InstService());
            Assert.AreEqual(1,collection.Count(pd=>true));
        }

        [Test]
        public void TestCreateFlowWithAssociatedSecurityAccounts()
        {
            var collection = _database.GetCollection<ProcessDefinitionPersistence>(MongoProcessDefinitionPersistenceService.CollectionName);
            IProcessDefinitionPersisnenceService service=InstService();
            AccountData[] accounts = new[]
            {
                new AccountData
                {
                    AccountType = 1,
                    Id = Guid.NewGuid(),
                    Name = "Underwriters",
                    SourceSystem = "ActiveDirectory"
                },
                new AccountData
                {
                    AccountType = 1,
                    Id = Guid.NewGuid(),
                    Name = "Modeler",
                    SourceSystem = "ActiveDirectory"
                },
            };

            OnCreateFlowWithAssociatedSecutityAccounts(service,accounts);
            Assert.AreEqual(1,collection.Count(pd=>true));
            FilterDefinitionBuilder<ProcessDefinitionPersistence> fb = Builders<ProcessDefinitionPersistence>.Filter;
            var fd = fb.In("Accounts.Name", new string[] { "Underwriters" });
            List<ProcessDefinitionPersistence> pds = collection.Find(fd).ToList();
            Assert.IsNotNull(pds);
            Assert.AreEqual(1,pds.Count);
            Assert.IsNotNull(pds[0].Accounts);
            Assert.AreEqual(2,pds[0].Accounts.Count);
        }

        [Test]
        public void TestRetrieveActiveProcessDefitinionsListsForAccounts()
        {
            AccountData[] accounts = new[]
            {
                new AccountData
                {
                    AccountType = 1,
                    Id = Guid.NewGuid(),
                    Name = "Underwriters",
                    SourceSystem = "ActiveDirectory"
                },
                new AccountData
                {
                    AccountType = 1,
                    Id = Guid.NewGuid(),
                    Name = "Modeler",
                    SourceSystem = "ActiveDirectory"
                },
                new AccountData
                {
                    AccountType = 1,
                    Id = Guid.NewGuid(),
                    Name = "Role1",
                    SourceSystem = "ActiveDirectory"
                },
                new AccountData
                {
                    AccountType = 1,
                    Id = Guid.NewGuid(),
                    Name = "Role2",
                    SourceSystem = "ActiveDirectory"
                },
                new AccountData
                {
                    AccountType = 1,
                    Id = Guid.NewGuid(),
                    Name = "None",
                    SourceSystem = "ActiveDirectory"
                }
            };
            var collection = _database.GetCollection<ProcessDefinitionPersistence>(MongoProcessDefinitionPersistenceService.CollectionName);
            IProcessDefinitionPersisnenceService service=InstService();
            ProcessDefinition pd1 = BuildProcessdefinition();
            ProcessDefinition pd2 = BuildProcessdefinition(id:"mongo.flow",name:"pd_1",description:"second flow");
            service.Create(pd1,ProcessDefStatusEnum.Active, 1, accounts[0],accounts[1]);
            service.Create(pd2,ProcessDefStatusEnum.Active, 1, accounts[2],accounts[3]);
            //List Underwriter should return one definition
            IReadOnlyList<ProcessDefinitionDigest> digest = service.ActivetWorkflows(accounts[0].Name);
            Assert.IsNotNull(digest);
            Assert.AreEqual(1,digest.Count);
            Assert.AreEqual(pd1.Name,digest[0].Name);

            digest = service.ActivetWorkflows(accounts[1].Name,accounts[3].Name);
            Assert.IsNotNull(digest);
            Assert.AreEqual(2,digest.Count);
            Assert.AreEqual(1,digest.Count(c=>c.Name==pd1.Name));
            Assert.AreEqual(1,digest.Count(c=>c.Name==pd2.Name));

            digest = service.ActivetWorkflows(accounts[4].Name);
            Assert.IsNotNull(digest);
            Assert.AreEqual(0,digest.Count);
        }

        [Test]
        public void TestRetrieveProcessDefinitionListsForAccounts()
        {
            AccountData[] accounts = new[]
            {
                new AccountData
                {
                    AccountType = 1,
                    Id = Guid.NewGuid(),
                    Name = "Underwriters",
                    SourceSystem = "ActiveDirectory"
                },
                new AccountData
                {
                    AccountType = 1,
                    Id = Guid.NewGuid(),
                    Name = "Modeler",
                    SourceSystem = "ActiveDirectory"
                },
                new AccountData
                {
                    AccountType = 1,
                    Id = Guid.NewGuid(),
                    Name = "Role1",
                    SourceSystem = "ActiveDirectory"
                },
                new AccountData
                {
                    AccountType = 1,
                    Id = Guid.NewGuid(),
                    Name = "Role2",
                    SourceSystem = "ActiveDirectory"
                },
                new AccountData
                {
                    AccountType = 1,
                    Id = Guid.NewGuid(),
                    Name = "None",
                    SourceSystem = "ActiveDirectory"
                }
            };
            var collection = _database.GetCollection<ProcessDefinitionPersistence>(MongoProcessDefinitionPersistenceService.CollectionName);
            IProcessDefinitionPersisnenceService service=InstService();
            ProcessDefinition pd1 = BuildProcessdefinition();
            ProcessDefinition pd2 = BuildProcessdefinition(id:"mongo.flow",name:"pd_1",description:"second flow");
            service.Create(pd1,ProcessDefStatusEnum.Active, 1, accounts[0],accounts[1]);
            service.Create(pd2,ProcessDefStatusEnum.NotActive, 1, accounts[2],accounts[3]);
            //List Underwriter should return one definition
            IReadOnlyList<ProcessDefinitionDigest> digest = service.LisAlltWorkflows(accounts[0].Name);
            Assert.IsNotNull(digest);
            Assert.AreEqual(1,digest.Count);
            Assert.AreEqual(pd1.Name,digest[0].Name);

            digest = service.LisAlltWorkflows(accounts[1].Name,accounts[3].Name);
            Assert.IsNotNull(digest);
            Assert.AreEqual(2,digest.Count);
            Assert.AreEqual(1,digest.Count(c=>c.Name==pd1.Name));
            Assert.AreEqual(1,digest.Count(c=>c.Name==pd2.Name));

            digest = service.LisAlltWorkflows(accounts[4].Name);
            Assert.IsNotNull(digest);
            Assert.AreEqual(0,digest.Count);
        }
        [Test]
        public void TestFindCreatedDefinition()
        {
            var collection = _database.GetCollection<ProcessDefinitionPersistence>(MongoProcessDefinitionPersistenceService.CollectionName);
            Assert.IsNotNull(collection);
            Assert.AreEqual(0,collection.Count(pd=>true));
            OnCreateAndListProcessDefinition(InstService());
            Assert.AreEqual(1,collection.Count(pd=>true));

        }
        [Test]
        public void TestCreatePorcessUsingExistingAccounts()
        {
            IProcessDefinitionPersisnenceService service=InstService();
            OnCreateProcessUsingExistingAccounts(service);
        }
        [Test]
        public void TestChangePorcessDefinitionStatus()
        {
            IProcessDefinitionPersisnenceService service=InstService();
            OnChangeProcessDefinitionStatus(service);
        }
        [Test]
        public void TestTryFindFlow()
        {
            IProcessDefinitionPersisnenceService service=InstService();
            OnTryFindFlow(service);
        }
        [Test]
        public void TestFindNonExistingFlowShouldReturnFalse()
        {
            IProcessDefinitionPersisnenceService service=InstService();
            OnFindNonExistingFlowShouldReturnFalse(service);
        }
        [Test]
        public void SaveDefinitionWithTheSameMd5SameVersionShouldFail()
        {
            IProcessDefinitionPersisnenceService service=InstService();
            OnSaveDefinitionWithTheSameMd5ShouldFail(service);
        }
        [Test]
        public void ShouldBePossibleToUpdateNameAndDescription()
        {
            IProcessDefinitionPersisnenceService service=InstService();
            OnPossibleToUpdateNameAndDescription(service);
        }

        [Test]
        public void TestCreateProcessDefinitionWithScriptAndRetrieve()
        {
            ProcessDefinition pd = BuildProcessdefinition("id.123", "test_definition", "description");
            Md5CalcVisitor visitor=new Md5CalcVisitor();
            pd.Accept(visitor);
            string md5 = visitor.CalculateMd5();
            IProcessDefinitionPersisnenceService service = InstService();
            service.Create(pd,ProcessDefStatusEnum.Active,1);
            IReadOnlyList<ProcessDefinitionDigest> flows = service.LisAlltWorkflows();
            Assert.IsNotNull(flows);
            Assert.AreEqual(1,flows.Count);
            ProcessDefStatusEnum stat;
            ProcessDefinition savedPd;
            AccountData[] accounts;
            Assert.IsTrue(service.TryFind(flows[0].Id, flows[0].Version, out savedPd, out stat, out accounts));

            Assert.IsNotNull(savedPd);
            visitor.Reset();
            savedPd.Accept(visitor);
            string savedMd5 = visitor.CalculateMd5();
            Assert.AreEqual(md5, savedMd5);

        }
        [Test]
        public void TestAddRolesShouldAddNewRoles()
        {
            var collection = _database.GetCollection<ProcessDefinitionPersistence>(MongoProcessDefinitionPersistenceService.CollectionName);
            IProcessDefinitionPersisnenceService service = InstService();
            AccountData[] accounts = new[]
            {
                new AccountData
                {
                    AccountType = 1,
                    Id = Guid.NewGuid(),
                    Name = "Underwriter",
                    SourceSystem = "ActiveDirectory"
                },
                new AccountData
                {
                    AccountType = 1,
                    Id = Guid.NewGuid(),
                    Name = "Modeler",
                    SourceSystem = "ActiveDirectory"
                },
            };

            OnCreateFlowWithAssociatedSecutityAccounts(service, accounts);
            Assert.AreEqual(1, collection.Count(pd => true));
            AccountData[] accountsToAdd = new[]
            {
                new AccountData
                {
                    AccountType = 2,
                    Id = Guid.NewGuid(),
                    Name = "London",
                    SourceSystem = "ActiveDirectory"
                },
                new AccountData
                {
                    AccountType = 1,
                    Id = Guid.NewGuid(),
                    Name = "Administrator",
                    SourceSystem = "ActiveDirectory"
                }
            };
            IReadOnlyList<ProcessDefinitionDigest> flows = service.LisAlltWorkflows();
            Assert.IsNotNull(flows);
            Assert.AreEqual(1,flows.Count);
            ProcessDefinitionDigest flow = flows.ElementAt(0);
            service.AddRoles(flow.Id,flow.Version,accountsToAdd);
            var idFilter 
                = Builders<ProcessDefinitionPersistence>.Filter.Eq(r=>r.Id,flow.Id);
            ProcessDefinitionPersistence persistence = collection.FindSync(idFilter).FirstOrDefault();
            Assert.IsNotNull(persistence);
            Assert.IsNotNull(persistence.Accounts);
            Assert.AreEqual(4,persistence.Accounts.Count);
            Assert.AreEqual(1, persistence.Accounts.Count(a=>a.Name=="London"));
            Assert.AreEqual(1, persistence.Accounts.Count(a => a.Name == "Administrator"));
            Assert.AreEqual(1, persistence.Accounts.Count(a => a.Name == "Underwriter"));
            Assert.AreEqual(1, persistence.Accounts.Count(a => a.Name == "Modeler"));
        }

        [Test]
        public void TestAddRolesShouldSkipExistingRoles()
        {
            var collection = _database.GetCollection<ProcessDefinitionPersistence>(MongoProcessDefinitionPersistenceService.CollectionName);
            IProcessDefinitionPersisnenceService service = InstService();
            AccountData[] accounts = new[]
            {
                new AccountData
                {
                    AccountType = 1,
                    Id = Guid.NewGuid(),
                    Name = "Underwriter",
                    SourceSystem = "ActiveDirectory"
                },
                new AccountData
                {
                    AccountType = 1,
                    Id = Guid.NewGuid(),
                    Name = "Modeler",
                    SourceSystem = "ActiveDirectory"
                },
            };

            OnCreateFlowWithAssociatedSecutityAccounts(service, accounts);
            Assert.AreEqual(1, collection.Count(pd => true));
            AccountData[] accountsToAdd = new[]
            {
                new AccountData
                {
                    AccountType = 2,
                    Id = Guid.NewGuid(),
                    Name = "London",
                    SourceSystem = "ActiveDirectory"
                },
                new AccountData
                {
                    AccountType = 1,
                    Id = Guid.NewGuid(),
                    Name = "Underwriter",
                    SourceSystem = "ActiveDirectory"
                }
            };
            IReadOnlyList<ProcessDefinitionDigest> flows = service.LisAlltWorkflows();
            Assert.IsNotNull(flows);
            Assert.AreEqual(1, flows.Count);
            ProcessDefinitionDigest flow = flows.ElementAt(0);
            service.AddRoles(flow.Id, flow.Version, accountsToAdd);
            var idFilter
                = Builders<ProcessDefinitionPersistence>.Filter.Eq(r => r.Id, flow.Id);
            ProcessDefinitionPersistence persistence = collection.FindSync(idFilter).FirstOrDefault();
            Assert.IsNotNull(persistence);
            Assert.IsNotNull(persistence.Accounts);
            Assert.AreEqual(3, persistence.Accounts.Count);
            Assert.AreEqual(1, persistence.Accounts.Count(a => a.Name == "London"));
            Assert.AreEqual(1, persistence.Accounts.Count(a => a.Name == "Underwriter"));
            Assert.AreEqual(1, persistence.Accounts.Count(a => a.Name == "Modeler"));

        }

        [Test]
        public void TestRemoveRolesShouldRemoveSpecificRoles()
        {
            var collection = _database.GetCollection<ProcessDefinitionPersistence>(MongoProcessDefinitionPersistenceService.CollectionName);
            IProcessDefinitionPersisnenceService service = InstService();
            AccountData[] accounts = new[]
            {
                new AccountData
                {
                    AccountType = 1,
                    Id = Guid.NewGuid(),
                    Name = "Underwriter",
                    SourceSystem = "ActiveDirectory"
                },
                new AccountData
                {
                    AccountType = 1,
                    Id = Guid.NewGuid(),
                    Name = "Modeler",
                    SourceSystem = "ActiveDirectory"
                },
            };

            OnCreateFlowWithAssociatedSecutityAccounts(service, accounts);
            Assert.AreEqual(1, collection.Count(pd => true));
            IReadOnlyList<ProcessDefinitionDigest> flows = service.LisAlltWorkflows();
            Assert.IsNotNull(flows);
            Assert.AreEqual(1, flows.Count);
            ProcessDefinitionDigest flow = flows.ElementAt(0);  
            service.RemoveRoles(flow.Id,flow.Version,accounts[0]);
            var idFilter
                = Builders<ProcessDefinitionPersistence>.Filter.Eq(r => r.Id, flow.Id);
            ProcessDefinitionPersistence persistence = collection.FindSync(idFilter).FirstOrDefault();
            Assert.IsNotNull(persistence);
            Assert.IsNotNull(persistence.Accounts);
            Assert.AreEqual(1, persistence.Accounts.Count);
            Assert.AreEqual(accounts[1].Name,persistence.Accounts[0].Name);

        }
        protected override IProcessDefinitionPersisnenceService InstService()
        {
            return new MongoProcessDefinitionPersistenceService(_database);
        }
    }
}