using System;

namespace KlaudWerk.ProcessEngine
{
    [Flags]
    public enum VariableServiceCapabilities {
        /// <summary>
        /// The service can validate the variable value
        /// </summary>
        Validate=1,
        /// <summary>
        /// The service can provide the list of possible values
        /// </summary>
        ListProvider
    }
}