using System;
using System.Collections.Generic;
using Klaudwerk.PropertySet.Persistence;

namespace KlaudWerk.ProcessEngine.Persistence
{
    /// <summary>
    /// Base class for Process Runtime Persistence
    /// </summary>
    public abstract class ProcessRuntimePersistenceBase
    {
        /// <summary>
        /// Process Id
        /// </summary>
        public virtual Guid Id { get; set; }

        /// <summary>
        /// Last Modified Date
        /// </summary>
        public virtual DateTime LastUpdated { get; set; }

        /// <summary>
        /// Process Status
        /// </summary>
        public virtual int Status { get; set; }

        /// <summary>
        /// Associated Propery Collection
        /// </summary>
        public virtual PersistentPropertyCollection PropertyCollection { get; set; }

        /// <summary>
        /// Id of Suspended Step. Can contain Null
        /// </summary>
        public virtual string SuspendedStepId { get; set; }

        /// <summary>
        /// Id of Next Step
        /// </summary>
        public virtual string NextStepId { get; set; }

        /// <summary>
        /// List of Errors
        /// </summary>
        public virtual IList<string> Errors { get; set; }

        /// <summary>
        /// Get Process Definition
        /// </summary>
        /// <returns></returns>
        public abstract ProcessDefinitionPersistenceBase GetProcessDefinition();
    }
}