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