using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Klaudwerk.PropertySet;
using KlaudWerk.ProcessEngine.Builder;
using KlaudWerk.ProcessEngine.Definition;
using KlaudWerk.ProcessEngine.Runtime;
using KlaudWerk.ProcessEngine.Test;
using Moq;
using NUnit.Framework;

namespace KlaudWerk.ProcessEngine.Persistence.Test
{
    [TestFixture]
    public class TestProcessRuntimePersistence
    {
        [SetUp]
        public void SetUp()
        {

        }

        [TearDown]
        public void TearDown()
        {
            using (var ctx = new ProcessDbContext())
            {
                ctx.Database.ExecuteSqlCommand("delete from  PROCESSES");
                ctx.Database.ExecuteSqlCommand("delete from PROCESS_DEFINITION_ACCOUNT");
                ctx.Database.ExecuteSqlCommand("delete from ACCOUNTS");
                ctx.Database.ExecuteSqlCommand("delete from  PROCESS_DEFINITIONS");
                ctx.Database.ExecuteSqlCommand("delete from  PROPERTY_ELEMENTS");
                ctx.Database.ExecuteSqlCommand("delete from  PROPERTY_SCHEMA_ELEMENTS");
                ctx.Database.ExecuteSqlCommand("delete from  PROPERTIES_COLLECTIONS");
            }
        }

        [Test]
        public void TestCreateSimplePersistentRuntime()
        {
            ProcessDefinitionPersisnenceService service=new ProcessDefinitionPersisnenceService();
            var processDefinition = BuildProcessdefinition();
            service.Create(processDefinition,ProcessDefStatusEnum.Active, 1);
            Md5CalcVisitor visitor=new Md5CalcVisitor();
            processDefinition.Accept(visitor);
            string md5 = visitor.CalculateMd5();
            ProcessRuntimePersistenceService pservice=new ProcessRuntimePersistenceService();
            PropertySetCollection collection=new PropertySetCollection(new PropertySchemaSet(new PropertySchemaFactory()));
            IProcessRuntime runtime = pservice.Create(processDefinition, collection);
            Assert.IsNotNull(runtime);
        }

        [Test]
        public void TestLoadSavedRuntimeProcess()
        {
            ProcessDefinitionPersisnenceService service=new ProcessDefinitionPersisnenceService();
            var processDefinition = BuildProcessdefinition();
            service.Create(processDefinition,ProcessDefStatusEnum.Active, 1);
            Md5CalcVisitor visitor=new Md5CalcVisitor();
            processDefinition.Accept(visitor);
            string md5 = visitor.CalculateMd5();
            ProcessRuntimePersistenceService pservice=new ProcessRuntimePersistenceService();
            PropertySetCollection collection=new PropertySetCollection(new PropertySchemaSet(new PropertySchemaFactory()));
            IProcessRuntime runtime = pservice.Create(processDefinition, collection);

            IProcessRuntime ufRuntime;
            StepRuntime nextStep;
            IPropertySetCollection ufCollection;
            bool val=pservice.TryUnfreeze(runtime.Id, out ufRuntime, out nextStep, out ufCollection);
            Assert.IsTrue(val);
            Assert.IsNotNull(ufRuntime);
            Assert.IsNotNull(ufCollection);
            Assert.AreEqual(ProcessStateEnum.NotStarted,ufRuntime.State);
            Assert.AreEqual(runtime.Id,ufRuntime.Id);
            string policyNumber = collection.Get<string>("PolicyNumber");
            Assert.IsTrue(string.IsNullOrEmpty(policyNumber));
        }

        [Test]
        public void TestExecuteWorkflowFormSuspendedState()
        {
            var processDefinition = BuildProcessWithTasks();
            ProcessDefinitionPersisnenceService service=new ProcessDefinitionPersisnenceService();
            service.Create(processDefinition,ProcessDefStatusEnum.Active, 1);

            ProcessRuntimePersistenceService pservice=new ProcessRuntimePersistenceService();
            PropertySetCollection collection=new PropertySetCollection(new PropertySchemaSet(new PropertySchemaFactory()));
            Mock<IProcessRuntimeEnvironment> mEnv=new Mock<IProcessRuntimeEnvironment>();
            mEnv.SetupGet(m => m.PropertySet).Returns(collection).Verifiable();
            mEnv.Setup(m => m.TaskServiceAsync()).Returns(() =>
                    Task.FromResult(new ExecutionResult(StepExecutionStatusEnum.Suspend))).Verifiable();

            IProcessRuntime runtime = pservice.Create(processDefinition, collection);
            string[] error;
            Assert.IsTrue(runtime.TryCompile(out error));
            var startStep = runtime.StartSteps.First();
            Assert.IsNotNull(startStep);
            var result = runtime.Execute(startStep, mEnv.Object);
            Assert.IsNotNull(result);
            Assert.AreEqual(StepExecutionStatusEnum.Ready, result.Item1.Status);
            Assert.AreEqual("task_1",result.Item2.StepId);
            // freeze the workflow
            pservice.Freeze(runtime,collection);
            // execute Task step
            result = runtime.Execute(result.Item2, mEnv.Object);
            Assert.IsNotNull(result);
            Assert.AreEqual(StepExecutionStatusEnum.Suspend, result.Item1.Status);
            Assert.AreEqual("task_1", result.Item2.StepId);
            mEnv.Verify(m=>m.TaskServiceAsync(),Times.Once);
            Assert.IsNotNull(runtime.SuspendedInStep);
            Assert.AreEqual("task_1",runtime.SuspendedInStep.StepId);
            // freeze the process
            pservice.Freeze(runtime, collection);
            // unfreeze the process

            IProcessRuntime unfrozenProcess;
            StepRuntime nextStep;
            IPropertySetCollection unfrozenCollection;
            Assert.IsTrue(pservice.TryUnfreeze(runtime.Id, out unfrozenProcess, out nextStep, out unfrozenCollection));
            Assert.IsNotNull(unfrozenProcess);
            Assert.IsNotNull(unfrozenCollection);
            Assert.AreEqual(1,unfrozenCollection.Get<int?>("Count"));
            Assert.AreEqual("P12345",unfrozenCollection.Get<string>("PolicyNumber"));
            Assert.AreEqual(ProcessStateEnum.Suspended,unfrozenProcess.State);
            Assert.IsNotNull(unfrozenProcess.SuspendedInStep);
            Assert.AreEqual("task_1",unfrozenProcess.SuspendedInStep.StepId);
        }

        [Test]
        public void TestExecuteMultipleStepsSaveStateBetweenSteps()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "com.klaudwerk.workflow.renewal",
                name: "Renewal", description: "Policy Renewal");
            IReadOnlyList<ProcessValidationResult> result;
            builder.Variables().Name("Count").Type(VariableTypeEnum.Int).Done()
            .Start("s_1").SetName("Start")
                .OnEntry()
                    .Language(ScriptLanguage.CSharpScript)
                    .Body("" +
                          " PropertySet.Set(\"Count\",(int?)1);" +
                          " return 1;").Done().Done()
            .Step("s_2").OnEntry()
                    .Language(ScriptLanguage.CSharpScript)
                    .Body("" +
                          " PropertySet.Set(\"Count\",(int?)2);" +
                          " return 1;").Done().Done()
            .Step("s_3").OnEntry()
                    .Language(ScriptLanguage.CSharpScript)
                    .Body("" +
                          " PropertySet.Set(\"Count\",(int?)3);" +
                          " return 1;").Done().Done()
            .End("e_1").SetName("End Process").Done()
            .Link().From("s_1").To("s_2").Name("s_1_s_2").Done()
            .Link().From("s_2").To("s_3").Name("s_2_s_3").Done()
            .Link().From("s_3").To("e_1").Name("end").Done()
            .TryValidate(out result);
            ProcessDefinition processDefinition = builder.Build();
            ProcessDefinitionPersisnenceService service = new ProcessDefinitionPersisnenceService();
            service.Create(processDefinition, ProcessDefStatusEnum.Active, 1);

            ProcessRuntimePersistenceService pservice = new ProcessRuntimePersistenceService();
            PropertySetCollection collection = new PropertySetCollection(new PropertySchemaSet(new PropertySchemaFactory()));
            Mock<IProcessRuntimeEnvironment> mEnv = new Mock<IProcessRuntimeEnvironment>();
            mEnv.SetupGet(m => m.PropertySet).Returns(collection).Verifiable();
            mEnv.Setup(m => m.TaskServiceAsync()).Returns(() =>
                    Task.FromResult(new ExecutionResult(StepExecutionStatusEnum.Suspend))).Verifiable();
            IProcessRuntime runtime = pservice.Create(processDefinition, collection);
            string[] errors;
            runtime.TryCompile(out errors);
            IProcessRuntime ufRuntime;
            StepRuntime ufStep;
            IPropertySetCollection ufCollection;
            pservice.TryUnfreeze(runtime.Id, out ufRuntime, out ufStep, out ufCollection);
            Assert.IsNotNull(ufRuntime);
            Assert.IsNotNull(ufCollection);
            Assert.IsNull(ufStep);
            Assert.IsNull(ufCollection.Get<int?>("Count"));
            Assert.AreEqual(ProcessStateEnum.NotStarted,ufRuntime.State);

            Tuple<ExecutionResult, StepRuntime> exResult = runtime.Execute(runtime.StartSteps[0], mEnv.Object);
            Assert.IsNotNull(exResult);
            pservice.TryUnfreeze(runtime.Id, out ufRuntime, out ufStep, out ufCollection);
            Assert.AreEqual(1,ufCollection.Get<int?>("Count"));
            Assert.AreEqual(ProcessStateEnum.Ready, ufRuntime.State);
            Assert.IsNotNull(ufStep);
            Assert.AreEqual("s_2",ufStep.StepId);

            exResult = runtime.Execute(exResult.Item2, mEnv.Object);
            Assert.IsNotNull(exResult);
            pservice.TryUnfreeze(runtime.Id, out ufRuntime, out ufStep, out ufCollection);
            Assert.AreEqual(2, ufCollection.Get<int?>("Count"));
            Assert.AreEqual(ProcessStateEnum.Ready, ufRuntime.State);
            Assert.IsNotNull(ufStep);
            Assert.AreEqual("s_3", ufStep.StepId);

            exResult = runtime.Execute(exResult.Item2, mEnv.Object);
            Assert.IsNotNull(exResult);
            pservice.TryUnfreeze(runtime.Id, out ufRuntime, out ufStep, out ufCollection);
            Assert.AreEqual(3, ufCollection.Get<int?>("Count"));
            Assert.AreEqual(ProcessStateEnum.Ready, ufRuntime.State);
            Assert.IsNotNull(ufStep);
            Assert.AreEqual("e_1", ufStep.StepId);

            exResult = runtime.Execute(exResult.Item2, mEnv.Object);
            Assert.IsNotNull(exResult);
            pservice.TryUnfreeze(runtime.Id, out ufRuntime, out ufStep, out ufCollection);
            Assert.AreEqual(3, ufCollection.Get<int?>("Count"));
            Assert.AreEqual(ProcessStateEnum.Completed, ufRuntime.State);
            Assert.IsNull(ufStep);
        }
        private ProcessDefinition BuildProcessWithTasks()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "com.klaudwerk.workflow.renewal",
                name: "Renewal", description: "Policy Renewal");
            IReadOnlyList<ProcessValidationResult> result;
            bool isValid=
                builder.Variables().Name("PolicyNumber").Type(VariableTypeEnum.String).Done()
                .Variables().Name("Count").Type(VariableTypeEnum.Int).Done()
                .Start("s_1").SetName("Start")
                    .Vars().Name("PolicyNumber").OnExit().Done()
                    .Vars().Name("Count").OnExit().Done()
                    .OnEntry()
                        .Language(ScriptLanguage.CSharpScript)
                        .Body("" +
                              " PropertySet.Set(\"PolicyNumber\",\"P12345\");"+
                              " PropertySet.Set(\"Count\",(int?)1);"+
                              " return 1;").Done().Done()
                .Step("task_1").Handler().HumanTask().Done().Done()
                .End("e_1").SetName("End Process").Done()
                .Link().From("s_1").To("task_1").Name("task").Done()
                .Link().From("task_1").To("e_1").Name("end").Done()
                .TryValidate(out result);
            Assert.IsTrue(isValid);
            return builder.Build();

        }

        private ProcessDefinition BuildProcessdefinition()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "com.klaudwerk.workflow.renewal",
                name: "Renewal", description: "Policy Renewal");
            IReadOnlyList<ProcessValidationResult> result;
            bool isValid=builder.Variables().Name("PolicyNumber").Type(VariableTypeEnum.String).Done()
                .Start("s_1").SetName("Start").Vars().Name("PolicyNumber").OnExit().Done().Done()
                .End("e_1").SetName("End Process").Done()
                .Link().From("s_1").To("e_1").Name("finish").Done()
                .TryValidate(out result);
            Assert.IsTrue(isValid);
            return builder.Build();
        }
    }
}