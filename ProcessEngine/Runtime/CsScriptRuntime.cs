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