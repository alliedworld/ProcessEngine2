namespace KlaudWerk.ProcessEngine
{
    /// <summary>
    /// IoC Execution service
    /// </summary>
    public interface IIocServiceExecution
    {
        /// <summary>
        /// Retrieve a service by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IExecutionService GetServce(string name);
    }
}