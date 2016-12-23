namespace KlaudWerk.ProcessEngine
{
    /// <summary>
    /// Enum defines the type of Step Processing
    /// </summary>
    public enum StepHandlerTypeEnum
    {
        /// <summary>
        /// No processing
        /// </summary>
        None=0,
        /// <summary>
        /// Script supplied with the script to execute
        /// </summary>
        Script = 1,
        /// <summary>
        /// Human Task
        /// </summary>
        Task,
        /// <summary>
        /// Inversion Of Control Service injected through Env Variables
        /// </summary>
        IoC,
        /// <summary>
        /// Service that step processor will load and execute
        /// </summary>
        Service
    }
}