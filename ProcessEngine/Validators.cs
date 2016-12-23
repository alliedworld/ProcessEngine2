using System;

namespace KlaudWerk.ProcessEngine
{
    /// <summary>
    /// Validators Extension Functions
    /// </summary>
    public static class Validators
    {
        /// <summary>
        /// Check if 'this' parameter is bot null
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static object NotNull(this object obj, string name)
        {
            if (obj == null)
                throw new ArgumentNullException($"Parameter {name} cannot be null.");
            return obj;
        }
        /// <summary>
        /// Checks if 'this' parameter is not a empty string.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static object NotEmptyString(this object obj, string name)
        {
            if (obj != null && obj is string && string.Empty.Equals(((string)obj).Trim()))
                throw new ArgumentException($"String {name} cannot be empty.");
            return obj;
        }
    }
}
