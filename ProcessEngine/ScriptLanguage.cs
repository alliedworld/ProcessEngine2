using System;

namespace KlaudWerk.ProcessEngine
{
    /// <summary>
    /// Script Languages Enumeration
    /// </summary>
    [Flags]
    public enum ScriptLanguage
    {
        /// <summary>
        /// None
        /// </summary>
        None = 0,
        /// <summary>
        /// C# as scripting language
        /// </summary>
        CSharpScript = 1,
    }
}