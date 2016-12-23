using System.Threading.Tasks;

namespace KlaudWerk.ProcessEngine
{
    /// <summary>
    /// Step Handler interface
    /// </summary>
    public interface IStepHandler:ICompilable
    {
        /// <summary>
        /// Executes the specified env.
        /// </summary>
        /// <param name="env">The env.</param>
        Task<ExecutionResult> ExecuteAsync(IProcessRuntimeEnvironment env);
    }
}