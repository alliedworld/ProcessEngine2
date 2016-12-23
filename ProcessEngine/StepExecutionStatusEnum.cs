namespace KlaudWerk.ProcessEngine
{
    /// <summary>
    /// Process Step Execution Result Enum
    /// </summary>
    public enum StepExecutionStatusEnum
    {
        /// <summary>
        /// No execution was performed
        /// </summary>
        None = 0,
        /// <summary>
        /// The execution failed
        /// </summary>
        Failed,
        /// <summary>
        /// The process should be suspended until an external event suspend
        /// </summary>
        Suspend,
        /// <summary>
        /// Ready
        /// </summary>
        Ready,
        /// <summary>
        /// The process is completed
        /// </summary>
        Completed
    }
}