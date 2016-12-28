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
