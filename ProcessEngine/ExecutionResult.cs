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

namespace KlaudWerk.ProcessEngine
{
    /// <summary>
    /// This class encapsulates the result of a Step execution
    /// </summary>
    public class ExecutionResult
    {
        /// <summary>
        /// Gets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public StepExecutionStatusEnum Status { get; }
        /// <summary>
        /// Gets the correlation identifier.
        /// </summary>
        /// <value>
        /// The correlation identifier should be set for external events.
        /// </value>
        public Guid? CorrelationId { get; }
        public string[] ErrorMessages { get; }


        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionResult"/> class.
        /// </summary>
        /// <param name="status">The status.</param>
        public ExecutionResult(StepExecutionStatusEnum status)
            :this(status,Guid.Empty,new string[]{})
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionResult"/> class.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="correlationId">The correlation identifier.</param>
        /// <param name="errorMessages">The error messages.</param>
        public ExecutionResult(StepExecutionStatusEnum status,
            Guid? correlationId, params string[] errorMessages)
        {
            Status = status;
            CorrelationId = correlationId;
            ErrorMessages = errorMessages??new string[]{};
        }
    }
}