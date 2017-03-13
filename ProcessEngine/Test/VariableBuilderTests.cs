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
using System.Runtime.Remoting;
using KlaudWerk.ProcessEngine.Builder;
using NUnit.Framework;

namespace KlaudWerk.ProcessEngine.Test
{
    [TestFixture]
    public class VariableBuilderTests
    {
        [Test]
        public void TestBuildProcessVariables()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "p_001", name: "Renewal", description: "Policy Renewal");
            builder.Variables().Name("PolicyNumber").Type(VariableTypeEnum.String)
                .Description("Associated Policy Number").Done();
            Assert.IsNotNull(builder.ProcessVariables);
            Assert.AreEqual(1,builder.ProcessVariables.Count);
        }

        [Test]
        public void VariableWithTheSameNameShouldBeReplaced()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "p_001", name: "Renewal", description: "Policy Renewal");
            builder.Variables().Name("PolicyNumber").Type(VariableTypeEnum.String)
                .Description("Associated Policy Number").Done();
            Assert.IsNotNull(builder.ProcessVariables);
            Assert.AreEqual(1,builder.ProcessVariables.Count);
            Assert.AreEqual("PolicyNumber",builder.ProcessVariables[0].VariableName);
            Assert.AreEqual(VariableTypeEnum.String,builder.ProcessVariables[0].VariableType);
            builder.Variables().Name("PolicyNumber").Type(VariableTypeEnum.Int).Done();
            Assert.IsNotNull(builder.ProcessVariables);
            Assert.AreEqual(1,builder.ProcessVariables.Count);
            Assert.AreEqual("PolicyNumber",builder.ProcessVariables[0].VariableName);
            Assert.AreEqual(VariableTypeEnum.Int,builder.ProcessVariables[0].VariableType);
        }

        [Test]
        public void FinalizationOfVariableBuilderWithoutNameShouldFail()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "p_001", name: "Renewal", description: "Policy Renewal");
            Assert.Throws<ArgumentNullException>(() =>
            {
                builder.Variables().Type(VariableTypeEnum.String)
                    .Description("Associated Policy Number").Done();
            });
        }

        [Test]
        public void TestSettingConstraintsForVariable()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "p_001", name: "Renewal", description: "Policy Renewal");
            builder.Variables().Name("PolicyNumber").Type(VariableTypeEnum.String)
                .Description("Associated Policy Number")
                .Constraints().MaxValue(100).MinValue(10).DefaultValue("DEFAULT").PossibeValues("1","2","3").Done().Done();
            Assert.IsNotNull(builder.ProcessVariables);
            Assert.AreEqual(1,builder.ProcessVariables.Count);
            var builderProcessVariable = builder.ProcessVariables[0];
            Assert.IsNotNull(builderProcessVariable.VariableConstraints);
            Assert.AreEqual("DEFAULT",builderProcessVariable.VariableConstraints.Default);
            Assert.AreEqual(10,builderProcessVariable.VariableConstraints.Min);
            Assert.AreEqual(100,builderProcessVariable.VariableConstraints.Max);
        }

        [Test]
        public void TestSettingPossibleValueOnVariable()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "p_001", name: "Renewal", description: "Policy Renewal");
            builder.Variables()
                .Name("List")
                .Type(VariableTypeEnum.String)
                .Constraints()
                .PossibeValues("1", "2", "3", "4").Done().Done();
            Assert.IsNotNull(builder.ProcessVariables);
            Assert.AreEqual(1, builder.ProcessVariables.Count);
            var vars = builder.ProcessVariables;
            Assert.IsNotNull(vars[0].VariableConstraints);
            Assert.IsNotNull(vars[0].VariableConstraints.Values);
            Assert.AreEqual(4,vars[0].VariableConstraints.Values.Length);
        }

        [Test]
        public void TestSetScriptOnVariable()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "p_001", name: "Renewal", description: "Policy Renewal");
            builder.Variables()
                .Name("List")
                .Type(VariableTypeEnum.Int)
                .Handler()
                .Script()
                .Language(ScriptLanguage.CSharpScript)
                .Body("return new int[]{1,2,3};").Done().Done().Done();

            Assert.IsNotNull(builder.ProcessVariables);
            Assert.AreEqual(1, builder.ProcessVariables.Count);
            var vars = builder.ProcessVariables;
            Assert.IsNotNull(vars[0].VariableHandler);
            Assert.AreEqual(StepHandlerTypeEnum.Script, vars[0].VariableHandler.StepHandlerType);
            Assert.IsNotNull(vars[0].VariableHandler.ScriptBuilder);
            Assert.AreEqual("return new int[]{1,2,3};",vars[0].VariableHandler.ScriptBuilder.ScriptBody);
        }

        [Test]
        public void TestSetIocServiceOnVariableBuilder()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "p_001", name: "Renewal", description: "Policy Renewal");
            builder.Variables()
                .Name("List")
                .Type(VariableTypeEnum.Int)
                .Handler()
                .IocService("IoCService").Done().Done();
            Assert.IsNotNull(builder.ProcessVariables);
            Assert.AreEqual(1, builder.ProcessVariables.Count);
            var vars = builder.ProcessVariables;
            Assert.IsNotNull(vars[0].VariableHandler);
            Assert.AreEqual(StepHandlerTypeEnum.IoC, vars[0].VariableHandler.StepHandlerType);
            Assert.IsNotNull(vars[0].VariableHandler.IocName);
            Assert.AreEqual("IoCService",vars[0].VariableHandler.IocName);
        }

        [Test]
        public void TestSetAssemblyNameToVariableBuilder()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "p_001", name: "Renewal", description: "Policy Renewal");
            builder.Variables()
                .Name("List")
                .Type(VariableTypeEnum.Int)
                .Handler()
                .Service("TypeName, AssemblyName").Done().Done();
            Assert.IsNotNull(builder.ProcessVariables);
            Assert.AreEqual(1, builder.ProcessVariables.Count);
            var vars = builder.ProcessVariables;
            Assert.IsNotNull(vars[0].VariableHandler);
            Assert.AreEqual(StepHandlerTypeEnum.Service, vars[0].VariableHandler.StepHandlerType);
            Assert.IsNotNull(vars[0].VariableHandler.FullClassName);
            Assert.AreEqual("TypeName, AssemblyName",vars[0].VariableHandler.FullClassName);

        }

        [Test]
        public void TestVariableBuilderRolesList()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "p_001", name: "Renewal", description: "Policy Renewal");
            builder.Variables()
                .Name("users").Type(VariableTypeEnum.RolesList).Constraints()
                .PossibeValues(new object[]{"admin","worker"}).Done().Done();
            Assert.IsNotNull(builder.ProcessVariables);
            Assert.AreEqual(1, builder.ProcessVariables.Count);
            Assert.AreEqual(new object[] { "admin", "worker" }, builder.ProcessVariables[0].VariableConstraints.Values);

        }

        [Test]
        public void TestVariableBuilderGroupList()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "p_001", name: "Renewal", description: "Policy Renewal");
            builder.Variables()
                .Name("users").Type(VariableTypeEnum.GroupsList).Constraints()
                .PossibeValues(new object[]{"g1","g2"}).Done().Done();
            Assert.IsNotNull(builder.ProcessVariables);
            Assert.AreEqual(1, builder.ProcessVariables.Count);
            Assert.AreEqual(new object[] { "g1", "g2" }, builder.ProcessVariables[0].VariableConstraints.Values);

        }

    }
}