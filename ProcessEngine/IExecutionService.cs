using System.Threading.Tasks;

namespace KlaudWerk.ProcessEngine
{
    /// <summary>
    /// Generic Execution Service
    /// </summary>
    public interface IExecutionService
    {
        /// <summary>
        /// Execute the task service asynchronously
        /// </summary>
        /// <param name="env"></param>
        /// <returns></returns>
        Task<ExecutionResult> ExecuteAsync(IProcessRuntimeEnvironment env);
        /// <summary>
        /// Exeute the task synchronously
        /// </summary>
        /// <param name="env"></param>
        /// <returns></returns>
        ExecutionResult Execute(IProcessRuntimeEnvironment env);

    }
}