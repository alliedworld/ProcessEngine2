using System;
using System.Collections.Generic;

namespace KlaudWerk.ProcessEngine.Definition
{
    /// <summary>
    /// Registry of process definitions
    /// </summary>
    public interface IProcessDefinitionRegistry
    {
        /// <summary>
        /// List of the Porcess Definitions in this registry
        /// </summary>
        IEnumerable<ProcessDescriptor> Processes { get; }
        /// <summary>
        /// Get the process definition by Process Guid
        /// </summary>
        /// <param name="processDefinitionId"></param>
        /// <returns></returns>
        bool TryGet(Guid processDefinitionId, out  ProcessDefinition pd);
        /// <summary>
        /// Try to initialize the registry
        /// </summary>
        /// <param name="errors"></param>
        /// <returns></returns>
        bool TryInitialize(out string[] errors);
    }
}