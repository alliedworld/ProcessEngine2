using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Klaudwerk.PropertySet;
using KlaudWerk.ProcessEngine.Runtime;

namespace KlaudWerk.ProcessEngine
{
    /// <summary>
    /// Process Runtime Environment Interface
    /// </summary>
    public interface IProcessRuntimeEnvironment
    {
        /// <summary>
        /// Current Step
        /// </summary>
        StepRuntime CurrentStep { get; set; }
        /// <summary>
        /// Caller claim principal
        /// </summary>
        ClaimsPrincipal Principal { get; set; }    
        /// <summary>
        /// Variable values associated with the workflow
        /// </summary>
        IPropertySetCollection PropertySet { get; }
        int ProcessEnvId { get; set; }
        /// <summary>
        /// Id of the process
        /// </summary>
        Guid ProcessId { get; }
        /// <summary>
        /// Id of the transition that needs to be taken
        /// </summary>
        string Transition { get; }
        /// <summary>
        /// Execute the IOC service asynchronous.
        /// </summary>
        /// <param name="iocName">Name of the ioc.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        Task<ExecutionResult> IocServiceAsync(string iocName);
        /// <summary>
        /// Executes the Task service asynchronous.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        Task<ExecutionResult> TaskServiceAsync();
        /// <summary>
        /// Loads the execute assemply asynchronous.
        /// </summary>
        /// <param name="classFullName">Full name of the class.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        Task<ExecutionResult> LoadExecuteAssemplyAsync(string classFullName);
        /// <summary>
        /// Get extra parameters
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        T Get<T>(string name) where T : class;
        /// <summary>
        /// Set Extra Parameters
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="value"></param>
        IProcessRuntimeEnvironment Set<T>(string name, T value) where T : class;
        /// <summary>
        /// Set IOC execution service
        /// </summary>
        /// <param name="execution"></param>
        IProcessRuntimeEnvironment SetIocExecution(IIocServiceExecution execution);
        /// <summary>
        /// Set Task Execution service
        /// </summary>
        /// <param name="execution"></param>
        IProcessRuntimeEnvironment SetTaskExecution(IExecutionService execution);
        /// <summary>
        /// Set Assembly Execution service
        /// </summary>
        /// <param name="execution"></param>
        IProcessRuntimeEnvironment SetAssemblyExecution(IAssemblyServiceExecution execution);
    }
}