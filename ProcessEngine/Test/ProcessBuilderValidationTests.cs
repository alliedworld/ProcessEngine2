using System.Collections.Generic;
using NUnit.Framework;

namespace KlaudWerk.ProcessEngine.Test
{
    [TestFixture]
    public class ProcessBuilderValidationTests
    {
        [Test]
        public void ValidationShouldFailForEmptyBuilder()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "123", name: "Renewal", description: "Policy Renewal");
            IReadOnlyList<ProcessValidationResult> validationResults;
            Assert.IsFalse(builder.TryValidate(out validationResults));
            Assert.IsNotNull(validationResults);
            Assert.AreEqual(1 ,validationResults.Count);
            Assert.AreEqual(ProcessValidationResult.ItemEnum.Process, validationResults[0].Artifact);
            Assert.AreEqual("123",validationResults[0].ArtifactId);
        }

        [Test]
        public void ValidationShouldFailWhenNoStartStepPresent()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "123", name: "Renewal", description: "Policy Renewal");
            builder.Step("s_1").Done().End("e_1").Done().Link().From("s_1").To("e_1").Done();
            IReadOnlyList<ProcessValidationResult> validationResults;
            Assert.IsFalse(builder.TryValidate(out validationResults));
            Assert.IsNotNull(validationResults);
            Assert.AreEqual(2,validationResults.Count);
            Assert.AreEqual(ProcessValidationResult.ItemEnum.Process, validationResults[0].Artifact);
            Assert.AreEqual("123",validationResults[0].ArtifactId);
            Assert.AreEqual(ProcessValidationResult.ItemEnum.Link, validationResults[1].Artifact);
        }

        [Test]
        public void ValidationShouldFailWhenNoEndStemPresent()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "123", name: "Renewal", description: "Policy Renewal");
            builder.Start("start").Done().Step("s_1").Done().Link().From("start").To("s_1").Done()
                .Link().From("s_1").To("start").Done();
            IReadOnlyList<ProcessValidationResult> validationResults;
            Assert.IsFalse(builder.TryValidate(out validationResults));
            Assert.IsNotNull(validationResults);
            Assert.AreEqual(1,validationResults.Count);
            Assert.AreEqual(ProcessValidationResult.ItemEnum.Process, validationResults[0].Artifact);
            Assert.AreEqual("123",validationResults[0].ArtifactId);
        }

        [Test]
        public void ValidationShouldFailWithUnlinkedSteps()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "123", name: "Renewal", description: "Policy Renewal");
            builder.Start("start").Done().Step("s_1").Done().End("e_1").Done()
                .Link().From("start").To("s_1").Done();
            IReadOnlyList<ProcessValidationResult> validationResults;
            Assert.IsFalse(builder.TryValidate(out validationResults));
            Assert.IsNotNull(validationResults);
            Assert.AreEqual(2,validationResults.Count);
            Assert.AreEqual(ProcessValidationResult.ItemEnum.Link, validationResults[0].Artifact);
            Assert.AreEqual(ProcessValidationResult.ItemEnum.Link, validationResults[1].Artifact);
            Assert.AreEqual("123",validationResults[0].ArtifactId);

        }

        [Test]
        public void FullLnkedFlowShouldBeValidated()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "123", name: "Renewal", description: "Policy Renewal");
            builder.Start("start").Done().Step("s_1").Done().End("e_1").Done()
                .Link().From("start").To("s_1").Done()
                .Link().From("s_1").To("e_1").Done();
            IReadOnlyList<ProcessValidationResult> validationResults;
            Assert.IsTrue(builder.TryValidate(out validationResults));
            Assert.IsNotNull(validationResults);
            Assert.AreEqual(0,validationResults.Count);
        }

        [Test]
        public void ValidationShouldFailForMappedStepVArsNotDefinedInProcess()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "123", name: "Renewal", description: "Policy Renewal");
            builder.Start("start").Done().Step("s_1").Vars().Name("policyNumber").OnEntry().Done().Done().End("e_1").Done()
                .Link().From("start").To("s_1").Done()
                .Link().From("s_1").To("e_1").Done();
            IReadOnlyList<ProcessValidationResult> validationResults;
            Assert.IsFalse(builder.TryValidate(out validationResults));
            Assert.IsNotNull(validationResults);
            Assert.AreEqual(1,validationResults.Count);

        }

    }
}