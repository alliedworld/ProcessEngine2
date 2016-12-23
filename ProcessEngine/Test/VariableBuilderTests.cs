using System;
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

    }
}