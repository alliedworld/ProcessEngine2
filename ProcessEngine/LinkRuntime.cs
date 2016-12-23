using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KlaudWerk.ProcessEngine.Definition;
using KlaudWerk.ProcessEngine.Runtime;

namespace KlaudWerk.ProcessEngine
{
    /// <summary>
    /// Link Runtime class
    /// </summary>
    public class LinkRuntime : ICompilable
    {
        private readonly LinkDefinition _ld;
        private CsScriptRuntime _script;
        public bool IsCompiled { get; private set; }
        /// <summary>
        /// Id of the source step
        /// </summary>
        public Guid SourceStepId => _ld.Source.Id;
        /// <summary>
        /// Id of the target step
        /// </summary>
        public Guid TargetStepId => _ld.Target.Id;
        /// <summary>
        /// Link Definition
        /// </summary>
        public LinkDefinition Definition => _ld;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ld">Link definition<see cref="LinkDefinition"/></param>
        public LinkRuntime(LinkDefinition ld)
        {
            _ld = ld;
        }
        /// <summary>
        /// Try to compile the script
        /// </summary>
        /// <param name="errors"></param>
        /// <returns></returns>
        public bool TryCompile(out string[] errors)
        {
            List<string> aggregatingErrors=new List<string>();
            if (IsCompiled)
            {
                errors = aggregatingErrors.ToArray();
                return true;
            }
            if (_ld.Script != null)
            {
                string[] onEntryErrors;
                _script = new CsScriptRuntime(_ld.Script);
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
            int val = await _script.Execute(env);
            return val == 1;
        }

    }
}