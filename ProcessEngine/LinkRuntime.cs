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