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