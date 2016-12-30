using System;
using System.Collections.Generic;
using System.Linq;
using KlaudWerk.ProcessEngine.Persistence;
using KlaudWerk.ProcessEngine.Persistence.Test;
using MongoDB.Driver;
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

        protected override IProcessDefinitionPersisnenceService InstService()
        {
            return new MongoProcessDefinitionPersistenceService(_database);
        }
    }
}