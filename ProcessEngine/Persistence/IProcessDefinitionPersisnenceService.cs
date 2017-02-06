using System;
using System.Collections.Generic;
using KlaudWerk.ProcessEngine.Definition;

namespace KlaudWerk.ProcessEngine.Persistence
{
    /// <summary>
    /// Service Interface for Process Definition Persistence
    /// </summary>
    public interface IProcessDefinitionPersisnenceService
    {
        /// <summary>
        /// List all Available workflow
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<ProcessDefinitionDigest> LisAlltWorkflows(params string[] accounts);

        /// <summary>
        /// List of all active workflow
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<ProcessDefinitionDigest> ActivetWorkflows(params string[] accounts);

        /// <summary>
        /// Save process definition
        /// </summary>
        /// <param name="definition"></param>
        /// <param name="status"></param>
        /// <param name="version"></param>
        /// <param name="accounts">Security accounts that have access to the flow</param>
        void Create(ProcessDefinition definition,
            ProcessDefStatusEnum status,
            int version,params AccountData[] accounts);

        /// <summary>
        /// Set or update status
        /// </summary>
        /// <param name="id"></param>
        /// <param name="version"></param>
        /// <param name="status"></param>
        bool SetStatus(Guid id, int version, ProcessDefStatusEnum status);

        /// <summary>
        /// Load and deserialize the process definition
        /// </summary>
        /// <param name="id"></param>
        /// <param name="version"></param>
        /// <param name="definition"></param>
        /// <param name="status"></param>
        /// <param name="accounts"></param>
        /// <returns></returns>
        bool TryFind(Guid id, int version, out ProcessDefinition definition,
            out ProcessDefStatusEnum status,
            out AccountData[] accounts);


        /// <summary>
        /// Update Process Definition
        /// </summary>
        /// <param name="id"></param>
        /// <param name="version"></param>
        /// <param name="action"></param>
        /// <param name="accounts"></param>
        /// <exception cref="NotImplementedException"></exception>
        void Update(Guid id, int version, Action<ProcessDefinitionPersistenceBase> action, AccountData[] accounts=null);

        /// <summary>
        /// Create Security Account Records
        /// </summary>
        /// <param name="accounts"></param>
        void CreateAccounts(params AccountData[] accounts);
    }
}