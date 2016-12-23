namespace KlaudWerk.ProcessEngine
{
    /// <summary>
    /// Compilable Artifacts
    /// </summary>
    public interface ICompilable
    {
        /// <summary>
        /// Try to compile the script
        /// </summary>
        /// <param name="errors"></param>
        /// <returns></returns>
        bool TryCompile(out string[] errors);
    }
}