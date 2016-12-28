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