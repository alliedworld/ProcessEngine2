using System;
using System.Linq;
using System.Threading.Tasks;
using KlaudWerk.ProcessEngine.Definition;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace KlaudWerk.ProcessEngine.Runtime
{
    /// <summary>
    /// Process CS Script runtime
    /// </summary>
    public class CsScriptRuntime
    {
        private readonly ScriptDefinition _sd;
        private Script<int> _script;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sd">The instance of <see cref="ScriptDefinition"/></param>
        public CsScriptRuntime(ScriptDefinition sd)
        {
            _sd = sd;
        }

        /// <summary>
        /// Try to compile the script
        /// </summary>
        /// <param name="errors"></param>
        /// <returns></returns>
        public bool TryCompile(out string[] errors)
        {
            errors=new string[]{};
            if (_sd.Lang == ScriptLanguage.None)
                return true;
            ScriptOptions scriptOptions=ScriptOptions.Default;
            if (_sd.Imports != null && _sd.Imports.Length > 0)
                scriptOptions = scriptOptions.WithImports(_sd.Imports);
            if(_sd.References!=null && _sd.References.Length>0)
                scriptOptions = scriptOptions.WithReferences(_sd.References);
            _script=CSharpScript.Create<int>(code: _sd.Script, options: scriptOptions,
                globalsType: typeof(IProcessRuntimeEnvironment));
            var diags = _script.Compile();
            if (diags != null && diags.Length > 0)
                errors = diags.Select(d => d.ToString()).ToArray();
            return errors.Length == 0;
        }
        /// <summary>
        /// Execute script Async
        /// </summary>
        /// <param name="env"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<int> Execute(IProcessRuntimeEnvironment env)
        {
            if (_sd.Lang == ScriptLanguage.None)
                return 1;
            if(_script==null)
                throw new ArgumentException("Script is not initialized.");
            ScriptState<int> result = await _script.RunAsync(env);
            if (result.Exception != null)
                throw result.Exception;
            return result.ReturnValue;
        }
    }
}