using System.Threading.Tasks;

namespace KlaudWerk.ProcessEngine
{
    /// <summary>
    /// Execute Task Related Service
    /// </summary>
    public interface ITaskServiceExecution
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