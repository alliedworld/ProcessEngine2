using KlaudWerk.ProcessEngine.Builder;
using NUnit.Framework;

namespace KlaudWerk.ProcessEngine.Test
{
    [TestFixture]
    public class ActionRelationBuilderTests
    {
        [Test]
        public void TestCreateActionRelationBuilder()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "p_001", name: "Renewal", description: "Policy Renewal");
            ActionRelationBuilder actionRelationBuilder=new ActionRelationBuilder(builder);
            Assert.AreEqual(builder,actionRelationBuilder.Done());

        }

        [Test]
        public void TestActionRelationForSteps()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "p_001", name: "Renewal", description: "Policy Renewal");
            ActionRelationBuilder actionRelationBuilder=new ActionRelationBuilder(builder);
            actionRelationBuilder.If("name").RequiredOnStep("step").Then("abc").RequiredOnStep("step1");
            Assert.AreEqual("name",actionRelationBuilder.SourceActionId);
            Assert.AreEqual("step",actionRelationBuilder.SourceStepId);
            Assert.AreEqual("abc",actionRelationBuilder.TargetActionId);
            Assert.AreEqual("step1",actionRelationBuilder.TargetStepId);
        }

        [Test]
        public void TestActionRelationCreateThroughProcessBuilder()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "p_001", name: "Renewal", description: "Policy Renewal");
            builder.BuildActionRelations().If("name").RequiredOnStep("step").Then("abc").RequiredOnStep("step1")
                .Done()
                .Done();
            Assert.IsNotNull(builder.ActionRelations);
            Assert.AreEqual(1,builder.ActionRelations.Count);
            ActionRelationBuilder actionRelationBuilder = builder.ActionRelations[0];
            Assert.AreEqual("name",actionRelationBuilder.SourceActionId);
            Assert.AreEqual("step",actionRelationBuilder.SourceStepId);
            Assert.AreEqual("abc",actionRelationBuilder.TargetActionId);
            Assert.AreEqual("step1",actionRelationBuilder.TargetStepId);
        }

        [Test]
        public void TestSameActionsRelationsCanBeOnlyAddedOnce()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "p_001", name: "Renewal", description: "Policy Renewal");
            builder.BuildActionRelations().If("name").RequiredOnStep("step").Then("abc").RequiredOnStep("step1")
                .Done()
                .Done();
            builder.BuildActionRelations().If("name").RequiredOnStep("step").Then("abc").RequiredOnStep("step1")
                .Done()
                .Done();
            builder.BuildActionRelations().If("name").RequiredOnStep("step").Then("abc").RequiredOnStep("step1")
                .Done()
                .Done();
            Assert.IsNotNull(builder.ActionRelations);
            Assert.AreEqual(1,builder.ActionRelations.Count);
            ActionRelationBuilder actionRelationBuilder = builder.ActionRelations[0];
            Assert.AreEqual("name",actionRelationBuilder.SourceActionId);
            Assert.AreEqual("step",actionRelationBuilder.SourceStepId);
            Assert.AreEqual("abc",actionRelationBuilder.TargetActionId);
            Assert.AreEqual("step1",actionRelationBuilder.TargetStepId);
        }

        [Test]
        public void TestSameSouceActionDifferentTargetStepCanBeAddedMultipleTimes()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "p_001", name: "Renewal", description: "Policy Renewal");
            builder.BuildActionRelations().If("name").RequiredOnStep("step").Then("abc").RequiredOnStep("step1")
                .Done()
                .Done();
            builder.BuildActionRelations().If("name").RequiredOnStep("step").Then("abc").RequiredOnStep("step2")
                .Done()
                .Done();
            builder.BuildActionRelations().If("name").RequiredOnStep("step").Then("abc").RequiredOnStep("step3")
                .Done()
                .Done();
            Assert.IsNotNull(builder.ActionRelations);
            Assert.AreEqual(3,builder.ActionRelations.Count);

        }

        [Test]
        public void TestSameSouceActionDifferentTargetActionCanBeAddedMultipleTimes()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "p_001", name: "Renewal", description: "Policy Renewal");
            builder.BuildActionRelations().If("name").RequiredOnStep("step").Then("abc1").RequiredOnStep("step1")
                .Done()
                .Done();
            builder.BuildActionRelations().If("name").RequiredOnStep("step").Then("abc2").RequiredOnStep("step1")
                .Done()
                .Done();
            builder.BuildActionRelations().If("name").RequiredOnStep("step").Then("abc3").RequiredOnStep("step1")
                .Done()
                .Done();
            Assert.IsNotNull(builder.ActionRelations);
            Assert.AreEqual(3,builder.ActionRelations.Count);
        }

        [Test]
        public void TestDifferentSouceActionSameTargetStepCanBeAddedMultipleTimes()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "p_001", name: "Renewal", description: "Policy Renewal");
            builder.BuildActionRelations().If("name1").RequiredOnStep("step").Then("abc1").RequiredOnStep("step1")
                .Done()
                .Done();
            builder.BuildActionRelations().If("name2").RequiredOnStep("step").Then("abc2").RequiredOnStep("step1")
                .Done()
                .Done();
            builder.BuildActionRelations().If("name3").RequiredOnStep("step").Then("abc3").RequiredOnStep("step1")
                .Done()
                .Done();
            Assert.IsNotNull(builder.ActionRelations);
            Assert.AreEqual(3,builder.ActionRelations.Count);

        }
    }
}