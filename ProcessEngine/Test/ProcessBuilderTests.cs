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
using KlaudWerk.ProcessEngine.Builder;
using KlaudWerk.ProcessEngine.Definition;
using NUnit.Framework;

namespace KlaudWerk.ProcessEngine.Test
{
    [TestFixture]
    public class ProcessBuilderTests
    {

        [Test]
        public void TestBuildProcess()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "com.klaudwerk.workflow.renewal", name: "Renewal", description: "Policy Renewal");
            Assert.IsNotNull(builder);
        }

        [Test]
        public void EmptyIdShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var factory = new ProcessBuilderFactory();
                var builder = factory.CreateProcess(id: string.Empty, name: "Renewal", description: "Policy Renewal");
            });
        }

        [Test]
        public void NullIdShouldThrowException()
        {
            Assert.Throws<ArgumentException>(()=>
            {
                var factory = new ProcessBuilderFactory();
                var builder = factory.CreateProcess(id: string.Empty, name: "Renewal", description: "Policy Renewal");
            });
        }

        [Test]
        public void BuildWorkflowWithStartAndEndStep()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "p_001", name: "Renewal", description: "Policy Renewal");
            var sb = builder.Start("123");
            var doneBuilder=sb.Done();
            Assert.AreEqual(builder,doneBuilder);
            builder.End("End_1");
            Assert.IsNotNull(sb);
            Assert.IsNotNull(builder.StartSteps);
            Assert.AreEqual(1,builder.StartSteps.Count);
            Assert.IsNotNull(builder.EndSteps);
            Assert.AreEqual(1,builder.EndSteps.Count);
            Assert.IsNotNull(builder.Steps);
            Assert.AreEqual(2,builder.Steps.Count);
        }
        [Test]
        public void AddStepsWithTheSameIdShouldFail()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "p_001", name: "Renewal", description: "Policy Renewal");
            var sb = builder.Step("123");

            Assert.Throws<ArgumentException>(() =>
            {
                builder.Step("123");
            });
            Assert.Throws<ArgumentException>(() =>
            {
                builder.Start("123");
            });
            Assert.Throws<ArgumentException>(() =>
            {
                builder.End("123");
            });


        }
        [Test]
        public void AddStartStepsWithTheSameIdShouldFail()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "p_001", name: "Renewal", description: "Policy Renewal");
            var sb = builder.Start("123");

            Assert.Throws<ArgumentException>(() =>
            {
                builder.Step("123");
            });
            Assert.Throws<ArgumentException>(() =>
            {
                builder.Start("123");
            });
            Assert.Throws<ArgumentException>(() =>
            {
                builder.End("123");
            });

        }
        [Test]
        public void AddEndWithTheSameIdShouldFail()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "p_001", name: "Renewal", description: "Policy Renewal");
            var sb = builder.End("123");

            Assert.Throws<ArgumentException>(() =>
            {
                builder.Step("123");
            });
            Assert.Throws<ArgumentException>(() =>
            {
                builder.Start("123");
            });
            Assert.Throws<ArgumentException>(() =>
            {
                builder.End("123");
            });

        }

        [Test]
        public void TryFindStepShouldReturnExistingStep()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "p_001", name: "Renewal", description: "Policy Renewal");
            var sb = builder.Start("123");
            Tuple<StepBuilder, StepTypeEnum> step;
            Assert.IsTrue(builder.TryFindStep("123",out step));
            Assert.IsNotNull(step);
            Assert.AreEqual("123",step.Item1.Id);
            Assert.AreEqual(StepTypeEnum.Start,step.Item2);
        }

        [Test]
        public void TryFindTestShouldReturnFalseForNonExistingStep()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "p_001", name: "Renewal", description: "Policy Renewal");
            var sb = builder.Start("123");
            Tuple<StepBuilder, StepTypeEnum> step;
            Assert.IsFalse(builder.TryFindStep("567",out step));
            Assert.IsNull(step);
        }
        [Test]
        public void TestLinkStepsInWorkflow()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "p_001", name: "Renewal", description: "Policy Renewal");
            var pb=builder.Start("s_1").Done().End("e_1").Done()
                .Link().From("s_1").To("e_1").Name("Approved").Done();
            Assert.IsNotNull(pb);
        }

        [Test]
        public void LinkToTheSameStepShouldThrowException()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "p_001", name: "Renewal", description: "Policy Renewal");
            builder.Start("s_1").Done().End("e_1");
            builder.Link().From("s_1").To("e_1").Done();
            Assert.Throws<ArgumentException>(() =>
            {
                builder.Link().From("s_1").To("e_1").Done();
            });

        }

        [Test]
        public void LinkToSelfShouldThrowException()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "p_001", name: "Renewal", description: "Policy Renewal");
            builder.Start("s_1").Done().End("e_1");
            builder.Link().From("s_1").To("e_1").Done();
            Assert.Throws<ArgumentException>(() =>
            {
                builder.Link().From("s_1").To("s_1").Done();
            });

        }

        [Test]
        public void EndStepShouldNotAllowOutgoingLinks()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "p_001", name: "Renewal", description: "Policy Renewal");
            builder.Start("s_1").Done().End("e_1");
            Assert.Throws<ArgumentException>(() =>
            {
                builder.Link().From("e_1").To("s_1").Done();
            });
        }

        [Test]
        public void LinkFromNonExistingStepShouldThrowException()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "p_001", name: "Renewal", description: "Policy Renewal");
            builder.Start("s_1").Done().End("e_1");
            Assert.Throws<ArgumentException>(() =>
            {
                builder.Link().From("s_2").To("s_1");
            });

        }
        [Test]
        public void LinkToNonExistingStepShouldThrowException()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "p_001", name: "Renewal", description: "Policy Renewal");
            builder.Start("s_1").Done().End("e_1");
            Assert.Throws<ArgumentException>(() =>
            {
                builder.Link().From("s_1").To("s_2");
            });
        }

        [Test]
        public void FinalizingLinkWithIncompleteFromStepShouldFail()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "p_001", name: "Renewal", description: "Policy Renewal");
            builder.Start("s_1").Done().End("e_1");
            Assert.Throws<ArgumentException>(() =>
            {
                builder.Link().To("s_1").Done();
            });

        }
        [Test]
        public void FinalizingLinkWithIncompleteTotepShouldFail()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "p_001", name: "Renewal", description: "Policy Renewal");
            builder.Start("s_1").Done().End("e_1");
            Assert.Throws<ArgumentException>(() =>
            {
                builder.Link().From("s_1").Done();
            });
        }
        [Test]
        public void ItShouldBePossibleToGetListoOfOutgoingLinks()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "p_001", name: "Renewal", description: "Policy Renewal");
            builder.Start("s_1").Done().End("e_1");
            builder.Step("s_2").Done();
            builder.Step("s_3").Done();
            builder.Link().From("s_1").To("s_2").Done();
            builder.Link().From("s_1").To("s_3").Done();
            builder.Link().From("s_1").To("e_1").Done();
            IReadOnlyList<LinkBuilder> links = builder.GetLinksFrom("s_1");
            Assert.IsNotNull(links);
            Assert.AreEqual(3,links.Count);
            Assert.AreEqual(1,links.Count(l=>l.StepTo.Id=="s_2" && l.StepFrom.Id=="s_1"));
            Assert.AreEqual(1,links.Count(l=>l.StepTo.Id=="s_3" && l.StepFrom.Id=="s_1"));
            Assert.AreEqual(1,links.Count(l=>l.StepTo.Id=="e_1" && l.StepFrom.Id=="s_1"));
        }

        [Test]
        public void ItShouldBePossibleToGetListOfIncomingLinks()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "p_001", name: "Renewal", description: "Policy Renewal");
            builder.Start("s_1").Done().End("e_1");
            builder.Step("s_2").Done();
            builder.Step("s_3").Done();
            builder.Link().From("s_1").To("s_2").Done();
            builder.Link().From("s_1").To("s_3").Done();
            builder.Link().From("s_1").To("e_1").Done();
            builder.Link().From("s_2").To("e_1").Done();
            builder.Link().From("s_3").To("e_1").Done();
            IReadOnlyList<LinkBuilder> links = builder.GetLinksTo("e_1");
            Assert.IsNotNull(links);
            Assert.AreEqual(3,links.Count);
            Assert.AreEqual(1,links.Count(l=>l.StepFrom.Id=="s_2" && l.StepTo.Id=="e_1"));
            Assert.AreEqual(1,links.Count(l=>l.StepFrom.Id=="s_3" && l.StepTo.Id=="e_1"));
            Assert.AreEqual(1,links.Count(l=>l.StepFrom.Id=="s_1" && l.StepTo.Id=="e_1"));
        }
        [Test]
        public void CallRemoveOnStepShouldRemoveStepAndLinksFromProcessor()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "p_001", name: "Renewal", description: "Policy Renewal");
            builder.Start("s_1").Done().End("e_1");
            builder.Link().From("s_1").To("e_1").Done();
            Assert.AreEqual(2, builder.Steps.Count);
            Assert.AreEqual(1, builder.Links.Count);
            builder.Steps[0].Item1.Remove();
            Assert.AreEqual(1, builder.Steps.Count);
            Assert.AreEqual(0, builder.Links.Count);
        }

        [Test]
        public void CreateProcessWithActionRelationsShouldBeSuccessfull()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "p_001", name: "Renewal", description: "Policy Renewal");
            builder.Start("s_1").Done().End("e_1");
            builder.Step("s_2").Action().Name("a_1").Skippable(false).Done();
            builder.Step("s_3").Action().Name("a_2").Skippable(false).Done();
            builder.Link().From("s_1").To("s_2").Done();
            builder.Link().From("s_1").To("s_3").Done();
            builder.Link().From("s_1").To("e_1").Done();
            builder.Link().From("s_2").To("e_1").Done();
            builder.Link().From("s_3").To("e_1").Done();
            builder.BuildActionRelations().If("a_1").RequiredOnStep("s_2").Then("a_2").RequiredOnStep("s_3").Done().Done();
            IReadOnlyList<ProcessValidationResult> erros    ;
            Assert.IsTrue(builder.TryValidate(out erros));
            Assert.AreEqual(0,erros.Count);
            var processDefinition = builder.Build();
            Assert.IsNotNull(processDefinition);
            Assert.IsNotNull(processDefinition.ActionsRelations);
            Assert.AreEqual(1,processDefinition.ActionsRelations.Length);
            Assert.AreEqual("s_2",processDefinition.ActionsRelations[0].SourceStepId);
            Assert.AreEqual("s_3",processDefinition.ActionsRelations[0].TargetStepId);
            Assert.AreEqual("a_1",processDefinition.ActionsRelations[0].SourceActionId);
            Assert.AreEqual("a_2",processDefinition.ActionsRelations[0].TargetActionId);
        }

        [Test]
        public void CreateProcessWithTagsShouldBuildProcessDefinitionWithTags()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "p_001", name: "Renewal", description: "Policy Renewal");
            builder.Start("s_1").Done().End("e_1");
            builder.Step("s_2");
            builder.Step("s_3");
            builder.Link().From("s_1").To("s_2").Done();
            builder.Link().From("s_1").To("s_3").Done();
            builder.Link().From("s_1").To("e_1").Done();
            builder.Link().From("s_2").To("e_1").Done();
            builder.Link().From("s_3").To("e_1").Done();
            builder.Tag("LOB").Handler().IocService("lobService").Done().Name("Line of Business");
            IReadOnlyList<ProcessValidationResult> erros    ;
            Assert.IsTrue(builder.TryValidate(out erros));
            Assert.AreEqual(0,erros.Count);
            var processDefinition = builder.Build();
            Assert.IsNotNull(processDefinition);
            Assert.IsNotNull(processDefinition.Tags);
            Assert.AreEqual(1,processDefinition.Tags.Count());
            TagDefinition td = processDefinition.Tags[0];
            Assert.AreEqual("LOB",td.Id);
            Assert.AreEqual("Line of Business",td.DisplayName);
            Assert.IsNotNull(td.Handler);
            Assert.AreEqual("lobService",td.Handler.IocName);
        }

    }

    public class ProcessBuilderFactory
    {
        public ProcessBuilderFactory()
        {
        }

        public ProcessBuilder CreateProcess(string id,string name, string description)
        {
            id.NotNull("id").NotEmptyString("id");
            return new ProcessBuilder(id,name,description);
        }
    }
}