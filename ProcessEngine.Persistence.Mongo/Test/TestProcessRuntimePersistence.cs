using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Klaudwerk.PropertySet;
using KlaudWerk.ProcessEngine;
using KlaudWerk.ProcessEngine.Builder;
using KlaudWerk.ProcessEngine.Definition;
using KlaudWerk.ProcessEngine.Persistence;
using KlaudWerk.ProcessEngine.Persistence.Test;
using KlaudWerk.ProcessEngine.Runtime;
using KlaudWerk.ProcessEngine.Test;
using MongoDB.Driver;
using Moq;
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

        [Test]
        public void TestContinueAfterSuspending()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "com.klaudwerk.workflow.renewal",
                name: "Renewal", description: "Policy Renewal");
            IReadOnlyList<ProcessValidationResult> result;
            builder.Start("s_1").Handler().HumanTask().Done().SetName("Start").Done()
                    .Step("s_2").Handler().HumanTask().Done().Done()
                    .Step("s_3").Handler().HumanTask().Done().Done()
                    .End("e_1").SetName("End Process").Done()
                    .Link().From("s_1").To("s_2").Name("s_1_s_2").Done()
                    .Link().From("s_2").To("s_3").Name("s_2_s_3").Done()
                    .Link().From("s_3").To("e_1").Name("end").Done()
                .TryValidate(out result);

            ProcessDefinition processDefinition = builder.Build();
            IProcessDefinitionPersisnenceService service = GetProcessDefinitionPersistenceService();
            service.Create(processDefinition, ProcessDefStatusEnum.Active, 1);

            IProcessRuntimeService pservice = GetProcessRuntime();
            PropertySetCollection collection = new PropertySetCollection(new PropertySchemaSet(new PropertySchemaFactory()));
            Mock<IProcessRuntimeEnvironment> mEnv = new Mock<IProcessRuntimeEnvironment>();
            mEnv.SetupGet(m => m.PropertySet).Returns(collection).Verifiable();
            mEnv.Setup(m => m.TaskServiceAsync())
                .Returns(() =>
                    Task.FromResult(new ExecutionResult(StepExecutionStatusEnum.Suspend)))
                .Verifiable();

            IProcessRuntime runtime = pservice.Create(processDefinition, collection);
            string[] errors;
            runtime.TryCompile(out errors);

            IProcessRuntime ufRuntime;
            StepRuntime ufStep;
            IPropertySetCollection ufCollection;
            Tuple<ExecutionResult, StepRuntime> execute = runtime.Execute(runtime.StartSteps[0], mEnv.Object);
            Assert.IsNotNull(execute);
            Assert.AreEqual(StepExecutionStatusEnum.Suspend,execute.Item1.Status,"The Workflow should be in Suspended state");
            Assert.AreEqual(execute.Item2.StepId,"s_1");
            execute = runtime.Continue(mEnv.Object);
            Assert.IsNotNull(execute);
            Assert.AreEqual(StepExecutionStatusEnum.Ready,execute.Item1.Status,"The Workflow should be in Suspended state");
            Assert.AreEqual(execute.Item2.StepId,"s_2");

            pservice.TryUnfreeze(runtime.Id, out ufRuntime, out ufStep, out ufCollection);
            Assert.IsNotNull(ufRuntime);
            Assert.IsNotNull(ufStep);
            Assert.AreEqual("s_2",ufStep.StepId);
        }

        [Test]
        public void TestContinueTaskWithLink()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "com.klaudwerk.workflow.renewal",
                name: "Renewal", description: "Policy Renewal");
            IReadOnlyList<ProcessValidationResult> result;
            builder.Start("s_1").Handler().HumanTask().Done().SetName("Start").Done()
                .Step("s_2").Handler().HumanTask().Done().Done()
                .Step("s_3").Handler().HumanTask().Done().Done()
                .End("e_1").SetName("End Process").Done()
                .Link().From("s_1").To("s_2").Name("s_1_s_2").Done()
                .Link().From("s_1").To("s_3").Name("s_1_s_3").Done()
                .Link().From("s_2").To("e_1").Name("s2_end").Done()
                .Link().From("s_3").To("e_1").Name("s3_end").Done()
                .TryValidate(out result);

            ProcessDefinition processDefinition = builder.Build();
            IProcessDefinitionPersisnenceService service = GetProcessDefinitionPersistenceService();
            service.Create(processDefinition, ProcessDefStatusEnum.Active, 1);

            IProcessRuntimeService pservice = GetProcessRuntime();
            PropertySetCollection collection = new PropertySetCollection(new PropertySchemaSet(new PropertySchemaFactory()));
            Mock<IProcessRuntimeEnvironment> mEnv = new Mock<IProcessRuntimeEnvironment>();
            mEnv.SetupGet(m => m.PropertySet).Returns(collection).Verifiable();
            mEnv.Setup(m => m.TaskServiceAsync())
                .Returns(() =>
                    Task.FromResult(new ExecutionResult(StepExecutionStatusEnum.Suspend)))
                .Verifiable();
            mEnv.SetupGet(m => m.Transition).Returns("s_1_s_3");
            IProcessRuntime runtime = pservice.Create(processDefinition, collection);
            string[] errors;
            runtime.TryCompile(out errors);
            Tuple<ExecutionResult, StepRuntime> execute = runtime.Execute(runtime.StartSteps[0], mEnv.Object);
            Assert.IsNotNull(execute);
            Assert.AreEqual(StepExecutionStatusEnum.Suspend,execute.Item1.Status,"The Workflow should be in Suspended state");
            Assert.AreEqual(execute.Item2.StepId,"s_1");
            execute = runtime.Continue(mEnv.Object);
            Assert.IsNotNull(execute);
            Assert.AreEqual(StepExecutionStatusEnum.Ready,execute.Item1.Status,"The Workflow should be in Suspended state");
            Assert.AreEqual(execute.Item2.StepId,"s_3");

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