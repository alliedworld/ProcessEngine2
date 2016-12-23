using System;
using Klaudwerk.PropertySet;
using KlaudWerk.ProcessEngine.Definition;

namespace KlaudWerk.ProcessEngine.Runtime
{
    public interface IProcessRuntimeService
    {
        IProcessRuntime Create(ProcessDefinition pd,IPropertySetCollection collection);

        /// <summary>
        /// Save or update the workflow
        /// </summary>
        /// <param name="runtime"></param>
        void Freeze(IProcessRuntime runtime, IPropertySetCollection collection);

        /// <summary>
        /// Try "unfreeze" the process
        /// </summary>
        /// <param name="processRuntimeId"></param>
        /// <param name="runtime"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        bool TryUnfreeze(Guid processRuntimeId, out IProcessRuntime runtime, out StepRuntime nextStep, out IPropertySetCollection collection);
    }
}