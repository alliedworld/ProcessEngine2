using System;
using System.Collections.Generic;
using System.Linq;
using KlaudWerk.ProcessEngine.Builder;
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
            builder.Link().From("s_1").To("e_1");
            Assert.AreEqual(2, builder.Steps.Count);
            Assert.AreEqual(1, builder.Links.Count);
            builder.Steps[0].Item1.Remove();
            Assert.AreEqual(1, builder.Steps.Count);
            Assert.AreEqual(0, builder.Links.Count);
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