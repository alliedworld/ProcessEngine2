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
using System.Threading.Tasks;
using Klaudwerk.PropertySet;
using Klaudwerk.PropertySet.Test;
using KlaudWerk.ProcessEngine.Builder;
using KlaudWerk.ProcessEngine.Definition;
using KlaudWerk.ProcessEngine.Runtime;
using Moq;
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
        public void TestExecuteScriptInEnvironment()
        {
            ProcessRuntimeEnvironment env = new ProcessRuntimeEnvironment(null) {ProcessEnvId = 1};
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
            var env=new ProcessRuntimeEnvironment(null);
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
            ProcessRuntimeEnvironment env=new ProcessRuntimeEnvironment(null);
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
            StepRuntime runtime=new StepRuntime(sd);
            string[] errors;
            Assert.IsTrue(runtime.TryCompile(out errors));
        }

        [Test]
        public void ScriptExecutionCanFailOnEnterValidationScript()
        {
            ProcessRuntimeEnvironment env=new ProcessRuntimeEnvironment(null);
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
            StepRuntime runtime=new StepRuntime(sd);
            string[] errors;
            Assert.IsTrue(runtime.TryCompile(out errors));
            Assert.IsFalse(runtime.ValidateOnEnter(env).Result.Valid);
            Assert.AreEqual(2,env.ProcessEnvId);
        }

        [Test]
        public void ScriptExecutionCanFailOnExitalidationScript()
        {
            ProcessRuntimeEnvironment env=new ProcessRuntimeEnvironment(null);
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
            StepRuntime runtime=new StepRuntime(sd);
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
            ProcessRuntimeEnvironment env = new ProcessRuntimeEnvironment(collection);
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
            StepRuntime runtime = new StepRuntime(sd);
            string[] errors;
            Assert.IsTrue(runtime.TryCompile(out errors));
            Assert.IsTrue(runtime.ValidateOnEnter(env).Result.Valid);
        }

        [Test]
        public void MissingOnEntryVariableShouldFailValidation()
        {
            IPropertySchemaSet propertySet = new ValueSetCollectionTest.MockPropertySchemaSet(new PropertySchemaFactory());
            IPropertySetCollection collection = new ValueSetCollectionTest.MockPropertySetCollection(propertySet);
            ProcessRuntimeEnvironment env = new ProcessRuntimeEnvironment(collection);
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
            StepRuntime runtime = new StepRuntime(sd);
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
            ProcessRuntimeEnvironment env = new ProcessRuntimeEnvironment(collection);
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
            StepRuntime runtime = new StepRuntime(sd);
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
            ProcessRuntimeEnvironment env=new ProcessRuntimeEnvironment(null);
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
            ProcessRuntimeEnvironment env=new ProcessRuntimeEnvironment(null);
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
            ProcessRuntimeEnvironment env=new ProcessRuntimeEnvironment(null);
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
            ProcessRuntimeEnvironment env = new ProcessRuntimeEnvironment(collection);
            env.ProcessEnvId = 1;
            StepDefinition sd = new StepDefinition(Guid.NewGuid(), "step_id", string.Empty, String.Empty, true, false,
                null, null, null, null, null, null,
                new StepHandlerDefinition(StepHandlerTypeEnum.None, null,string.Empty,string.Empty));
            StepRuntime runtime = new StepRuntime(sd);
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
            StepRuntime runtime = new StepRuntime(sd);
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
            StepRuntime runtime = new StepRuntime(sd);
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
            StepRuntime runtime = new StepRuntime(sd);
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
            ProcessRuntimeEnvironment env = new ProcessRuntimeEnvironment(collection);
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
            StepRuntime runtime = new StepRuntime(sd);
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
            ProcessRuntimeEnvironment env = new ProcessRuntimeEnvironment(collection);
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
            StepRuntime runtime = new StepRuntime(sd);
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
            ProcessRuntimeEnvironment env = new ProcessRuntimeEnvironment(collection);
            env.ProcessEnvId = 1;
            ScriptDefinition script=new ScriptDefinition(
                @"
                   throw new ArgumentNullException();
                ",
                ScriptLanguage.CSharpScript, new string[]{"System"},new string[]{});
            StepDefinition sd = new StepDefinition(Guid.NewGuid(), "step_id", string.Empty, String.Empty, true, false,
                null, null, null, null, null, null,
                new StepHandlerDefinition(StepHandlerTypeEnum.Script, script,string.Empty,string.Empty));
            StepRuntime runtime = new StepRuntime(sd);
            string[] errors;
            Assert.True(runtime.TryCompile(out errors));
            var result = runtime.ExecuteAsync(env).Result;
            Assert.IsNotNull(result);
            Assert.AreEqual(StepExecutionStatusEnum.Failed,result.Status);
        }
        #endregion

        #region Process Runtime Tests

        [Test]
        public void TestVariableDefinitionsSetProperiesInCollection()
        {
            VariableDefinition vdChar=new VariableDefinition("v_char",string.Empty,VariableTypeEnum.Char, string.Empty);
            VariableDefinition vdInt=new VariableDefinition("v_int",string.Empty,VariableTypeEnum.Int, string.Empty);
            VariableDefinition vdDecimal=new VariableDefinition("v_decimal",string.Empty,VariableTypeEnum.Decimal, string.Empty);
            VariableDefinition vdString=new VariableDefinition("v_string",string.Empty,VariableTypeEnum.String, string.Empty);
            VariableDefinition vdJson=new VariableDefinition("v_json",string.Empty,VariableTypeEnum.Json, string.Empty);
            VariableDefinition vdObject=new VariableDefinition("v_object",string.Empty,VariableTypeEnum.Char, string.Empty);
            VariableDefinition vdNone=new VariableDefinition("v_none",string.Empty,VariableTypeEnum.None, string.Empty);
            VariableDefinition vdBool=new VariableDefinition("v_bool",string.Empty,VariableTypeEnum.Boolean, string.Empty);

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
            ProcessRuntimeEnvironment env = new ProcessRuntimeEnvironment(collection);
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "com.klaudwerk.workflow.renewal",
                name: "Renewal", description: "Policy Renewal");
            ProcessDefinition pd = builder.Variables().Name("v_bool").Type(VariableTypeEnum.Boolean).Done()
                .Variables().Name("v_int").Type(VariableTypeEnum.Int).Done()
                .Variables().Name("v_string").Type(VariableTypeEnum.String).Done()
                .Start("s_1").SetName("start")
                .Vars().Name("v_bool").OnExit().Done()
                .Vars().Name("v_int").OnExit().Done()
                .Vars().Name("v_string").OnExit().Done()
                .OnEntry().Language(ScriptLanguage.CSharpScript).Body(
                        " PropertySet.Set(\"v_bool\",(bool?)true);"+
                        " PropertySet.Set(\"v_int\",(int?)455);"+
                        " PropertySet.Set(\"v_string\",\"value\");"+
                        " return 1;").Done().Done()
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
        public void SetupProcessExecuteFull()
        {
            IPropertySchemaSet propertySet = new ValueSetCollectionTest.MockPropertySchemaSet(new PropertySchemaFactory());
            IPropertySetCollection collection = new ValueSetCollectionTest.MockPropertySetCollection(propertySet);
            ProcessRuntimeEnvironment env = new ProcessRuntimeEnvironment(collection);
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