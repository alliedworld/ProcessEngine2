using System;
using Klaudwerk.PropertySet;

namespace KlaudWerk.ProcessEngine.Runtime
{
    /// <summary>
    /// Memento for Process Runtime
    /// </summary>
    public class ProcessRuntimeMemento
    {
        public Guid Id { get; }
        public Guid PorcessDefinitionId { get; }
        public int ProcessDefinitionVersion { get; }
        public IPropertySetCollection Variables { get; }
        public Guid StepId { get; }
        public ProcessStateEnum Status { get; }

        public ProcessRuntimeMemento(Guid id, Guid porcessDefinitionId,
            int processDefinitionVersion,
            IPropertySetCollection variables, Guid stepId, ProcessStateEnum status)
        {
            Id = id;
            PorcessDefinitionId = porcessDefinitionId;
            ProcessDefinitionVersion = processDefinitionVersion;
            Variables = variables;
            StepId = stepId;
            Status = status;
        }
    }
}