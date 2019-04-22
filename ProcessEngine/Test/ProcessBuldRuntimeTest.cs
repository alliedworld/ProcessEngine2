/**
The MIT License (MIT)

Copyright (c) 2016 Igor Polouektov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
  */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Klaudwerk.PropertySet;
using Klaudwerk.PropertySet.Test;
using KlaudWerk.ProcessEngine.Builder;
using KlaudWerk.ProcessEngine.Definition;
using KlaudWerk.ProcessEngine.Runtime;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace KlaudWerk.ProcessEngine.Test
{
    [TestFixture]
    public class ProcessBuldRuntimeTest
    {
        [Test]
        public void TestBuildFullyLinkedProcess()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "com.klaudwerk.workflow.renewal",
                name: "Renewal", description: "Policy Renewal");
            builder.Start("s_1").SetName("Start").Action().Name("Provide Administrators Form").Skippable(false).Done()
                .Action().Name("Provide Requirements For Legal Draft Form").Skippable(false).Done()
                .Done()
                .Step("r_1").SetName("Legal Draft Review").Action().Name("Review Legal Draft").Skippable(false).Done()
                .Done()
                .Step("r_uw").SetName("Underwriter Review").Done()
                .Step("r_CUO").SetName("CUO Review").Done()
                .Step("p_sent").SetName("Send to partner").Done()
                .Step("p_received").SetName("Received from Partner").Done()
                .End("e_1").Done()
                .Link().From("s_1").To("r_1").Done()
                .Link().From("r_1").To("r_uw").Done()
                .Link().From("r_uw").To("r_CUO").Name("Approved").Done()
                .Link().From("r_uw").To("r_1").Name("Rejected").Done()
                .Link().From("r_CUO").To("p_sent").Name("Approved").Description("Approved. Send to Partner").Done()
                .Link().From("r_CUO").To("r_uw").Name("Rejected").Done()
                .Link().From("p_sent").To("p_received").Done()
                .Link().From("p_received").To("r_CUO").Name("Rejected").Description("Rejected, need additional review.")
                .Done()
                .Link().From("p_received").To("e_1").Name("Agreement").Done();
            IReadOnlyList<ProcessValidationResult> result;
            bool isValid = builder.TryValidate(out result);
            Assert.IsTrue(isValid);
            Assert.IsNotNull(result);
            Assert.AreEqual(0,result.Count);
            ProcessDefinition pd = builder.Build();
            Assert.IsNotNull(pd);
            Assert.IsNotNull(pd.Steps);
            Assert.AreEqual(7,pd.Steps.Length);
            Assert.IsNotNull(pd.Links);
            Assert.AreEqual(9,pd.Links.Length);
            // get MD5
            Md5CalcVisitor visitor=new Md5CalcVisitor();
            pd.Accept(visitor);
            string md5 = visitor.CalculateMd5();
            Assert.IsFalse(string.IsNullOrEmpty(md5));
        }

        [Test]
        public void TestCreateRuntimeForSimpleFlow()
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
            var processDefinition = builder.Build();
            Assert.IsNotNull(processDefinition);
        }

        [Test]
        public void RuntimeFlowsWithTheSameDefinitionMd5ShouldBeEqual()
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
            var processDefinition = builder.Build();
            Md5CalcVisitor visitor=new Md5CalcVisitor();
            Assert.IsNotNull(processDefinition);
            processDefinition.Accept(visitor);
            string md5 = visitor.CalculateMd5();
            var processDefinition1 = builder.Build();
            visitor.Reset();
            processDefinition1.Accept(visitor);
            string md5One = visitor.CalculateMd5();
            Assert.AreEqual(md5,md5One);
        }

        [Test]
        public void ChnagingFlowShouldCalculateDifferentMd5()
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
            var processDefinition = builder.Build();
            Md5CalcVisitor visitor=new Md5CalcVisitor();
            Assert.IsNotNull(processDefinition);
            processDefinition.Accept(visitor);
            string md5 = visitor.CalculateMd5();
            builder.Step("s_2").SetName("My Step").Done()
                .Link().From("s_1").To("e_1").Remove()
                .Link().From("s_1").To("s_2").Done()
                .Link().From("s_2").To("e_1").Done();
            Assert.IsTrue(builder.TryValidate(out result));
            visitor.Reset();
            processDefinition = builder.Build();
            processDefinition.Accept(visitor);
            string md5One = visitor.CalculateMd5();
            Assert.AreNotEqual(md5,md5One);

        }

        [Test]
        public void TestGroupOrRoleVariableDefinitionsShouldPreserveAccounstAfterBuild()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "com.klaudwerk.workflow.renewal",
                name: "Renewal", description: "Policy Renewal");
            IReadOnlyList<ProcessValidationResult> result;
            bool isValid=builder.Variables().Name("roles")
                    .Type(VariableTypeEnum.RolesList)
                    .Constraints().PossibeValues(new object[]{"admin","worker"}).Done()
                    .Done()
                .Variables().Name("groups")
                    .Type(VariableTypeEnum.GroupsList)
                    .Constraints().PossibeValues(new object[]{"g1"}).Done()
                    .Done()
                .Start("s_1").SetName("Start").Done()
                .End("e_1").SetName("End Process").Done()
                .Link().From("s_1").To("e_1").Name("finish").Done()
                .TryValidate(out result);
            Assert.IsTrue(isValid);
            var processDefinition = builder.Build();
            Assert.IsNotNull(processDefinition);
            Assert.IsNotNull(processDefinition.Variables);
            Assert.AreEqual(2,processDefinition.Variables.Length);
            Assert.IsNotNull(processDefinition.Variables[0].Constraints);
            Assert.IsNotNull(processDefinition.Variables[1].Constraints);
            Assert.AreEqual(new string[]{"admin","worker"},processDefinition.Variables[0].Constraints.PossibleValues.
                Select(JsonConvert.DeserializeObject<string>).ToArray());
            Assert.AreEqual(new string[]{"g1"},processDefinition.Variables[1].Constraints.PossibleValues
                .Select(JsonConvert.DeserializeObject<string>).ToArray());
        }
        [Test]
        public void TestExecuteScriptInEnvironment()
        {
            ProcessRuntimeEnvironment env = new ProcessRuntimeEnvironment(null,null) {ProcessEnvId = 1};
            ScriptDefinition sd=new ScriptDefinition(
                @"
                    ProcessEnvId+1
                ",
                ScriptLanguage.CSharpScript, new string[]{},new string[]{});
            CsScriptRuntime scriptRuntime=new CsScriptRuntime(sd);
            string[] errors;
            Assert.IsTrue(scriptRuntime.TryCompile(out errors));
            var execute = scriptRuntime.Execute(env).Result;
            Assert.AreEqual(2,execute);
        }

        [Test]
        public void TestScriptExecutionShouldThrowException()
        {
            var env=new ProcessRuntimeEnvironment(null, null);
            env.ProcessEnvId = 1;
            ScriptDefinition sd=new ScriptDefinition(
                @"
                    throw new ArgumentException();
                ",
                ScriptLanguage.CSharpScript, new string[]{"System"},new string[]{});
            CsScriptRuntime scriptRuntime=new CsScriptRuntime(sd);
            string[] errors;
            Assert.IsTrue(scriptRuntime.TryCompile(out errors));
            Assert.Throws<AggregateException>(() => {
                int r= scriptRuntime.Execute(env).Result;
            });
        }
        [Test]
        public void TestStepDefinitionCompilation()
        {
            ProcessRuntimeEnvironment env=new ProcessRuntimeEnvironment(null, null);
            env.ProcessEnvId = 1;
            ScriptDefinition onEntry=new ScriptDefinition(
                @"
                    ProcessEnvId+1
                ",
                ScriptLanguage.CSharpScript, new string[]{},new string[]{});
            ScriptDefinition onEExit=new ScriptDefinition(
                @"
                    ProcessEnvId-1
                ",
                ScriptLanguage.CSharpScript, new string[]{},new string[]{});
            StepDefinition sd = new StepDefinition(Guid.NewGuid(), "step_id", string.Empty, String.Empty, true, false,
                null, null, onEntry, onEExit, null, null,
                new StepHandlerDefinition(StepHandlerTypeEnum.None, null,string.Empty,string.Empty));
            StepRuntime runtime=new StepRuntime(sd, new LinkRuntime[] { });
            string[] errors;
            Assert.IsTrue(runtime.TryCompile(out errors));
        }

        [Test]
        public void ScriptExecutionCanFailOnEnterValidationScript()
        {
            ProcessRuntimeEnvironment env=new ProcessRuntimeEnvironment(null, null);
            env.ProcessEnvId = 1;
            ScriptDefinition onEntry=new ScriptDefinition(
                @"
                    ProcessEnvId=2;
                    return 0;
                ",
                ScriptLanguage.CSharpScript, new string[]{},new string[]{});
            ScriptDefinition onExit=new ScriptDefinition(
                @"
                    ProcessEnvId-1
                ",
                ScriptLanguage.CSharpScript, new string[]{},new string[]{});
            StepDefinition sd = new StepDefinition(Guid.NewGuid(), "step_id", string.Empty, String.Empty, true, false,
                null, null, onEntry, onExit, null, null,new StepHandlerDefinition(StepHandlerTypeEnum.None, null,
                    string.Empty,string.Empty));
            StepRuntime runtime=new StepRuntime(sd,new LinkRuntime[] {});
            string[] errors;
            Assert.IsTrue(runtime.TryCompile(out errors));
            Assert.IsFalse(runtime.ValidateOnEnter(env).Result.Valid);
            Assert.AreEqual(2,env.ProcessEnvId);
        }

        [Test]
        public void ScriptExecutionCanFailOnExitalidationScript()
        {
            ProcessRuntimeEnvironment env=new ProcessRuntimeEnvironment(null, null);
            env.ProcessEnvId = 1;
            ScriptDefinition onEntry=new ScriptDefinition(
                @"
                    ProcessEnvId=2;
                    return 1;
                ",
                ScriptLanguage.CSharpScript, new string[]{},new string[]{});
            ScriptDefinition onExit=new ScriptDefinition(
                @"
                    ProcessEnvId=3;
                    return 0;
                ",
                ScriptLanguage.CSharpScript, new string[]{},new string[]{});
            StepDefinition sd = new StepDefinition(Guid.NewGuid(), "step_id", string.Empty, String.Empty, true, false,
                null, null, onEntry, onExit, null, null,
                new StepHandlerDefinition(StepHandlerTypeEnum.None, null,string.Empty,string.Empty));
            StepRuntime runtime=new StepRuntime(sd, new LinkRuntime[] { });
            string[] errors;
            Assert.IsTrue(runtime.TryCompile(out errors));
            Assert.IsTrue(runtime.ValidateOnEnter(env).Result.Valid);
            Assert.AreEqual(2,env.ProcessEnvId);
            Assert.IsFalse(runtime.ValidateOnExit(env).Result.Valid);
            Assert.AreEqual(3,env.ProcessEnvId);
        }

        [Test]
        public void OnEntryValidationWithSatisfiedVariablesShouldPass()
        {
            IPropertySchemaSet propertySet=new ValueSetCollectionTest.MockPropertySchemaSet(new PropertySchemaFactory());
            IPropertySetCollection collection=new ValueSetCollectionTest.MockPropertySetCollection(propertySet);
            ProcessRuntimeEnvironment env = new ProcessRuntimeEnvironment(null, collection);
            propertySet.SetSchema("policy", new StringSchema());
            propertySet.SetSchema("account", new StringSchema());
            propertySet.SetSchema("id", new IntSchema());
            collection.Set("policy", "P0001");
            collection.Set("account", "AOOO1");
            collection.Set("id", (int?)100);
            env.ProcessEnvId = 1;
            VariableMapDefinition[] vars = {
                new VariableMapDefinition("policy",VarRequiredEnum.OnEntry),
                new VariableMapDefinition("account",VarRequiredEnum.OnEntry),
                new VariableMapDefinition("id",VarRequiredEnum.OnEntry),
            };
            StepDefinition sd = new StepDefinition(Guid.NewGuid(), "step_id", string.Empty, String.Empty, true, false,
                null, null, null, null, null, vars,
                new StepHandlerDefinition(StepHandlerTypeEnum.None, null,string.Empty,string.Empty));
            StepRuntime runtime = new StepRuntime(sd, new LinkRuntime[] { });
            string[] errors;
            Assert.IsTrue(runtime.TryCompile(out errors));
            Assert.IsTrue(runtime.ValidateOnEnter(env).Result.Valid);
        }

        [Test]
        public void MissingOnEntryVariableShouldFailValidation()
        {
            IPropertySchemaSet propertySet = new ValueSetCollectionTest.MockPropertySchemaSet(new PropertySchemaFactory());
            IPropertySetCollection collection = new ValueSetCollectionTest.MockPropertySetCollection(propertySet);
            ProcessRuntimeEnvironment env = new ProcessRuntimeEnvironment(null, collection);
            propertySet.SetSchema("policy", new StringSchema());
            propertySet.SetSchema("account", new StringSchema());
            propertySet.SetSchema("id", new IntSchema());
            collection.Set("policy", "P0001");
            collection.Set("id", (int?)100);
            env.ProcessEnvId = 1;
            VariableMapDefinition[] vars = {
                new VariableMapDefinition("policy",VarRequiredEnum.OnEntry),
                new VariableMapDefinition("account",VarRequiredEnum.OnEntry),
                new VariableMapDefinition("id",VarRequiredEnum.OnEntry),
            };
            StepDefinition sd = new StepDefinition(Guid.NewGuid(), "step_id", string.Empty, String.Empty, true, false,
                null, null, null, null, null, vars,
                new StepHandlerDefinition(StepHandlerTypeEnum.None, null,string.Empty,string.Empty));
            StepRuntime runtime = new StepRuntime(sd, new LinkRuntime[] { });
            string[] errors;
            Assert.IsTrue(runtime.TryCompile(out errors));
            var res=runtime.ValidateOnEnter(env).Result;
            Assert.IsFalse(res.Valid);
            Assert.IsNotNull(res.Messages);
            Assert.AreEqual(1, res.Messages.Length);

        }

        [Test]
        public void MissingOnExitVariablesShouldFailValidation()
        {
            IPropertySchemaSet propertySet = new ValueSetCollectionTest.MockPropertySchemaSet(new PropertySchemaFactory());
            IPropertySetCollection collection = new ValueSetCollectionTest.MockPropertySetCollection(propertySet);
            ProcessRuntimeEnvironment env = new ProcessRuntimeEnvironment(null, collection);
            propertySet.SetSchema("policy", new StringSchema());
            propertySet.SetSchema("account", new StringSchema());
            propertySet.SetSchema("id", new IntSchema());
            collection.Set("policy", "P0001");
            collection.Set("id", (int?)100);
            env.ProcessEnvId = 1;
            VariableMapDefinition[] vars = {
                new VariableMapDefinition("policy",VarRequiredEnum.OnEntry),
                new VariableMapDefinition("account",VarRequiredEnum.OnExit),
                new VariableMapDefinition("id",VarRequiredEnum.OnEntry),
            };
            StepDefinition sd = new StepDefinition(Guid.NewGuid(), "step_id", string.Empty, String.Empty, true, false,
                null, null, null, null, null, vars,
                new StepHandlerDefinition(StepHandlerTypeEnum.None, null,string.Empty,string.Empty));
            StepRuntime runtime = new StepRuntime(sd, new LinkRuntime[] { });
            string[] errors;
            Assert.IsTrue(runtime.TryCompile(out errors));
            var res=runtime.ValidateOnEnter(env).Result;
            Assert.IsTrue(res.Valid);
            res=runtime.ValidateOnExit(env).Result;
            Assert.IsFalse(res.Valid);
            Assert.IsNotNull(res.Messages);
            Assert.AreEqual(1, res.Messages.Length);
        }

        [Test]
        public void LinkWithScriptCanBeCompiled()
        {
            ProcessRuntimeEnvironment env=new ProcessRuntimeEnvironment(null, null);
            env.ProcessEnvId = 1;
            ScriptDefinition script=new ScriptDefinition(
                @"
                    ProcessEnvId=2;
                    return 1;
                ",
                ScriptLanguage.CSharpScript, new string[]{},new string[]{});
            LinkDefinition ld=new LinkDefinition(new StepDefinitionId(Guid.NewGuid(),"s1"),
                new StepDefinitionId(Guid.NewGuid(),"s2"),"l1","l1",script);
            LinkRuntime lr=new LinkRuntime(ld);
            string[] errors;
            Assert.IsTrue(lr.TryCompile(out errors));
            Assert.IsTrue(lr.IsCompiled);
            Assert.IsNotNull(errors);
            Assert.AreEqual(0,errors.Length);
        }

        [Test]
        public void LinkWithInvalidScriptShouldReturnErrors()
        {
            ProcessRuntimeEnvironment env=new ProcessRuntimeEnvironment(null, null);
            env.ProcessEnvId = 1;
            ScriptDefinition script=new ScriptDefinition(
                @"
                    ProcessEnvId=2;
                    return
                ",
                ScriptLanguage.CSharpScript, new string[]{},new string[]{});
            LinkDefinition ld=new LinkDefinition(new StepDefinitionId(Guid.NewGuid(),"s1"),
                new StepDefinitionId(Guid.NewGuid(),"s2"),"l1","l1",script);
            LinkRuntime lr=new LinkRuntime(ld);
            string[] errors;
            Assert.IsFalse(lr.TryCompile(out errors));
            Assert.IsTrue(lr.IsCompiled);
            Assert.IsNotNull(errors);
            Assert.Greater(errors.Length,0);
        }

        [Test]
        public void LinkWithScriptCanBeEvaluated()
        {
            ProcessRuntimeEnvironment env=new ProcessRuntimeEnvironment(null, null);
            env.ProcessEnvId = 1;
            ScriptDefinition script=new ScriptDefinition(
                @"
                    return ProcessEnvId==2?1:0;
                ",
                ScriptLanguage.CSharpScript, new string[]{},new string[]{});
            LinkDefinition ld=new LinkDefinition(new StepDefinitionId(Guid.NewGuid(),"s1"),
                new StepDefinitionId(Guid.NewGuid(),"s2"),"l1","l1",script);
            LinkRuntime lr=new LinkRuntime(ld);
            string[] errors;
            Assert.IsTrue(lr.TryCompile(out errors));
            bool rs = lr.Evaluate(env).Result;
            Assert.IsFalse(rs);
            env.ProcessEnvId = 2;
            rs = lr.Evaluate(env).Result;
            Assert.IsTrue(rs);
        }

        #region Step Handler Tests

        [Test]
        public void EmptyHandlerShouldNotCallMethods()
        {
            IPropertySchemaSet propertySet = new ValueSetCollectionTest.MockPropertySchemaSet(new PropertySchemaFactory());
            IPropertySetCollection collection = new ValueSetCollectionTest.MockPropertySetCollection(propertySet);
            ProcessRuntimeEnvironment env = new ProcessRuntimeEnvironment(null, collection);
            env.ProcessEnvId = 1;
            StepDefinition sd = new StepDefinition(Guid.NewGuid(), "step_id", string.Empty, String.Empty, true, false,
                null, null, null, null, null, null,
                new StepHandlerDefinition(StepHandlerTypeEnum.None, null,string.Empty,string.Empty));
            StepRuntime runtime = new StepRuntime(sd, new LinkRuntime[] { });
            string[] errors;
            Assert.IsTrue(runtime.TryCompile(out errors));
            var result = runtime.ExecuteAsync(env).Result;
            Assert.IsNotNull(result);
            Assert.AreEqual(StepExecutionStatusEnum.Ready, result.Status);
        }
        [Test]
        public void IocHandlerShouldCallMetodOnEnvironment()
        {
            Mock<IProcessRuntimeEnvironment> mEnv=new Mock<IProcessRuntimeEnvironment>();
            mEnv.Setup(e=>e.IocServiceAsync(It.Is<string>(v=>v=="IocMethod")))
                .Returns(Task.FromResult(new ExecutionResult(StepExecutionStatusEnum.Ready)))
                .Verifiable("IocService was not called");
            StepDefinition sd = new StepDefinition(Guid.NewGuid(), "step_id", string.Empty, String.Empty, true, false,
                null, null, null, null, null, null,
                new StepHandlerDefinition(StepHandlerTypeEnum.IoC, null,"IocMethod",string.Empty));
            StepRuntime runtime = new StepRuntime(sd, new LinkRuntime[] { });
            string[] errors;
            Assert.IsTrue(runtime.TryCompile(out errors));
            var result = runtime.ExecuteAsync(mEnv.Object).Result;
            Assert.IsNotNull(result);
            Assert.AreEqual(StepExecutionStatusEnum.Ready, result.Status);
            mEnv.Verify(m=>m.IocServiceAsync(It.Is<string>(v=>v=="IocMethod")),Times.Once);
        }

        [Test]
        public void TaskHandlerShoulCallTaskService()
        {
            Mock<IProcessRuntimeEnvironment> mEnv=new Mock<IProcessRuntimeEnvironment>();
            mEnv.Setup(e=>e.TaskServiceAsync())
                .Returns(Task.FromResult(new ExecutionResult(StepExecutionStatusEnum.Ready)))
                .Verifiable("Task service was not called");
            StepDefinition sd = new StepDefinition(Guid.NewGuid(), "step_id", string.Empty, String.Empty, true, false,
                null, null, null, null, null, null,
                new StepHandlerDefinition(StepHandlerTypeEnum.Task, null,string.Empty,string.Empty));
            StepRuntime runtime = new StepRuntime(sd, new LinkRuntime[] { });
            string[] errors;
            Assert.IsTrue(runtime.TryCompile(out errors));
            var result = runtime.ExecuteAsync(mEnv.Object).Result;
            Assert.IsNotNull(result);
            Assert.AreEqual(StepExecutionStatusEnum.Ready, result.Status);
            mEnv.Verify(m=>m.TaskServiceAsync(),Times.Once);
        }

        [Test]
        public void AsmLoadHandlerShoulCallAsmLoadService()
        {
            Mock<IProcessRuntimeEnvironment> mEnv=new Mock<IProcessRuntimeEnvironment>();
            mEnv.Setup(e=>e.LoadExecuteAssemplyAsync(It.Is<string>(a=>a=="AsmToExecute")))
                .Returns(Task.FromResult(new ExecutionResult(StepExecutionStatusEnum.Ready)))
                .Verifiable("Assemly Execution service was not called");
            StepDefinition sd = new StepDefinition(Guid.NewGuid(), "step_id", string.Empty, String.Empty, true, false,
                null, null, null, null, null, null,
                new StepHandlerDefinition(StepHandlerTypeEnum.Service, null,string.Empty,"AsmToExecute"));
            StepRuntime runtime = new StepRuntime(sd, new LinkRuntime[] { });
            string[] errors;
            Assert.IsTrue(runtime.TryCompile(out errors));
            var result = runtime.ExecuteAsync(mEnv.Object).Result;
            Assert.IsNotNull(result);
            Assert.AreEqual(StepExecutionStatusEnum.Ready, result.Status);
            mEnv.Verify(m=>m.LoadExecuteAssemplyAsync(It.Is<string>(a=>a=="AsmToExecute")),Times.Once);

        }

        [Test]
        public void ScriptHandlerShouldExecuteScript()
        {
            IPropertySchemaSet propertySet = new ValueSetCollectionTest.MockPropertySchemaSet(new PropertySchemaFactory());
            IPropertySetCollection collection = new ValueSetCollectionTest.MockPropertySetCollection(propertySet);
            ProcessRuntimeEnvironment env = new ProcessRuntimeEnvironment(null, collection);
            env.ProcessEnvId = 1;
            ScriptDefinition script=new ScriptDefinition(
                @"
                    ProcessEnvId=2;
                    return 1;
                ",
                ScriptLanguage.CSharpScript, new string[]{},new string[]{});
            StepDefinition sd = new StepDefinition(Guid.NewGuid(), "step_id", string.Empty, String.Empty, true, false,
                null, null, null, null, null, null,
                new StepHandlerDefinition(StepHandlerTypeEnum.Script, script,string.Empty,string.Empty));
            StepRuntime runtime = new StepRuntime(sd, new LinkRuntime[] { });
            string[] errors;
            Assert.IsTrue(runtime.TryCompile(out errors));
            var result = runtime.ExecuteAsync(env).Result;
            Assert.IsNotNull(result);
            Assert.AreEqual(StepExecutionStatusEnum.Ready, result.Status);
            Assert.AreEqual(2,env.ProcessEnvId);
        }

        [Test]
        public void InvalidHandlerScriptShouldReturnCompilationError()
        {
            IPropertySchemaSet propertySet = new ValueSetCollectionTest.MockPropertySchemaSet(new PropertySchemaFactory());
            IPropertySetCollection collection = new ValueSetCollectionTest.MockPropertySetCollection(propertySet);
            ProcessRuntimeEnvironment env = new ProcessRuntimeEnvironment(null, collection);
            env.ProcessEnvId = 1;
            ScriptDefinition script=new ScriptDefinition(
                @"
                    ProcessEnvId=
                    return 1
                ",
                ScriptLanguage.CSharpScript, new string[]{},new string[]{});
            StepDefinition sd = new StepDefinition(Guid.NewGuid(), "step_id", string.Empty, String.Empty, true, false,
                null, null, null, null, null, null,
                new StepHandlerDefinition(StepHandlerTypeEnum.Script, script,string.Empty,string.Empty));
            StepRuntime runtime = new StepRuntime(sd, new LinkRuntime[] { });
            string[] errors;
            Assert.IsFalse(runtime.TryCompile(out errors));
            Assert.IsNotNull(errors);
            Assert.Greater(errors.Length,0);

        }

        [Test]
        public void ErrorInHandlerExecutionShoulReturnFailedResult()
        {
            IPropertySchemaSet propertySet = new ValueSetCollectionTest.MockPropertySchemaSet(new PropertySchemaFactory());
            IPropertySetCollection collection = new ValueSetCollectionTest.MockPropertySetCollection(propertySet);
            ProcessRuntimeEnvironment env = new ProcessRuntimeEnvironment(null, collection);
            env.ProcessEnvId = 1;
            ScriptDefinition script=new ScriptDefinition(
                @"
                   throw new ArgumentNullException();
                ",
                ScriptLanguage.CSharpScript, new string[]{"System"},new string[]{});
            StepDefinition sd = new StepDefinition(Guid.NewGuid(), "step_id", string.Empty, String.Empty, true, false,
                null, null, null, null, null, null,
                new StepHandlerDefinition(StepHandlerTypeEnum.Script, script,string.Empty,string.Empty));
            StepRuntime runtime = new StepRuntime(sd, new LinkRuntime[] { });
            string[] errors;
            Assert.True(runtime.TryCompile(out errors));
            var result = runtime.ExecuteAsync(env).Result;
            Assert.IsNotNull(result);
            Assert.AreEqual(StepExecutionStatusEnum.Failed,result.Status);
        }
        #endregion

        #region Process Runtime Tests

        [Test]
        public void TestVariableDefinitionsCanBeCompiled()
        {
            StepHandlerDefinition shd = new StepHandlerDefinition
            {
                Script = new ScriptDefinition("return 1;",
                    ScriptLanguage.CSharpScript, new string[] { }, new string[] { })
            };
            VariableDefinition varStr=new VariableDefinition("v_str",string.Empty,VariableTypeEnum.String, shd,null);
            VariableRuntime rt=new VariableRuntime(varStr);
            string[] errors;
            Assert.IsTrue(rt.TryCompile(out errors));
            Assert.IsTrue(rt.IsCompiled);

        }

        [Test]
        public void TestVariableDefinitionsSetPossibleValuesThroughScript()
        {
            StepHandlerDefinition shd = new StepHandlerDefinition
            {
                Script = new ScriptDefinition("string[] values=new []{\"One\",\"Two\",\"Three\"};\n"+
                                              "SetPossibleValues(\"v_str\",values);\n"+
                                              "return 1;\n"
                                              ,
                    ScriptLanguage.CSharpScript, new string[] { }, new string[] { })
            };
            IPropertySchemaSet propertySet = new ValueSetCollectionTest.MockPropertySchemaSet(new PropertySchemaFactory());
            IPropertySetCollection collection = new ValueSetCollectionTest.MockPropertySetCollection(propertySet);
            VariableDefinition varStr=new VariableDefinition("v_str",string.Empty,VariableTypeEnum.String, shd,null);
            VariableRuntime rt=new VariableRuntime(varStr);
            varStr.SetupVariable(collection);
            ProcessRuntimeEnvironment env=new ProcessRuntimeEnvironment(null, collection);
            Assert.AreEqual(1, collection.Count);
            string[] errors;
            Assert.IsTrue(rt.TryCompile(out errors));
            Assert.IsTrue(rt.IsCompiled);
            Assert.IsTrue(rt.Evaluate(env).Result);
            var valueSchema = collection.Schemas.GetSchema("v_str");
            Assert.IsNotNull(valueSchema);
            Assert.IsNotNull(valueSchema.PossibleValues);
            Assert.AreEqual(3, valueSchema.PossibleValues.Count());
            Assert.AreEqual("One",valueSchema.PossibleValues.ElementAt(0).ToString());
        }
        [Test]
        public void TestSetupDateTimeVariable()
        {
            IPropertySchemaSet propertySet = new ValueSetCollectionTest.MockPropertySchemaSet(new PropertySchemaFactory());
            IPropertySetCollection collection = new ValueSetCollectionTest.MockPropertySetCollection(propertySet);
            VariableDefinition varDateTime=new VariableDefinition("v_dateTime",string.Empty,VariableTypeEnum.DateTime, null,null);
            VariableRuntime rt=new VariableRuntime(varDateTime);
            varDateTime.SetupVariable(collection);
        }
        [Test]
        public void TestVariableSetDisplayHint()
        {
            VariableBuilder builder=new VariableBuilder(new ProcessBuilder("id"));
            builder.Name("var").Constraints().MaxValue(100).MinValue(10).Hint(DisplayHintEnum.Currency).Done().Done();
            ConstraintDefinition cd=new ConstraintDefinition(builder.VariableConstraints);
            Assert.AreEqual(DisplayHintEnum.Currency, cd.DisplayHint);

        }
        [Test]
        public void TestVariableDefinitionUserListSetCollectionValueToString()
        {
            VariableDefinition varUsers=new VariableDefinition("users",string.Empty,VariableTypeEnum.UsersList, null,null);
            IPropertySchemaSet propertySet = new ValueSetCollectionTest.MockPropertySchemaSet(new PropertySchemaFactory());
            IPropertySetCollection collection = new ValueSetCollectionTest.MockPropertySetCollection(propertySet);
            varUsers.SetupVariable(collection);
            Assert.AreEqual(1, collection.Count);
            var valueSchema = collection.Schemas.GetSchema("users");
            Assert.IsNotNull(valueSchema);
            Assert.AreEqual("UsersList", valueSchema.TypeName);
            string val= collection.Get<string>("users");
            Assert.IsTrue(string.IsNullOrEmpty(val));
        }

        [Test]
        public void TestVariableDefinitionRoleListSetCollectionValueToString()
        {
            VariableDefinition varRoles=new VariableDefinition("roles",string.Empty,VariableTypeEnum.RolesList, null,null);
            IPropertySchemaSet propertySet = new ValueSetCollectionTest.MockPropertySchemaSet(new PropertySchemaFactory());
            IPropertySetCollection collection = new ValueSetCollectionTest.MockPropertySetCollection(propertySet);
            varRoles.SetupVariable(collection);
            Assert.AreEqual(1, collection.Count);
            var valueSchema = collection.Schemas.GetSchema("roles");
            Assert.IsNotNull(valueSchema);
            Assert.AreEqual("RolesList", valueSchema.TypeName);
            string val= collection.Get<string>("roles");
            Assert.IsTrue(string.IsNullOrEmpty(val));
        }

        [Test]
        public void TestVariableDefinitionGroupsListSetCollectionValueToString()
        {
            VariableDefinition varGroups=new VariableDefinition("groups",string.Empty,VariableTypeEnum.GroupsList, null,null);
            IPropertySchemaSet propertySet = new ValueSetCollectionTest.MockPropertySchemaSet(new PropertySchemaFactory());
            IPropertySetCollection collection = new ValueSetCollectionTest.MockPropertySetCollection(propertySet);
            varGroups.SetupVariable(collection);
            Assert.AreEqual(1, collection.Count);
            var valueSchema = collection.Schemas.GetSchema("groups");
            Assert.IsNotNull(valueSchema);
            Assert.AreEqual("GroupsList", valueSchema.TypeName);
            string val= collection.Get<string>("groups");
            Assert.IsTrue(string.IsNullOrEmpty(val));

        }

        [Test]
        public void TestVariableDefinitionsSetProperiesInCollection()
        {
            VariableDefinition vdChar=new VariableDefinition("v_char",string.Empty,VariableTypeEnum.Char, new StepHandlerDefinition()
                ,new ConstraintDefinition());
            VariableDefinition vdInt=new VariableDefinition("v_int",string.Empty,VariableTypeEnum.Int, new StepHandlerDefinition(),new ConstraintDefinition());
            VariableDefinition vdDecimal=new VariableDefinition("v_decimal",string.Empty,VariableTypeEnum.Decimal,  new StepHandlerDefinition(),new ConstraintDefinition());
            VariableDefinition vdString=new VariableDefinition("v_string",string.Empty,VariableTypeEnum.String, new StepHandlerDefinition(),new ConstraintDefinition());
            VariableDefinition vdJson=new VariableDefinition("v_json",string.Empty,VariableTypeEnum.Json, new StepHandlerDefinition(),new ConstraintDefinition());
            VariableDefinition vdObject=new VariableDefinition("v_object",string.Empty,VariableTypeEnum.Char, new StepHandlerDefinition(),new ConstraintDefinition());
            VariableDefinition vdNone=new VariableDefinition("v_none",string.Empty,VariableTypeEnum.None, new StepHandlerDefinition(),new ConstraintDefinition());
            VariableDefinition vdBool=new VariableDefinition("v_bool",string.Empty,VariableTypeEnum.Boolean, new StepHandlerDefinition(),new ConstraintDefinition());

            VariableDefinition[] defs = {vdChar, vdInt, vdDecimal, vdString, vdJson, vdObject, vdNone, vdBool};

            IPropertySchemaSet propertySet = new ValueSetCollectionTest.MockPropertySchemaSet(new PropertySchemaFactory());
            IPropertySetCollection collection = new ValueSetCollectionTest.MockPropertySetCollection(propertySet);
            foreach (VariableDefinition variableDefinition in defs)
            {
                variableDefinition.SetupVariable(collection);
            }
            Assert.AreEqual(defs.Length - 1, collection.Count);
            Assert.IsNull(collection.Get<char?>("v_char"));
            collection.Set("v_char",(char?)'c');
            Assert.AreEqual('c',collection.Get<char?>("v_char"));

            Assert.IsNull(collection.Get<int?>("v_int"));
            collection.Set("v_int",(int?)100);
            Assert.AreEqual(100,collection.Get<int?>("v_int"));
        }

        [Test]
        public void SetupFlowAndAssignVariablesOnStepEntry()
        {
            IPropertySchemaSet propertySet = new ValueSetCollectionTest.MockPropertySchemaSet(new PropertySchemaFactory());
            IPropertySetCollection collection = new ValueSetCollectionTest.MockPropertySetCollection(propertySet);
            ProcessRuntimeEnvironment env = new ProcessRuntimeEnvironment(null, collection);
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "com.klaudwerk.workflow.renewal",
                name: "Renewal", description: "Policy Renewal");
            ProcessDefinition pd = builder.Variables().Name("v_bool").Type(VariableTypeEnum.Boolean).Done()
                .Variables().Name("v_int").Type(VariableTypeEnum.Int).Handler().IocService("container_service").Done().Done()
                .Variables().Name("v_string").Type(VariableTypeEnum.String).Done()
                .Start("s_1").SetName("start")
                .Vars().Name("v_bool").OnExit().Done()
                .Vars().Name("v_int").OnExit().Done()
                .Vars().Name("v_string").OnExit().Done()
                .OnEntry().Language(ScriptLanguage.CSharpScript).Body(
                        " PropertySet.Set(\"v_bool\",(bool?)true);"+
                        " PropertySet.Set(\"v_int\",(int?)455);"+
                        " PropertySet.Set(\"v_string\",\"value\");"+
                        " return 1;").AddReferences("PropertySet, Version=1.0.0.0").Done().Done()
                .End("e_1").SetName("end").Done()
                .Link().From("s_1").To("e_1").Done()
                .Build();
            ProcessRuntimeService rtb=new ProcessRuntimeService();
            var processRuntime = rtb.Create(pd,collection);
            Assert.IsNotNull(processRuntime);
            string[] errors;
            Assert.IsTrue(processRuntime.TryCompile(out errors));
            StepRuntime start = processRuntime.StartSteps[0];
            var status = processRuntime.Execute(start,env);
            Assert.IsNotNull(status);
            Assert.AreEqual(StepExecutionStatusEnum.Ready,status.Item1.Status);
            StepRuntime nextStep = status.Item2;
            Assert.IsNotNull(nextStep);
            Assert.IsTrue(nextStep.IsEnd);
            // check variables
            Assert.AreEqual(true,collection.Get<bool?>("v_bool"));
            Assert.AreEqual(455,collection.Get<int?>("v_int"));
            Assert.AreEqual("value",collection.Get<string>("v_string"));
        }

        [Test]
        public void TestRuntimeProcessShouldSetDefaultAndPossibleValuesOnVariables()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "com.klaudwerk.workflow.renewal",
                name: "Renewal", description: "Policy Renewal");
            IReadOnlyList<ProcessValidationResult> result;
            bool isValid = builder
                    .Variables().Name("str").Type(VariableTypeEnum.String)
                        .Constraints().DefaultValue("1").PossibeValues(new object[] { "1", "2", "3" }).Done()
                    .Done()
                    .Variables().Name(("int")).Type(VariableTypeEnum.Int)
                        .Constraints().DefaultValue(100).PossibeValues(new object[] { 100, 200, 200 }).Done()
                    .Done()
                    .Variables().Name("bool").Type(VariableTypeEnum.Boolean)
                        .Constraints().DefaultValue(true).Done()
                    .Done()
                    .Variables().Name("decimal").Type(VariableTypeEnum.Decimal)
                        .Constraints().DefaultValue(12.5d).PossibeValues(new object[] { 10.5d, 11.5d, 12.5d }).Done()
                    .Done()
                    .Variables().Name("char").Type(VariableTypeEnum.Char)
                        .Constraints().DefaultValue('c').PossibeValues(new object[] { 'a', 'b', 'c' }).Done()
                    .Done()
                    .Variables().Type(VariableTypeEnum.Json).Name("json")
                        .Constraints().DefaultValue("{s:100}").PossibeValues(new object[] { "{s:100}", "{s:200}", "{s:300}" }).Done()
                    .Done()
                    .Variables().Type(VariableTypeEnum.Object).Name("obj")
                        .Constraints().DefaultValue("1").PossibeValues(new object[] { "1", "2", "3" }).Done()
                    .Done()
                    .Variables().Type(VariableTypeEnum.GroupsList).Name("grp")
                        .Constraints().DefaultValue("gr1").PossibeValues(new object[] { "gr1", "gr2", "gr3" }).Done()
                    .Done()
                    .Variables().Type(VariableTypeEnum.RolesList).Name("role")
                        .Constraints().DefaultValue("r1").PossibeValues(new object[] { "r1", "r2", "r3" }).Done()
                    .Done()
                    .Variables().Type(VariableTypeEnum.UsersList).Name("usr")
                        .Constraints().DefaultValue("u1").PossibeValues(new object[] { "u1", "u2", "u3" }).Done()
                    .Done()

                .Start("s_1").SetName("Start").Done()
                .End("e_1").SetName("End Process").Done()
                .Link().From("s_1").To("e_1").Name("finish").Done()
                .TryValidate(out result);
            Assert.IsTrue(isValid);
            var processDefinition = builder.Build();
            Assert.IsNotNull(processDefinition);
            IPropertySchemaSet propertySet = new ValueSetCollectionTest.MockPropertySchemaSet(new PropertySchemaFactory());
            IPropertySetCollection collection = new ValueSetCollectionTest.MockPropertySetCollection(propertySet);
            ProcessRuntimeEnvironment env = new ProcessRuntimeEnvironment(null, collection);
            ProcessRuntimeService rtb = new ProcessRuntimeService();
            var processRuntime = rtb.Create(processDefinition, collection);
            Assert.IsNotNull(processRuntime);
            string[] errors;
            Assert.IsTrue(processRuntime.TryCompile(out errors));
            // collection variables should be set
            Assert.AreEqual(10,collection.Count);
            IValueSchema<object> schema = collection.Schemas.GetSchema("str");
            Assert.IsNotNull(schema);
            Assert.IsNotNull(schema.DefaultValue);
            Assert.IsNotNull(schema.PossibleValues);
            Assert.AreEqual(3, schema.PossibleValues.Count());
            Assert.AreEqual(new object[] { "1", "2", "3" }, schema.PossibleValues);

            schema = collection.Schemas.GetSchema("int");
            Assert.IsNotNull(schema);
            Assert.IsNotNull(schema.DefaultValue);
            Assert.IsNotNull(schema.PossibleValues);
            Assert.AreEqual(3, schema.PossibleValues.Count());
            Assert.AreEqual(new object[] { 100, 200, 200 }, schema.PossibleValues);

            schema = collection.Schemas.GetSchema("bool");
            Assert.IsNotNull(schema);
            Assert.IsNotNull(schema.DefaultValue);
            Assert.IsNotNull(schema.PossibleValues);
            Assert.AreEqual(2, schema.PossibleValues.Count());
            Assert.AreEqual(new object[] { true, false }, schema.PossibleValues);

            schema = collection.Schemas.GetSchema("decimal");
            Assert.IsNotNull(schema);
            Assert.IsNotNull(schema.DefaultValue);
            Assert.IsNotNull(schema.PossibleValues);
            Assert.AreEqual(3, schema.PossibleValues.Count());
            Assert.AreEqual(new object[] { 10.5d, 11.5d, 12.5d }, schema.PossibleValues);

            schema = collection.Schemas.GetSchema("char");
            Assert.IsNotNull(schema);
            Assert.IsNotNull(schema.DefaultValue);
            Assert.IsNotNull(schema.PossibleValues);
            Assert.AreEqual(3, schema.PossibleValues.Count());
            Assert.AreEqual(new object[] { 'a', 'b', 'c' }, schema.PossibleValues);

            schema = collection.Schemas.GetSchema("json");
            Assert.IsNotNull(schema);
            Assert.IsNotNull(schema.DefaultValue);
            Assert.IsNotNull(schema.PossibleValues);
            Assert.AreEqual(3, schema.PossibleValues.Count());
            Assert.AreEqual(new object[] { "{s:100}", "{s:200}", "{s:300}" }, schema.PossibleValues);

            schema = collection.Schemas.GetSchema("obj");
            Assert.IsNotNull(schema);
            Assert.IsNotNull(schema.DefaultValue);
            Assert.IsNotNull(schema.PossibleValues);
            Assert.AreEqual(3, schema.PossibleValues.Count());
            Assert.AreEqual(new object[] { "1", "2", "3" }, schema.PossibleValues);

            schema = collection.Schemas.GetSchema("grp");
            Assert.IsNotNull(schema);
            Assert.IsNotNull(schema.DefaultValue);
            Assert.IsNotNull(schema.PossibleValues);
            Assert.AreEqual(3, schema.PossibleValues.Count());
            Assert.AreEqual(new object[] { "gr1", "gr2", "gr3" }, schema.PossibleValues);

            schema = collection.Schemas.GetSchema("role");
            Assert.IsNotNull(schema);
            Assert.IsNotNull(schema.DefaultValue);
            Assert.IsNotNull(schema.PossibleValues);
            Assert.AreEqual(3, schema.PossibleValues.Count());
            Assert.AreEqual(new object[] { "r1", "r2", "r3" }, schema.PossibleValues);


            schema = collection.Schemas.GetSchema("usr");
            Assert.IsNotNull(schema);
            Assert.IsNotNull(schema.DefaultValue);
            Assert.IsNotNull(schema.PossibleValues);
            Assert.AreEqual(3, schema.PossibleValues.Count());
            Assert.AreEqual(new object[] { "u1", "u2", "u3" }, schema.PossibleValues);
        }

        [Test]
        public void SetupFlowTestRequiredVariables()
        {
            IPropertySchemaSet propertySet = new ValueSetCollectionTest.MockPropertySchemaSet(new PropertySchemaFactory());
            IPropertySetCollection collection = new ValueSetCollectionTest.MockPropertySetCollection(propertySet);
            ProcessRuntimeEnvironment env = new ProcessRuntimeEnvironment(null, collection);
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "com.klaudwerk.workflow.renewal",
                name: "Renewal", description: "Policy Renewal");
            ProcessDefinition pd = builder.Variables().Name("v_bool").Type(VariableTypeEnum.Boolean).Done()
                .Variables().Name("v_int").Type(VariableTypeEnum.Int).Done()
                .Variables().Name("v_string").Type(VariableTypeEnum.String).Handler().IocService("container_service").Done().Done()
                .Start("s_1").SetName("start")
                .Vars().Name("v_bool").Done()
                .Vars().Name("v_int").Done()
                .Vars().Name("v_string").Done()
                .OnEntry().Language(ScriptLanguage.CSharpScript).Body(
                        " PropertySet.Set(\"v_bool\",(bool?)true);" +
                        " PropertySet.Set(\"v_int\",(int?)455);" +
                        " PropertySet.Set(\"v_string\",\"value\");" +
                        " return 1;").AddReferences("PropertySet, Version=1.0.0.0").Done().Done()
                .Step("step")
                .Vars().Name("v_int").OnExit().Done()
                .Vars().Name("v_string").OnEntry().Done()
                .Done()
                .End("e_1").SetName("end").Done()
                .Link().From("s_1").To("step").Done()
                .Link().From("step").To("e_1").Done()
                .Build();
            ProcessRuntimeService rtb = new ProcessRuntimeService();
            var processRuntime = rtb.Create(pd, collection);
            Assert.IsNotNull(processRuntime);
            string[] errors;
            Assert.IsTrue(processRuntime.TryCompile(out errors));
            StepRuntime start = processRuntime.StartSteps[0];
            var status = processRuntime.Execute(start, env);
            Assert.IsNotNull(status);
            Assert.AreEqual(StepExecutionStatusEnum.Ready, status.Item1.Status);
            StepRuntime nextStep = status.Item2;
            Assert.IsNotNull(nextStep);
            Assert.AreEqual("step", nextStep.StepId);
            // check variables
            Assert.AreEqual(true, collection.Get<bool?>("v_bool"));
            Assert.AreEqual(455, collection.Get<int?>("v_int"));
            Assert.AreEqual("value", collection.Get<string>("v_string"));
            Assert.IsNotNull(nextStep.StepDefinition.VariablesMap);
            Assert.AreEqual(2, nextStep.StepDefinition.VariablesMap.Length);
            Assert.IsNotNull(pd.Variables[2].HandlerDefinition);
            Assert.AreEqual("container_service", pd.Variables[2].HandlerDefinition.IocName);
            Assert.AreEqual(VarRequiredEnum.OnExit, nextStep.StepDefinition.VariablesMap[0].Required);
            Assert.AreEqual(VarRequiredEnum.OnEntry, nextStep.StepDefinition.VariablesMap[1].Required);
        }

        [Test]
        public void SetupProcessExecuteFull()
        {
            IPropertySchemaSet propertySet = new ValueSetCollectionTest.MockPropertySchemaSet(new PropertySchemaFactory());
            IPropertySetCollection collection = new ValueSetCollectionTest.MockPropertySetCollection(propertySet);
            ProcessRuntimeEnvironment env = new ProcessRuntimeEnvironment(null, collection);
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "com.klaudwerk.workflow.renewal",
                name: "Renewal", description: "Policy Renewal");
            ProcessDefinition pd = builder
                .Start("s_1").SetName("start").Done()
                .Step("s_2").SetName("step").Done()
                .End("e_1").SetName("end").Done()
                .Link().From("s_1").To("s_2").Done()
                .Link().From("s_2").To("e_1").Done()
                .Build();
                ProcessRuntimeService rtService=new ProcessRuntimeService();
            var processRuntime = rtService.Create(builder.Build(), collection);
            string[] errors;
            Assert.IsTrue(processRuntime.TryCompile(out errors));
            Assert.IsNotNull(processRuntime);
            Assert.AreEqual(1,processRuntime.StartSteps.Count);
            Assert.AreEqual(ProcessStateEnum.NotStarted,processRuntime.State);
            var status = processRuntime.Execute(processRuntime.StartSteps[0],env);
            Assert.IsNotNull(status);
            Assert.AreEqual(StepExecutionStatusEnum.Ready, status.Item1.Status);
            Assert.IsNotNull(status.Item2);
            Assert.AreEqual("s_2",status.Item2.StepId);
            status = processRuntime.Execute(status.Item2,env);
            Assert.IsNotNull(status);
            Assert.AreEqual(StepExecutionStatusEnum.Ready, status.Item1.Status);
            Assert.IsNotNull(status.Item2);
            Assert.AreEqual("e_1",status.Item2.StepId);
            status = processRuntime.Execute(status.Item2,env);
            Assert.IsNotNull(status);
            Assert.AreEqual(StepExecutionStatusEnum.Completed, status.Item1.Status);
            Assert.AreEqual("e_1",status.Item2.StepId);
        }

        [Test]
        public void SetupProcessCompileWithVariables()
        {
            IPropertySchemaSet propertySet = new ValueSetCollectionTest.MockPropertySchemaSet(new PropertySchemaFactory());
            IPropertySetCollection collection = new ValueSetCollectionTest.MockPropertySetCollection(propertySet);
            ProcessRuntimeEnvironment env = new ProcessRuntimeEnvironment(null, collection);
            string scriptBody = "string[] values=new []{\"One\",\"Two\",\"Three\"};\n" +
                                "SetPossibleValues(\"v_str\",values);\n" +
                                "return 1;\n";
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "com.klaudwerk.workflow.renewal",
                name: "Renewal", description: "Policy Renewal");
            ProcessDefinition pd = builder
                .Variables().Name("v_str").Type(VariableTypeEnum.String).Handler().Script()
                    .Language(ScriptLanguage.CSharpScript)
                    .Body(scriptBody).Done().Done().Done()
                .Variables().Name("v_int").Type(VariableTypeEnum.Int).Done()
                .Start("s_1").SetName("start").Done()
                .Step("s_2").SetName("step").Done()
                .End("e_1").SetName("end").Done()
                .Link().From("s_1").To("s_2").Done()
                .Link().From("s_2").To("e_1").Done()
                .Build();
            ProcessRuntimeService rtService=new ProcessRuntimeService();
            var processRuntime = rtService.Create(builder.Build(), collection);
            string[] errors;
            Assert.IsTrue(processRuntime.TryCompile(out errors),string.Join(";",errors ?? new string[]{}));
            Assert.IsNotNull(processRuntime.Variables);
            Assert.AreEqual(2,processRuntime.Variables.Count);
            foreach (VariableRuntime variable in processRuntime.Variables)
            {
                Assert.IsTrue(variable.IsCompiled);
                Assert.IsTrue(variable.Evaluate(env).Result,"Evaluation failed!");
            }
            Assert.AreEqual(2,collection.Count);
            var valueSchema = collection.Schemas.GetSchema("v_str");
            Assert.IsNotNull(valueSchema);
            Assert.IsNotNull(valueSchema.PossibleValues);
            Assert.AreEqual(3,valueSchema.PossibleValues.Count());
            Assert.AreEqual(new []{"One","Two","Three"},valueSchema.PossibleValues);
        }

        [Test]
        public void TestProcessShouldBeSuspendedInTaskStep()
        {
            IPropertySchemaSet propertySet = new ValueSetCollectionTest.MockPropertySchemaSet(new PropertySchemaFactory());
            IPropertySetCollection collection = new ValueSetCollectionTest.MockPropertySetCollection(propertySet);
            Mock<IProcessRuntimeEnvironment> mEnv=new Mock<IProcessRuntimeEnvironment>();
            mEnv.SetupGet(m => m.PropertySet).Returns(collection);
            mEnv.Setup(m => m.TaskServiceAsync()).Returns(() =>
                    Task.FromResult(new ExecutionResult(StepExecutionStatusEnum.Suspend)));
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "com.klaudwerk.workflow.renewal",
                name: "Renewal", description: "Policy Renewal");
            ProcessDefinition pd = builder
                .Start("s_1").SetName("start").Done()
                .Step("s_2").SetName("step").Handler().HumanTask().Done().Done()
                .End("e_1").SetName("end").Done()
                .Link().From("s_1").To("s_2").Done()
                .Link().From("s_2").To("e_1").Done()
                .Build();
            ProcessRuntimeService rtService=new ProcessRuntimeService();
            var processRuntime = rtService.Create(builder.Build(), collection);
            string[] errors;
            Assert.IsTrue(processRuntime.TryCompile(out errors));
            Assert.IsNotNull(processRuntime);
            Assert.AreEqual(1,processRuntime.StartSteps.Count);
            var status = processRuntime.Execute(processRuntime.StartSteps[0],mEnv.Object);
            Assert.IsNotNull(status);
            Assert.AreEqual(StepExecutionStatusEnum.Ready, status.Item1.Status);
            Assert.IsNotNull(status.Item2);
            Assert.AreEqual("s_2",status.Item2.StepId);
            status = processRuntime.Execute(status.Item2,mEnv.Object);
            Assert.IsNotNull(status);
            Assert.AreEqual(StepExecutionStatusEnum.Suspend, status.Item1.Status);
            Assert.AreEqual(ProcessStateEnum.Suspended,processRuntime.State);
            Assert.IsNotNull(processRuntime.SuspendedInStep);
            Assert.AreEqual("s_2",processRuntime.SuspendedInStep.StepId);
        }

        [Test]
        public void SuspendedFlowCannotContinue()
        {
            IPropertySchemaSet propertySet = new ValueSetCollectionTest.MockPropertySchemaSet(new PropertySchemaFactory());
            IPropertySetCollection collection = new ValueSetCollectionTest.MockPropertySetCollection(propertySet);
            Mock<IProcessRuntimeEnvironment> mEnv = new Mock<IProcessRuntimeEnvironment>();
            mEnv.SetupGet(m => m.PropertySet).Returns(collection);
            mEnv.Setup(m => m.TaskServiceAsync()).Returns(() =>
                    Task.FromResult(new ExecutionResult(StepExecutionStatusEnum.Suspend)));

            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "com.klaudwerk.workflow.renewal",
                name: "Renewal", description: "Policy Renewal");
            IReadOnlyList<ProcessValidationResult> result;
            bool isValid =
                builder.Variables().Name("PolicyNumber").Type(VariableTypeEnum.String).Done()
                .Variables().Name("Count").Type(VariableTypeEnum.Int).Done()
                .Start("task_1").Handler().HumanTask().Done().Done()
                .End("e_1").SetName("End Process").Done()
                .Link().From("task_1").To("e_1").Name("end").Done()
                .TryValidate(out result);
            Assert.IsTrue(isValid);
            ProcessRuntimeService rtService = new ProcessRuntimeService();
            var processRuntime = rtService.Create(builder.Build(), collection);
            string[] errors;
            Assert.IsTrue(processRuntime.TryCompile(out errors));
            processRuntime.Execute(processRuntime.StartSteps[0], mEnv.Object);
            Assert.IsNotNull(processRuntime.SuspendedInStep);
            Assert.AreEqual("task_1",processRuntime.SuspendedInStep.StepId);
            Assert.Throws<InvalidProcessStateException>(() =>
            {
                processRuntime.Execute(processRuntime.StartSteps[0], mEnv.Object);
            });
        }
        [Test]
        public void TestBuildProcessRuntime()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "com.klaudwerk.workflow.renewal",
                name: "Renewal", description: "Policy Renewal");
            builder.Start("s_1").SetName("Start").Handler().Script().Language(ScriptLanguage.CSharpScript)
                .Body("return 1;").Done().Done().Done()
                .Step("s_2").SetName("DoSomething").Handler().Script().Language(ScriptLanguage.CSharpScript)
                .Body("return 1;").Done().Done().Done()
                .End("e_1").Done()
                .Link().From("s_1").To("s_2").Done()
                .Link().From("s_2").To("e_1").Done()
                .Variables().Name("PolicyNumber").Type(VariableTypeEnum.String).Done()
                .Variables().Name("AccountName").Type(VariableTypeEnum.String).Done()
                .Variables().Name("AccountId").Type(VariableTypeEnum.Int).Done();
            IReadOnlyList<ProcessValidationResult> result;
            Assert.IsTrue(builder.TryValidate(out result));
            ProcessDefinition processDefinition = builder.Build();
            Assert.IsNotNull(processDefinition);
        }

        [Test]
        public void TestWakeUpSuspendedProcess()
        {
            ProcessDefinition pd = BuildProcessWithTasks();
            IPropertySchemaSet propertySet = new ValueSetCollectionTest.MockPropertySchemaSet(new PropertySchemaFactory());
            IPropertySetCollection collection = new ValueSetCollectionTest.MockPropertySetCollection(propertySet);
            Mock<IProcessRuntimeEnvironment> mEnv = new Mock<IProcessRuntimeEnvironment>();
            mEnv.SetupGet(m => m.PropertySet).Returns(collection);
            mEnv.Setup(m => m.TaskServiceAsync()).Returns(() =>
                    Task.FromResult(new ExecutionResult(StepExecutionStatusEnum.Suspend)));
            ProcessRuntimeService rtService = new ProcessRuntimeService();
            var processRuntime = rtService.Create(pd, collection);
            string[] errors;
            Assert.IsTrue(processRuntime.TryCompile(out errors));
            var execResult = processRuntime.Execute(processRuntime.StartSteps[0], mEnv.Object);
            processRuntime.Execute(execResult.Item2, mEnv.Object);
            Assert.IsNotNull(processRuntime.SuspendedInStep);
            Assert.AreEqual("task_1", processRuntime.SuspendedInStep.StepId);
            // now wake up the process
            Tuple<ExecutionResult, StepRuntime> state = processRuntime.Continue(mEnv.Object);
            Assert.IsNotNull(state);
            Assert.AreEqual(StepExecutionStatusEnum.Ready, state.Item1.Status);
            Assert.IsNotNull(state.Item2);
            Assert.AreEqual("e_1",state.Item2.StepId);
        }

        [Test]
        public void TestWakeUpSuspendedProcessErrorOnExit()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "com.klaudwerk.workflow.renewal",
                name: "Renewal", description: "Policy Renewal");
            IReadOnlyList<ProcessValidationResult> result;
            bool isValid =
                builder.Variables().Name("PolicyNumber").Type(VariableTypeEnum.String).Done()
                .Variables().Name("Count").Type(VariableTypeEnum.Int).Done()
                .Start("s_1").SetName("Start").Done()
                .Step("task_1").Handler().HumanTask().Done()
                .OnExit().Language(ScriptLanguage.CSharpScript).Body("return 0;").Done().Done()
                .End("e_1").SetName("End Process").Done()
                .Link().From("s_1").To("task_1").Name("task").Done()
                .Link().From("task_1").To("e_1").Name("end").Done()
                .TryValidate(out result);
            Assert.IsTrue(isValid);
            ProcessDefinition pd = builder.Build();
            IPropertySchemaSet propertySet = new ValueSetCollectionTest.MockPropertySchemaSet(new PropertySchemaFactory());
            IPropertySetCollection collection = new ValueSetCollectionTest.MockPropertySetCollection(propertySet);
            Mock<IProcessRuntimeEnvironment> mEnv = new Mock<IProcessRuntimeEnvironment>();
            mEnv.SetupGet(m => m.PropertySet).Returns(collection);
            mEnv.Setup(m => m.TaskServiceAsync()).Returns(() =>
                    Task.FromResult(new ExecutionResult(StepExecutionStatusEnum.Suspend)));
            ProcessRuntimeService rtService = new ProcessRuntimeService();
            var processRuntime = rtService.Create(pd, collection);
            string[] errors;
            Assert.IsTrue(processRuntime.TryCompile(out errors));
            var execResult = processRuntime.Execute(processRuntime.StartSteps[0], mEnv.Object);
            processRuntime.Execute(execResult.Item2, mEnv.Object);
            Assert.IsNotNull(processRuntime.SuspendedInStep);
            Assert.AreEqual("task_1", processRuntime.SuspendedInStep.StepId);
            // now wake up the process
            Tuple<ExecutionResult, StepRuntime> state = processRuntime.Continue(mEnv.Object);
            Assert.IsNotNull(state);
            Assert.AreEqual(StepExecutionStatusEnum.Failed,state.Item1.Status);
            Assert.IsNotNull(state.Item2);
            Assert.AreEqual("task_1",state.Item2.StepId);
        }

        [Test]
        public void TestContinueProcessExecutionWithTransition()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "com.klaudwerk.workflow.renewal",
                name: "Renewal", description: "Policy Renewal");
            IReadOnlyList<ProcessValidationResult> result;
            bool isValid =
                builder.Variables().Name("PolicyNumber").Type(VariableTypeEnum.String).Done()
                .Variables().Name("Count").Type(VariableTypeEnum.Int).Done()
                .Start("s_1").SetName("Start").Done()
                .Step("task_1").Handler().HumanTask().Done()
                .OnExit().Language(ScriptLanguage.CSharpScript).Body("return 1;").Done().Done()
                .Step("s_2").Done()
                .Step("s_3").Done()
                .End("e_1").SetName("End Process").Done()
                .Link().From("s_1").To("task_1").Name("task").Done()
                .Link().From("task_1").To("s_2").Name("approved").Done()
                .Link().From("task_1").To("s_3").Name("rejected").Done()
                .Link().From("task_1").To("e_1").Name("end").Done()
                .Link().From("s_2").To("e_1").Done()
                .Link().From("s_3").To("e_1").Done()
                .TryValidate(out result);
            Assert.IsTrue(isValid);
            ProcessDefinition pd = builder.Build();
            IPropertySchemaSet propertySet = new ValueSetCollectionTest.MockPropertySchemaSet(new PropertySchemaFactory());
            IPropertySetCollection collection = new ValueSetCollectionTest.MockPropertySetCollection(propertySet);
            Mock<IProcessRuntimeEnvironment> mEnv = new Mock<IProcessRuntimeEnvironment>();
            mEnv.SetupGet(m => m.PropertySet).Returns(collection);
            mEnv.Setup(m => m.TaskServiceAsync()).Returns(() =>
                    Task.FromResult(new ExecutionResult(StepExecutionStatusEnum.Suspend)));
            ProcessRuntimeService rtService = new ProcessRuntimeService();
            var processRuntime = rtService.Create(pd, collection);
            string[] errors;
            Assert.IsTrue(processRuntime.TryCompile(out errors));
            var execResult = processRuntime.Execute(processRuntime.StartSteps[0], mEnv.Object);
            processRuntime.Execute(execResult.Item2, mEnv.Object);
            Assert.IsNotNull(processRuntime.SuspendedInStep);
            Assert.AreEqual("task_1", processRuntime.SuspendedInStep.StepId);
            // now wake up the process
            mEnv.SetupGet(m => m.Transition).Returns("approved");
            Tuple<ExecutionResult, StepRuntime> state = processRuntime.Continue(mEnv.Object);
            Assert.IsNotNull(state);
            Assert.IsNotNull(state);
            Assert.AreEqual(StepExecutionStatusEnum.Ready, state.Item1.Status);
            Assert.IsNotNull(state.Item2);
            Assert.AreEqual("s_2", state.Item2.StepId);
        }


        #endregion

        private ProcessDefinition BuildProcessWithTasks()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "com.klaudwerk.workflow.renewal",
                name: "Renewal", description: "Policy Renewal");
            IReadOnlyList<ProcessValidationResult> result;
            bool isValid =
                builder.Variables().Name("PolicyNumber").Type(VariableTypeEnum.String).Done()
                .Variables().Name("Count").Type(VariableTypeEnum.Int).Done()
                .Start("s_1").SetName("Start").Done()
                .Step("task_1").Handler().HumanTask().Done().Done()
                .End("e_1").SetName("End Process").Done()
                .Link().From("s_1").To("task_1").Name("task").Done()
                .Link().From("task_1").To("e_1").Name("end").Done()
                .TryValidate(out result);
            Assert.IsTrue(isValid);
            return builder.Build();

        }
    }
}