using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KlaudWerk.ProcessEngine.Persistence
{
    /// <summary>
    /// Persistence Link class
    /// </summary>
    public class ProcessDefinitionAccount
    {
        /// <summary>
        /// Gets or sets the process definition identifier.
        /// </summary>
        /// <value>
        /// The process definition identifier.
        /// </value>
        [Key, Column(Order = 0)]
        public Guid ProcessDefinitionId { get; set; }
        /// <summary>
        /// Gets or sets the process definition version.
        /// </summary>
        /// <value>
        /// The process definition version.
        /// </value>
        [Key, Column(Order = 1)]
        public int ProcessDefinitionVersion { get; set; }
        /// <summary>
        /// Gets or sets the account data identifier.
        /// </summary>
        /// <value>
        /// The account data identifier.
        /// </value>
        [Key, Column(Order = 2)]
        public Guid AccountDataId { get; set; }

        /// <summary>
        /// Gets or sets the process definition.
        /// </summary>
        /// <value>
        /// The process definition.
        /// </value>
        public virtual ProcessDefinitionPersistence ProcessDefinition { get; set; }
        /// <summary>
        /// Gets or sets the account.
        /// </summary>
        /// <value>
        /// The account.
        /// </value>
        public virtual AccountData Account { get; set; }
    }
}