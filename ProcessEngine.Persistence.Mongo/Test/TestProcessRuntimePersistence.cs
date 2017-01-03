using KlaudWerk.ProcessEngine.Persistence;
using KlaudWerk.ProcessEngine.Persistence.Test;
using KlaudWerk.ProcessEngine.Runtime;
using MongoDB.Driver;
using NUnit.Framework;

namespace Klaudwerk.ProcessEngine.Persistence.Mongo.Test
{
    [TestFixture]
    public class TestProcessRuntimePersistence : TestProcessRuntimePersistenceBase
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
            _database.DropCollection(MongoProcessRuntimePersistenceService.CollectionName);
            _database.DropCollection(MongoProcessDefinitionPersistenceService.CollectionName);
        }

        [Test]
        public void TestCreateSimplePersistentRuntime()
        {
            OnTestCreateSimplePersistentRuntime();
        }

        [Test]
        public void TestLoadSavedRuntimeProcess()
        {
            OnTestLoadSimpleRuntimeProcess();
        }

        [Test]
        public void TestExecuteWorkflowFormSuspendedState()
        {
            OnTestExecuteWorkflowFromSuspendedStep();
        }

        [Test]
        public void TestExecuteMultipleStepsSaveStateBetweenSteps()
        {
            OnTestExecuteMultipleStepsSaveStateBetweenSteps();
        }

        protected override IProcessDefinitionPersisnenceService GetProcessDefinitionPersistenceService()
        {
            return new MongoProcessDefinitionPersistenceService(_database);
        }

        protected override IProcessRuntimeService GetProcessRuntime()
        {
            return new MongoProcessRuntimePersistenceService(_database);
        }

    }
}