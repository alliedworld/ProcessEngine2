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
            IEnumerable<StepRuntime> stepRuntimes = pd.Steps.Select(sd => new StepRuntime(sd));
            IEnumerable<LinkRuntime> linkRuntimes = pd.Links.Select(ld => new LinkRuntime(ld));
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
            IEnumerable<StepRuntime> stepRuntimes = pd.Steps.Select(sd => new StepRuntime(sd));
            IEnumerable<LinkRuntime> linkRuntimes = pd.Links.Select(ld => new LinkRuntime(ld));
            StepDefinition stepDef = pd.Steps.SingleOrDefault(s => s.StepId == step);
            StepRuntime suspended = stepDef == null ? null : new StepRuntime(stepDef);
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

    }
}