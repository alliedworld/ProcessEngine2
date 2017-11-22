using System.Collections.Generic;

namespace KlaudWerk.ProcessEngine
{
    /// <summary>
    /// Variable service interface
    /// </summary>
    public interface IVariablesService
    {
        /// <summary>
        /// The service capabilities
        /// </summary>
        VariableServiceCapabilities Capabilities { get; }
        /// <summary>
        /// Try to validate a variable value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="errors"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        bool TryValidate<T>(T value, out string[] errors);
        /// <summary>
        /// Return the list of possible values
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IEnumerable<T> GetPossibleValues<T>();
    }
}