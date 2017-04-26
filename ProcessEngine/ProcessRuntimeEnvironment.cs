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
using System.Security.Claims;
using System.Threading.Tasks;
using Klaudwerk.PropertySet;
using Klaudwerk.PropertySet.Test;
using KlaudWerk.ProcessEngine.Runtime;

namespace KlaudWerk.ProcessEngine
{
    /// <summary>
    /// Process Runtime Environment
    /// </summary>
    public class ProcessRuntimeEnvironment : IProcessRuntimeEnvironment
    {

        private readonly IPropertySetCollection _params=
            new ValueSetCollectionTest.MockPropertySetCollection(new PropertySchemaSet(new PropertySchemaFactory()));

        private IExecutionService _taskExecution;
        private IIocServiceExecution _iocServiceExecution;
        private IAssemblyServiceExecution _assemblyServiceExecution;
        /// <summary>
        /// Current Step
        /// </summary>
        public StepRuntime CurrentStep { get; set; }

        /// <summary>
        /// Caller claim principal
        /// </summary>
        public ClaimsPrincipal Principal { get; set; }

        /// <summary>
        /// Variable values associated with the workflow
        /// </summary>
        public IPropertySetCollection PropertySet { get; }

        public int ProcessEnvId { get; set; }
        public Guid ProcessId { get; set; }
        public IProcessRuntime ProcessRuntime { get; }
        public string Transition { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessRuntimeEnvironment"/> class.
        /// </summary>
        /// <param name="propertySet">The property set.</param>
        public ProcessRuntimeEnvironment(
            IProcessRuntime runtime,
            IPropertySetCollection propertySet)
        {
            ProcessRuntime = runtime;
            PropertySet = propertySet;
        }


        /// <summary>
        /// Execute the IOC service asynchronous.
        /// </summary>
        /// <param name="iocName">Name of the ioc.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual Task<ExecutionResult> IocServiceAsync(string iocName)
        {
            IExecutionService servce;
            return (_iocServiceExecution == null || (servce = _iocServiceExecution.GetServce(iocName))==null)?
                Task.FromResult(new ExecutionResult(StepExecutionStatusEnum.Failed, null,
                    $"No IOC Service registered for {iocName}")):
                servce.ExecuteAsync(this);
        }

        /// <summary>
        /// Executes the Task service asynchronous.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual Task<ExecutionResult> TaskServiceAsync()
        {
            return _taskExecution==null? Task.FromResult(new ExecutionResult(StepExecutionStatusEnum.Failed, null,
                    "No Task Service registered.")) :
                _taskExecution.ExecuteAsync(this);
        }

        /// <summary>
        /// Loads the execute assemply asynchronous.
        /// </summary>
        /// <param name="classFullName">Full name of the class.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual Task<ExecutionResult> LoadExecuteAssemplyAsync(string classFullName)
        {
            IExecutionService servce;
            Exception error=null;
            return (_assemblyServiceExecution == null || !_assemblyServiceExecution.TryLoadClass(classFullName,out servce,out error))?
                Task.FromResult(new ExecutionResult(StepExecutionStatusEnum.Failed, null,
                    string.Format("Cannot load class {0}. Error:{1}",classFullName,error?.Message ?? ""))) :
                servce.ExecuteAsync(this);
        }
        public T Get<T>(string name) where T : class
        {
            return _params.Get<T>(name);
        }

        public IProcessRuntimeEnvironment Set<T>(string name, T value) where T:class 
        {
            _params.Set(name,value);
            return this;
        }

        /// <summary>
        /// Set possible values for a variable
        /// </summary>
        /// <param name="name"></param>
        /// <param name="values"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IProcessRuntimeEnvironment SetPossibleValues(string name, params object[] values)
        {
            var valueSchema = PropertySet.Schemas.GetSchema(name);
            valueSchema.PossibleValues = values;
            return this;
        }

        /// <summary>
        /// Set IOC execution service
        /// </summary>
        /// <param name="execution"></param>
        public IProcessRuntimeEnvironment SetIocExecution(IIocServiceExecution execution)
        {
            _iocServiceExecution = execution;
            return this;
        }

        /// <summary>
        /// Set Task Execution service
        /// </summary>
        /// <param name="execution"></param>
        public IProcessRuntimeEnvironment SetTaskExecution(IExecutionService execution)
        {
            _taskExecution = execution;
            return this;
        }

        /// <summary>
        /// Set Assembly Execution service
        /// </summary>
        /// <param name="execution"></param>
        public IProcessRuntimeEnvironment SetAssemblyExecution(IAssemblyServiceExecution execution)
        {
            _assemblyServiceExecution = execution;
            return this;
        }

        /// <summary>
        /// Return Variable service for a variable defined in the workflowe
        /// </summary>
        /// <param name="variableName"></param>
        /// <returns></returns>
        public IVariablesService GetVariableService(string variableName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Tag Service Provider
        /// </summary>
        public ITagServiceProvider TagServiceProvider
        {
            get;
        }
    }
}