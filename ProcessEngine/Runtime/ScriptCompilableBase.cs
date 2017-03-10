using System.Collections.Generic;
using System.Threading.Tasks;
using KlaudWerk.ProcessEngine.Definition;

namespace KlaudWerk.ProcessEngine.Runtime
{
    /// <summary>
    /// Base class for Compilable scripts
    /// </summary>
    public abstract class ScriptCompilableBase : ICompilable
    {
        protected CsScriptRuntime _script;
        public bool IsCompiled { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sd"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        protected bool TryCompileScript(ScriptDefinition sd,out string[] errors)
        {
            List<string> aggregatingErrors=new List<string>();
            if (IsCompiled)
            {
                errors = aggregatingErrors.ToArray();
                return true;
            }
            if (sd != null)
            {
                string[] onEntryErrors;
                _script = new CsScriptRuntime(sd);
                _script.TryCompile(out onEntryErrors);
                aggregatingErrors.AddRange(onEntryErrors);
            }
            errors = aggregatingErrors.ToArray();
            IsCompiled = true;
            return errors.Length == 0;

        }

        /// <summary>
        /// Evaluate the link
        /// </summary>
        /// <param name="env"></param>
        /// <returns>evaluation result true if a link condition has been satisfied. </returns>
        public async Task<bool> Evaluate(IProcessRuntimeEnvironment env)
        {
            if (_script == null)
                return await Task.FromResult(true);

            int val = await _script.Execute(env);
            return val == 1;
        }

        public abstract bool TryCompile(out string[] errors);
    }
}