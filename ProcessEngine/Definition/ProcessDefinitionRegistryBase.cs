using System;
using System.Collections.Generic;
using System.Linq;
using KlaudWerk.ProcessEngine.Builder;

namespace KlaudWerk.ProcessEngine.Definition
{
    /// <summary>
    /// Base abstract registry class that provide single process definition
    /// </summary>
    public abstract class ProcessDefinitionRegistryBase:IProcessDefinitionRegistry
    {
        protected abstract Guid Id { get; }
        protected abstract ProcessBuilder ProcessBuilder { get; }
        private ProcessDefinition _definition;
        private ProcessDescriptor[] _descriptors;
        public IEnumerable<ProcessDescriptor> Processes
        {
            get
            {
                if (_descriptors == null)
                {
                    ProcessDefinition pd = _definition;
                    Md5CalcVisitor visitor=new Md5CalcVisitor();
                    pd.Accept(visitor);
                    _descriptors=new []
                    {
                        new ProcessDescriptor(pd.Id,pd.FlowId,pd.Name,pd.Description,visitor.CalculateMd5()),
                    };
                }
                return _descriptors;
            }
        }

        /// <summary>
        /// Try to get a process definition by Id
        /// </summary>
        /// <param name="processDefinitionId"></param>
        /// <param name="pd"></param>
        /// <returns></returns>
        public bool TryGet(Guid processDefinitionId, out ProcessDefinition pd)
        {

            if (_definition == null)
            {
                pd = null;
                return false;
            }
            pd = (processDefinitionId == _definition.Id) ? _definition : null;
            return pd != null;
        }

        /// <summary>
        /// Tries to initialize the definition
        /// </summary>
        /// <param name="errors"></param>
        /// <returns></returns>
        public bool TryInitialize(out string[] errors)
        {
            errors = new string[] {};
            IReadOnlyList<ProcessValidationResult> validationResult;
            if(!ProcessBuilder.TryValidate(out validationResult))
            {
                errors=validationResult.Select(r => $"{r.Artifact}-({r.ArtifactId}):{r.Message}").ToArray();
                return false;
            }
            _definition = ProcessBuilder.Build();
            _definition.Id = Id;
            return true;
        }
    }
}