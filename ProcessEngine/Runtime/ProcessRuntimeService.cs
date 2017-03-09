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
using System.Linq;
using Klaudwerk.PropertySet;
using KlaudWerk.ProcessEngine.Definition;

namespace KlaudWerk.ProcessEngine.Runtime
{
    public class ProcessRuntimeService : IProcessRuntimeService
    {
        /// <summary>
        /// Instantiate new process runtime
        /// </summary>
        /// <param name="pd"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        public virtual IProcessRuntime Create(ProcessDefinition pd,IPropertySetCollection collection)
        {
            IEnumerable<LinkRuntime> linkRuntimes = pd.Links.Select(ld => new LinkRuntime(ld));
            IEnumerable<StepRuntime> stepRuntimes = pd.Steps.Select(sd => new StepRuntime(sd,linkRuntimes.Where(l=>l.SourceStepId==sd.Id).ToArray()));
            
            foreach (VariableDefinition variableDefinition in pd.Variables)
                variableDefinition.SetupVariable(collection);
            return new ProcessRuntime(Guid.NewGuid(), linkRuntimes,stepRuntimes);
        }

        /// <summary>
        /// Instantiate a process with associated step and state
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pd"></param>
        /// <param name="step"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        protected virtual ProcessRuntime Create(Guid id, ProcessDefinition pd, string step, ProcessStateEnum status)
        {
            IEnumerable<LinkRuntime> linkRuntimes = pd.Links.Select(ld => new LinkRuntime(ld)).ToList();
            IEnumerable<StepRuntime> stepRuntimes = pd.Steps.Select(sd => new StepRuntime(sd, linkRuntimes.Where(l => l.SourceStepId == sd.Id).ToArray()));
            StepDefinition stepDef = pd.Steps.SingleOrDefault(s => s.StepId == step);
            StepRuntime suspended = stepDef == null ? null : new StepRuntime(stepDef, linkRuntimes.Where(l => l.SourceStepId == stepDef.Id).ToArray());
            return new ProcessRuntime(id, linkRuntimes,stepRuntimes,
                suspended,status);

        }
        /// <summary>
        /// Freeze thw process
        /// </summary>
        /// <param name="runtime"></param>
        /// <param name="collection"></param>
        public virtual void Freeze(IProcessRuntime runtime, IPropertySetCollection collection)
        {

        }

        /// <summary>
        /// Unfreeze the process
        /// </summary>
        /// <param name="processRuntimeId"></param>
        /// <param name="runtime"></param>
        /// <param name="nextStep"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        public virtual bool TryUnfreeze(Guid processRuntimeId, out IProcessRuntime runtime, out StepRuntime nextStep, out IPropertySetCollection collection)
        {
            throw new NotImplementedException();
        }

        public virtual bool Cancel(Guid processRuntimeId, string reason)
        {
            throw new NotImplementedException();
        }
    }
}