using System;
using System.Collections.Generic;

namespace KlaudWerk.ProcessEngine.Runtime
{
    /// <summary>
    /// Process Runtime interface
    /// </summary>
    public interface IProcessRuntime : ICompilable
    {
        /// <summary>
        /// Process Id
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Process State
        /// </summary>
        ProcessStateEnum State { get; }

        /// <summary>
        /// Process was suspended in step
        /// </summary>
        StepRuntime SuspendedInStep { get; }

        /// <summary>
        /// Last Executed Step in the process
        /// </summary>
        StepRuntime LastExecutedStep { get; }

        /// <summary>
        /// Start Steps in the workflow
        /// </summary>
        IReadOnlyList<StepRuntime> StartSteps { get; }

        /// <summary>
        /// List of errors
        /// </summary>
        IReadOnlyList<string> Errors { get; }

        /// <summary>
        /// Try to compile all compilabe artifacts
        /// </summary>
        /// <param name="errors"></param>
        /// <returns></returns>
        bool TryCompile(out string[] errors);

        /// <summary>
        /// Execute Single Step in the workflow
        /// </summary>
        /// <returns>
        /// This method returns a tuple of two elements:
        /// <see cref="ExecutionResult"/>
        /// <see cref="StepRuntime"/>
        /// Next Step in Execution flow
        /// If the execution result has value of Suspend
        ///  </returns>
        Tuple<ExecutionResult,StepRuntime> Execute(StepRuntime rt,IProcessRuntimeEnvironment env);

        /// <summary>
        /// Continue the process execution from Suspended state
        /// </summary>
        /// <param name="env"></param>
        /// <returns></returns>
        Tuple<ExecutionResult, StepRuntime> Continue(IProcessRuntimeEnvironment env);
    }
}