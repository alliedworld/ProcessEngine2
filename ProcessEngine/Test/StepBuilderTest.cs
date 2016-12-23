using System;
using KlaudWerk.ProcessEngine.Builder;
using NUnit.Framework;

namespace KlaudWerk.ProcessEngine.Test
{
    [TestFixture]
    public class StepBuilderTest
    {
        [Test]
        public void TestCreateStepBuilder()
        {
            var builder = new StepBuilder(parent:new ProcessBuilder(id:"p_1"),  id: "123", name: "Start", description: "Start Step");
            Assert.AreEqual("123", builder.Id);
            Assert.AreEqual("Start", builder.Name);
            Assert.AreEqual("Start Step", builder.Description);
        }

        [Test]
        public void EmptyIdShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => {
                new StepBuilder(new ProcessBuilder(id:"p_1"),string.Empty);
            });
        }
        [Test]
        public void NoValueStringIdShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => {
                new StepBuilder(new ProcessBuilder(id:"p_1"),"    ");
            });
        }

        [Test]
        public void NullIdShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentNullException>(() => {
                new StepBuilder(new ProcessBuilder(id:"p_1"),null);
            });

        }

        [Test]
        public void NameCanBeChanged()
        {
            var builder = new StepBuilder(new ProcessBuilder(id:"p_1"),id: "123", name: "Start", description: "Start Step");
            Assert.AreEqual("123", builder.Id);
            Assert.AreEqual("Start", builder.Name);
            var ret = builder.SetName("New Name");
            Assert.AreEqual(builder, ret);
            Assert.AreEqual("New Name", builder.Name);

        }
        [Test]
        public void DescriptionCanBeChanged()
        {
            var builder = new StepBuilder(new ProcessBuilder(id:"p_1"),id: "123", name: "Start", description: "Start Step");
            Assert.AreEqual("123", builder.Id);
            Assert.AreEqual("Start Step", builder.Description);
            var ret = builder.SetDescription("New Description");
            Assert.AreEqual(builder, ret);
            Assert.AreEqual("New Description", builder.Description);
        }

        [Test]
        public void StepBuilderAllowsToDefineStepVariables()
        {
            var builder = new StepBuilder(new ProcessBuilder(id:"p_1"),id: "123", name: "Start", description: "Start Step");
            builder.Vars().Name("abc").OnEntry().Done();

        }

        [Test]
        public void TestOnEnterScriptSetup()
        {
            var builder = new StepBuilder(new ProcessBuilder(id:"p_1"),id: "123", name: "Start", description: "Start Step");
            var sb = builder.OnEntry().Language(ScriptLanguage.CSharpScript).Body(@"Text");
            Assert.IsNotNull(sb);
            Assert.AreEqual("Text", sb.ScriptBody);
            Assert.AreEqual(ScriptLanguage.CSharpScript, sb.ScriptLanguage);
            Assert.AreEqual(sb, builder.OnEntry());
            Assert.AreEqual(builder, sb.Done());
        }
        [Test]
        public void TestOnExitScriptSetup()
        {
            var builder = new StepBuilder(new ProcessBuilder(id:"p_1"),id: "123", name: "Start", description: "Start Step");
            var sb = builder.OnExit().Language(ScriptLanguage.CSharpScript).Body(@"Text");
            Assert.IsNotNull(sb);
            Assert.AreEqual("Text", sb.ScriptBody);
            Assert.AreEqual(ScriptLanguage.CSharpScript, sb.ScriptLanguage);
            Assert.AreEqual(sb, builder.OnExit());
            Assert.AreEqual(builder, sb.Done());
            Assert.AreNotEqual(builder.OnEntry(), builder.OnExit());
        }

        [Test]
        public void TestResetOnEntryScriptSetup()
        {
            var builder = new StepBuilder(new ProcessBuilder(id:"p_1"),id: "123", name: "Start", description: "Start Step");
            var sb = builder.OnEntry().Language(ScriptLanguage.CSharpScript).Body(@"Text");
            Assert.IsNotNull(sb);
            Assert.AreEqual("Text", sb.ScriptBody);
            Assert.AreEqual(ScriptLanguage.CSharpScript, sb.ScriptLanguage);
            builder.OnEntry().Reset();
            Assert.AreEqual(string.Empty, sb.ScriptBody);
            Assert.AreEqual(ScriptLanguage.None, sb.ScriptLanguage);
        }
        [Test]
        public void TestResetOnExitScriptSetup()
        {
            var builder = new StepBuilder(new ProcessBuilder(id:"p_1"),id: "123", name: "Start", description: "Start Step");
            var sb = builder.OnExit().Language(ScriptLanguage.CSharpScript).Body(@"Text");
            Assert.IsNotNull(sb);
            Assert.AreEqual("Text", sb.ScriptBody);
            Assert.AreEqual(ScriptLanguage.CSharpScript, sb.ScriptLanguage);
            builder.OnExit().Reset();
            Assert.AreEqual(string.Empty, sb.ScriptBody);
            Assert.AreEqual(ScriptLanguage.None, sb.ScriptLanguage);
        }

        [Test]
        public void TestGetSecurityBuilder()
        {
            var builder = new StepBuilder(new ProcessBuilder(id:"p_1"),id: "123", name: "Start", description: "Start Step");
            var sb=builder.Security();
            Assert.IsNotNull(sb);
        }

        [Test]
        public void FinalizingActionAdditionShouldCreateActionInStep()
        {
            var builder = new StepBuilder(new ProcessBuilder(id: "p_1"), id: "123", name: "Start", description: "Start Step");
            builder.Action().Name("a1").Description("Provide Documentation").Skippable(false).Done();
            Assert.IsNotNull(builder.Actions);
            Assert.AreEqual(1,builder.Actions.Count);
            Assert.AreEqual("a1", builder.Actions[0].ActionName);
            Assert.AreEqual("Provide Documentation", builder.Actions[0].ActionDescription);
            Assert.IsFalse(builder.Actions[0].IsSkippable);
        }
        [Test]
        public void FinalizingActionWithTheSameNameShouldReplaceAction()
        {
            var builder = new StepBuilder(new ProcessBuilder(id: "p_1"), id: "123", name: "Start", description: "Start Step");
            builder.Action().Name("a1").Description("Provide Documentation").Skippable(false).Done();
            Assert.IsNotNull(builder.Actions);
            Assert.AreEqual(1, builder.Actions.Count);
            Assert.AreEqual("a1", builder.Actions[0].ActionName);
            Assert.AreEqual("Provide Documentation", builder.Actions[0].ActionDescription);
            Assert.IsFalse(builder.Actions[0].IsSkippable);
            builder.Action().Name("a1").Description("New Documentation").Skippable(true).Done();
            Assert.AreEqual("New Documentation", builder.Actions[0].ActionDescription);
            Assert.IsTrue(builder.Actions[0].IsSkippable);
        }
        [Test]
        public void FinalizeActionWithoutNameSouldThrowException()
        {
            Assert.Throws<ArgumentNullException>(()=> {
                var builder = new StepBuilder(new ProcessBuilder(id: "p_1"), id: "123", name: "Start", description: "Start Step");
                builder.Action().Description("Provide Documentation").Skippable(false).Done();
            });
        }
        [Test]
        public void RemoveActionShouldCallRemoveOnPArent()
        {
            var builder = new StepBuilder(new ProcessBuilder(id: "p_1"), id: "123", name: "Start", description: "Start Step");
            builder.Action().Name("a1").Description("Provide Documentation").Skippable(false).Done();
            Assert.IsNotNull(builder.Actions);
            Assert.AreEqual(1, builder.Actions.Count);
            builder.Actions[0].Remove();
            Assert.IsNotNull(builder.Actions);
            Assert.AreEqual(0, builder.Actions.Count);
        }

        [Test]
        public void StepBuilderShouldContainEmptyDeefaultHandler()
        {
            var builder = new StepBuilder(new ProcessBuilder(id: "p_1"), id: "123", name: "Start", description: "Start Step");
            Assert.IsNotNull(builder.StepHandler);
            Assert.AreEqual(StepHandlerTypeEnum.None,builder.StepHandler.StepHandlerType);
        }

        [Test]
        public void HumanTaskShouldPoulateTaskStepHandler()
        {
            var builder = new StepBuilder(new ProcessBuilder(id: "p_1"), id: "123", name: "Start", description: "Start Step");
            builder.Handler().HumanTask().Done();
            Assert.IsNotNull(builder.StepHandler);
            Assert.AreEqual(StepHandlerTypeEnum.Task,builder.StepHandler.StepHandlerType);

        }

        [Test]
        public void ScriptStepHandlerShouldCreateScriptBuilder()
        {
            var builder = new StepBuilder(new ProcessBuilder(id: "p_1"), id: "123", name: "Start", description: "Start Step");
            builder.Handler().Script().Language(ScriptLanguage.CSharpScript).Body("1+1").Done().Done();
            Assert.IsNotNull(builder.StepHandler);
            Assert.AreEqual(StepHandlerTypeEnum.Script,builder.StepHandler.StepHandlerType);
            Assert.IsNotNull(builder.Handler().ScriptBuilder);
            Assert.AreEqual(ScriptLanguage.CSharpScript, builder.Handler().ScriptBuilder.ScriptLanguage);
            Assert.AreEqual("1+1",builder.Handler().ScriptBuilder.ScriptBody);
        }

        [Test]
        public void IocHandlerShoutSetIocName()
        {
            var builder = new StepBuilder(new ProcessBuilder(id: "p_1"), id: "123", name: "Start", description: "Start Step");
            builder.Handler().IocService("SendEmail");
            Assert.IsNotNull(builder.StepHandler);
            Assert.AreEqual(StepHandlerTypeEnum.IoC,builder.StepHandler.StepHandlerType);
            Assert.AreEqual("SendEmail",builder.StepHandler.IocName);
        }
    }
}
