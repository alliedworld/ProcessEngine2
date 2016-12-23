using KlaudWerk.ProcessEngine.Builder;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using NUnit.Framework;

namespace KlaudWerk.ProcessEngine.Test
{
    public class StepEnvironment
    {

    }

    [TestFixture]
    public class ScriptTest
    {
        [Test]
        public void TestCreateScriptEngine()
        {
            int result =  CSharpScript.EvaluateAsync<int>("1+2").Result;
            Assert.AreEqual(3,result);
        }

        [Test]
        public void TestCreateExecuteScript()
        {
            var builder = new ScriptBuilder<StepBuilder>(new StepBuilder(new ProcessBuilder("p1"), "s_01"));
            builder.Language(ScriptLanguage.CSharpScript).Body("1+1");
            var script = CSharpScript.Create(builder.ScriptBody, globalsType: typeof(StepEnvironment));
            var diagnostic=script.Compile();
            Assert.IsNotNull(diagnostic);
            Assert.AreEqual(0, diagnostic.Length);
            var state = script.RunAsync(new StepEnvironment()).Result;
            Assert.IsNotNull(state);
            Assert.AreEqual(2, state.ReturnValue);
        }

        [Test]
        public void TestCreateCompiledScriptError()
        {
            var builder = new ScriptBuilder<StepBuilder>(new StepBuilder(new ProcessBuilder("p1"), "s_01"));
            builder.Language(ScriptLanguage.CSharpScript).Body("1=abracadabra");
            var script = CSharpScript.Create(builder.ScriptBody, globalsType: typeof(StepEnvironment));
            var diagnostic=script.Compile();
            Assert.Greater(diagnostic.Length,0);

        }
    }
}
