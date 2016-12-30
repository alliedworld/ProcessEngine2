using System;

namespace KlaudWerk.ProcessEngine.Persistence
{
    /// <summary>
    /// Base class for Porcess Definition Persistence that does not include accounts
    /// </summary>
    public class ProcessDefinitionPersistenceBase
    {
        /// <summary>
        /// Id
        /// </summary>
        public virtual Guid Id { get; set; }

        /// <summary>
        /// Version
        /// </summary>
        public virtual int Version { get; set; }

        /// <summary>
        /// Process Definition Name
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Process Defnition Description
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        public virtual int Status { get; set; }

        /// <summary>
        /// MD5 of the definition
        /// </summary>
        public virtual string Md5 { get; set; }

        /// <summary>
        /// Serialized JSON body
        /// </summary>
        public virtual string JsonProcessDefinition { get; set; }

        /// <summary>
        /// Last Modified
        /// </summary>
        public virtual DateTime LastModified { get; set; }

        /// <summary>
        /// Flow Id
        /// </summary>
        public virtual string FlowId { get; set; }
    }
}