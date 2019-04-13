using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Klaudwerk.PropertySet;
using KlaudWerk.ProcessEngine;
using KlaudWerk.ProcessEngine.Builder;
using KlaudWerk.ProcessEngine.Definition;
using KlaudWerk.ProcessEngine.Persistence;
using KlaudWerk.ProcessEngine.Persistence.Test;
using KlaudWerk.ProcessEngine.Runtime;
using KlaudWerk.ProcessEngine.Test;
using MongoDB.Bson;
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
        private AccountData[] accounts;

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
        public void TestRetrieveSimplePersistentRuntimeSummary()
        {
            var runtime = OnTestCreateSimplePersistentRuntime();
            IProcessRuntimeService pservice = GetProcessRuntime();
            ProcessRuntimeSummary summary;
            Assert.IsTrue(pservice.TryGetProcessSummary(runtime.Id, out summary));
            Assert.IsNotNull(summary);
            Assert.AreEqual(runtime.Id,summary.Id);
            Assert.AreEqual(runtime.State,summary.Status);
            Assert.IsFalse(string.IsNullOrEmpty(summary.FlowDefinitionId));
            Assert.IsFalse(string.IsNullOrEmpty(summary.FlowDefinitionMd5));

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
        public void TestContinueAfterSuspendingDeactivatedFlow()
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
            Assert.AreEqual(StepExecutionStatusEnum.Suspend, execute.Item1.Status, "The Workflow should be in Suspended state");
            Assert.AreEqual(execute.Item2.StepId, "s_1");
            execute = runtime.Continue(mEnv.Object);
            Assert.IsNotNull(execute);
            Assert.AreEqual(StepExecutionStatusEnum.Ready, execute.Item1.Status, "The Workflow should be in Suspended state");
            Assert.AreEqual(execute.Item2.StepId, "s_2");
            //  Deactivate the workflow
            var workflow=service.LisAlltWorkflows().SingleOrDefault();
            service.SetStatus(workflow.Id,1, ProcessDefStatusEnum.NotActive);
            // deploy new workflow
            builder = factory.CreateProcess(id: "com.klaudwerk.workflow.renewal",
                            name: "Renewal", description: "Policy Renewal");
            builder.Start("s_1").Handler().HumanTask().Done().SetName("Start").Done()
                    .Step("s_2").Handler().HumanTask().Done().Done()
                    .Step("s_3").Handler().HumanTask().Done().Done()
                    .Step("s_4").Handler().HumanTask().Done().Done()
                    .End("e_1").SetName("End Process").Done()
                    .Link().From("s_1").To("s_2").Name("s_1_s_2").Done()
                    .Link().From("s_2").To("s_3").Name("s_2_s_3").Done()
                    .Link().From("s_3").To("s_4").Name("s_3_s_4").Done()
                    .Link().From("s_4").To("e_1").Name("end").Done()
                .TryValidate(out result);
            processDefinition = builder.Build();
            service.Create(processDefinition, ProcessDefStatusEnum.Active, 1);

            pservice.TryUnfreeze(runtime.Id, out ufRuntime, out ufStep, out ufCollection);
            Assert.IsNotNull(ufRuntime);
            Assert.IsNotNull(ufStep);
            Assert.AreEqual("s_2", ufStep.StepId);
        
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

        [Test]
        public void TestContinueTaskWithLinkAnVariables()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "com.klaudwerk.workflow.renewal",
                name: "Renewal", description: "Policy Renewal");
            IReadOnlyList<ProcessValidationResult> result;
            builder
                .Variables().Name("v_1").Type(VariableTypeEnum.String).Done()
                .Variables().Name("v_2").Type(VariableTypeEnum.String).Done()
                .Start("s_1").Handler().HumanTask().Done().SetName("Start")
                    .Vars().Name("v_1").Done()
                    .Vars().Name("v_2").Done()
                    .Done()
                .Step("s_2").Handler().HumanTask().Done()
                    .Done()
                .Step("s_3").Handler().HumanTask().Done()
                    .Vars().Name("v_1").OnExit().Done()
                    .Vars().Name("v_2").OnExit().Done()
                    .Done()
                .End("e_1").SetName("End Process").Done()
                .Link().From("s_1").To("s_2").Name("s_1_s_2").Done()
                .Link().From("s_1").To("s_3").Name("s_1_s_3").Done()
                .Link().From("s_2").To("e_1").Name("s2_end").Done()
                .Link().From("s_3").To("e_1").Name("s3_end").Done()
                .TryValidate(out result);

            ProcessDefinition processDefinition = builder.Build();
            IProcessDefinitionPersisnenceService service = GetProcessDefinitionPersistenceService();
            service.Create(processDefinition, ProcessDefStatusEnum.Active, 1);
            // retrieve process definition
            var flows=service.ActivetWorkflows();
            Assert.IsNotNull(flows);
            Assert.AreEqual(1, flows.Count);
            ProcessDefinition loadedPd;
            ProcessDefStatusEnum status;
            Assert.IsTrue(service.TryFind(flows[0].Id, flows[0].Version, out loadedPd, out status, out accounts));

            IProcessRuntimeService pservice = GetProcessRuntime();
            PropertySetCollection collection = new PropertySetCollection(new PropertySchemaSet(new PropertySchemaFactory()));
            Mock<IProcessRuntimeEnvironment> mEnv = new Mock<IProcessRuntimeEnvironment>();
            mEnv.SetupGet(m => m.PropertySet).Returns(collection).Verifiable();
            mEnv.Setup(m => m.TaskServiceAsync())
                .Returns(() =>
                    Task.FromResult(new ExecutionResult(StepExecutionStatusEnum.Suspend)))
                .Verifiable();
            mEnv.SetupGet(m => m.Transition).Returns("s_1_s_3");
            IProcessRuntime runtime = pservice.Create(loadedPd, collection);
            string[] errors;
            runtime.TryCompile(out errors);
            Tuple<ExecutionResult, StepRuntime> execute = runtime.Execute(runtime.StartSteps[0], mEnv.Object);
            Assert.IsNotNull(execute);
            Assert.AreEqual(StepExecutionStatusEnum.Suspend, execute.Item1.Status, "The Workflow should be in Suspended state");
            Assert.AreEqual(execute.Item2.StepId, "s_1");
            collection.Set("v_1", "v_1");
            collection.Set("v_2", "v_2");
            execute = runtime.Continue(mEnv.Object);
            Assert.IsNotNull(execute);
            Assert.AreEqual(StepExecutionStatusEnum.Ready, execute.Item1.Status, "The Workflow should be in Suspended state");
            Assert.AreEqual(execute.Item2.StepId, "s_3");
            Assert.IsNotNull(execute.Item2.StepDefinition.VariablesMap);
            Assert.AreEqual(2,execute.Item2.StepDefinition.VariablesMap.Length);
            Assert.AreEqual(VarRequiredEnum.OnExit, execute.Item2.StepDefinition.VariablesMap[0].Required);
            Assert.AreEqual(VarRequiredEnum.OnExit, execute.Item2.StepDefinition.VariablesMap[1].Required);

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