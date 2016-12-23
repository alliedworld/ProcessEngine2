using System;
using System.Collections.Generic;

namespace KlaudWerk.ProcessEngine.Persistence
{
    /// <summary>
    /// Process Runtime Persistence Class
    /// </summary>
    public class ProcessRuntimePersistence
    {
        /// <summary>
        /// Process Id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Last Modified Date
        /// </summary>
        public DateTime LastUpdated { get; set; }
        /// <summary>
        /// Process Status
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// Associated Process Swdinirion
        /// </summary>
        public virtual ProcessDefinitionPersistence ProcessDefinition { get; set; }
        /// <summary>
        /// Associated Propery Collection
        /// </summary>
        public virtual PersistentPropertyCollection PropertyCollection { get; set; }
        /// <summary>
        /// Id of Suspended Step. Can contain Null
        /// </summary>
        public string SuspendedStepId { get; set; }
        /// <summary>
        /// Id of Next Step
        /// </summary>
        public string NextStepId { get; set; }
        /// <summary>
        /// List of Errors
        /// </summary>
        public virtual IList<string> Errors { get; set; }
    }
}