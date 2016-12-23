using System;

namespace KlaudWerk.ProcessEngine
{
    /// <summary>
    /// Enumeration that defines step required variables
    /// </summary>
    [Flags]
    public enum VarRequiredEnum
    {
        /// <summary>
        /// a variable value is optional
        /// </summary>
        None = 0,
        /// <summary>
        /// a variable value required to be set before a step body will be executed
        /// </summary>
        OnEntry=1,
        /// <summary>
        /// a variable value should be set by step body execution
        /// </summary>
        OnExit=2
    }
}