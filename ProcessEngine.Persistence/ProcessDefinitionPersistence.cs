using System;
using System.Collections.Generic;

namespace KlaudWerk.ProcessEngine.Persistence
{
    /// <summary>
    /// Persistent Process Definition
    /// </summary>
    public class ProcessDefinitionPersistence
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Version
        /// </summary>
        public int Version { get; set; }
        /// <summary>
        /// Process Definition Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Process Defnition Description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Status
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// MD5 of the definition
        /// </summary>
        public string Md5 { get; set; }
        /// <summary>
        /// Serialized JSON body
        /// </summary>
        public string JsonProcessDefinition { get; set; }
        /// <summary>
        /// Last Modified
        /// </summary>
        public DateTime LastModified { get; set; }
        /// <summary>
        /// Flow Id
        /// </summary>
        public string FlowId { get; set; }
        /// <summary>
        /// List of accounts that
        /// </summary>
        public virtual ICollection<ProcessDefinitionAccount> Accounts { get; set; }
    }
}