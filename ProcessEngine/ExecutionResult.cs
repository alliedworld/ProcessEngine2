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