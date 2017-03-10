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
using KlaudWerk.ProcessEngine.Definition;

namespace KlaudWerk.ProcessEngine.Runtime
{
    /// <summary>
    /// Link Runtime class
    /// </summary>
    public class LinkRuntime : ScriptCompilableBase
    {
        private readonly LinkDefinition _ld;
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
        public override bool TryCompile(out string[] errors)
        {
            return TryCompileScript(_ld.Script, out errors);
        }
    }
}