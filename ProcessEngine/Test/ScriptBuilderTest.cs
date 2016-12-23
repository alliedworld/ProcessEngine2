using System;
using KlaudWerk.ProcessEngine.Builder;
using NUnit.Framework;

namespace KlaudWerk.ProcessEngine.Test
{
    [TestFixture]
    public class ScriptBuilderTest
    {
        [Test]
        public void TestCreateWithNullParent()
        {
            Assert.Throws<ArgumentNullException>(() => {
                new ScriptBuilder<object>(null);
            });
        }
        [Test]
        public void TestSetProperties()
        {
            var builder = new StepBuilder(parent:new ProcessBuilder(id:"p_1"),id: "123", name: "Start", description: "Start Step");
            var scriptBuilder = new ScriptBuilder<object>(builder);
            scriptBuilder.Body("Text");
            scriptBuilder.Language(ScriptLanguage.CSharpScript);
            Assert.AreEqual("Text", scriptBuilder.ScriptBody);
            Assert.AreEqual(ScriptLanguage.CSharpScript, scriptBuilder.ScriptLanguage);
        }

        [Test]
        public void TestResetProperties()
        {
            var builder = new StepBuilder(parent:new ProcessBuilder(id:"p_1"),id: "123", name: "Start", description: "Start Step");
            var scriptBuilder = new ScriptBuilder<object>(builder);
            scriptBuilder.Body("Text");
            scriptBuilder.Language(ScriptLanguage.CSharpScript);
            Assert.AreEqual("Text", scriptBuilder.ScriptBody);
            Assert.AreEqual(ScriptLanguage.CSharpScript, scriptBuilder.ScriptLanguage);
            scriptBuilder.Reset();
            Assert.AreEqual(string.Empty, scriptBuilder.ScriptBody);
            Assert.AreEqual(ScriptLanguage.None, scriptBuilder.ScriptLanguage);

        }
        [Test]
        public void TestSetImports()
        {
            var builder = new StepBuilder(parent:new ProcessBuilder(id:"p_1"),id: "123", name: "Start", description: "Start Step");
            var scriptBuilder = new ScriptBuilder<object>(builder);
            scriptBuilder.AddImportds("System.Net", "Newton.Json");
            Assert.IsNotNull(scriptBuilder.Imports);
            Assert.AreEqual(2, scriptBuilder.Imports.Count);
            scriptBuilder.AddImportds("System.Linq");
            Assert.AreEqual(3, scriptBuilder.Imports.Count);
        }

        [Test]
        public void TestSetReferences()
        {
            var builder = new StepBuilder(parent:new ProcessBuilder(id:"p_1"),id: "123", name: "Start", description: "Start Step");
            var scriptBuilder = new ScriptBuilder<object>(builder);
            scriptBuilder.AddReferences("AssemblyOne", "AssemblyTwo");
            Assert.IsNotNull(scriptBuilder.References);
            Assert.AreEqual(2, scriptBuilder.References.Count);
            scriptBuilder.AddReferences("AssemblyThree");
            Assert.AreEqual(3, scriptBuilder.References.Count);
        }

        [Test]
        public void TestResetImports()
        {
            var builder = new StepBuilder(parent:new ProcessBuilder(id:"p_1"),id: "123", name: "Start", description: "Start Step");
            var scriptBuilder = new ScriptBuilder<object>(builder);
            scriptBuilder.AddImportds("System.Net", "Newton.Json");
            Assert.IsNotNull(scriptBuilder.Imports);
            Assert.AreEqual(2, scriptBuilder.Imports.Count);
            scriptBuilder.Reset();
            Assert.AreEqual(0, scriptBuilder.Imports.Count);
        }

        [Test]
        public void TestResetReferences()
        {
            var builder = new StepBuilder(parent:new ProcessBuilder(id:"p_1"),id: "123", name: "Start", description: "Start Step");
            var scriptBuilder = new ScriptBuilder<object>(builder);
            scriptBuilder.AddReferences("AssemblyOne", "AssemblyTwo");
            Assert.IsNotNull(scriptBuilder.References);
            Assert.AreEqual(2, scriptBuilder.References.Count);
            scriptBuilder.Reset();
            Assert.AreEqual(0, scriptBuilder.References.Count);
        }
        [Test]
        public void TestUniqueReferences()
        {
            var builder = new StepBuilder(parent:new ProcessBuilder(id:"p_1"),id: "123", name: "Start", description: "Start Step");
            var scriptBuilder = new ScriptBuilder<object>(builder);
            scriptBuilder.AddReferences("AssemblyOne", "AssemblyTwo");
            Assert.IsNotNull(scriptBuilder.References);
            Assert.AreEqual(2, scriptBuilder.References.Count);
            scriptBuilder.AddReferences("AssemblyOne", "AssemblyTwo");
            Assert.AreEqual(2, scriptBuilder.References.Count);
        }
        [Test]
        public void TestUniqueImports()
        {
            var builder = new StepBuilder(parent:new ProcessBuilder(id:"p_1"),id: "123", name: "Start", description: "Start Step");
            var scriptBuilder = new ScriptBuilder<object>(builder);
            scriptBuilder.AddImportds("System.Net", "Newton.Json");
            Assert.IsNotNull(scriptBuilder.Imports);
            Assert.AreEqual(2, scriptBuilder.Imports.Count);
            scriptBuilder.AddImportds("System.Net", "Newton.Json");
            Assert.AreEqual(2, scriptBuilder.Imports.Count);
        }

    }
}
