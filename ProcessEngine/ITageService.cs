using System.Collections.Generic;

namespace KlaudWerk.ProcessEngine
{
    /// <summary>
    /// Tag Service
    /// </summary>
    public interface ITageService
    {
        string Name { get; }
        string DisplayName { get; }
        IReadOnlyList<string> GetValues(IProcessRuntimeEnvironment env);

    }
}