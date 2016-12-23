using System;
using System.Threading.Tasks;
using Klaudwerk.PropertySet;

namespace KlaudWerk.ProcessEngine
{
    /// <summary>
    /// Process Runtime Environment Interface
    /// </summary>
    public interface IProcessRuntimeEnvironment
    {
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
    }

    /// <summary>
    /// Process Runtime Environment
    /// </summary>
    public class ProcessRuntimeEnvironment : IProcessRuntimeEnvironment
    {

        public IPropertySetCollection PropertySet { get; }

        public int ProcessEnvId { get; set; }
        public Guid ProcessId { get; set; }
        public string Transition { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessRuntimeEnvironment"/> class.
        /// </summary>
        /// <param name="propertySet">The property set.</param>
        public ProcessRuntimeEnvironment(IPropertySetCollection propertySet)
        {
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Executes the Task service asynchronous.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual Task<ExecutionResult> TaskServiceAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Loads the execute assemply asynchronous.
        /// </summary>
        /// <param name="classFullName">Full name of the class.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual Task<ExecutionResult> LoadExecuteAssemplyAsync(string classFullName)
        {
            throw new NotImplementedException();
        }
    }
}