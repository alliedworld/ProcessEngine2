using System;
using System.Collections.Generic;

namespace KlaudWerk.ProcessEngine.Persistence
{
    /// <summary>
    /// Persisted collection of property elements
    /// </summary>
    public class PersistentPropertyCollection
    {
        /// <summary>
        /// Gets or sets the unique collection id.
        /// </summary>
        /// <value>The id.</value>
        public virtual Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        public virtual int Version { get; set; }

        /// <summary>
        /// Gets or sets the elements.
        /// </summary>
        /// <value>The elements.</value>
        public virtual List<PersistentPropertyElement> Elements { get; set; }
        public virtual List<PersistentSchemaElement> Schemas { get; set; }
    }
}