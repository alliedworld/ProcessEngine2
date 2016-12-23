using System.Collections.Generic;

namespace Klaudwerk.PropertySet
{
    public class PropertySchemaSet:PropertySchemaSetBase
    {
        private readonly Dictionary<string, IValueSchema<object>> _schemaStorage = new Dictionary<string, IValueSchema<object>>();
        public PropertySchemaSet(IPropertySchemaFactory schemaFactory) : base(schemaFactory)
        {
        }

        #region Overrides of PropertySchemaSetBase

        /// <summary>
        /// Gets the schemas.
        /// </summary>
        public override IEnumerable<KeyValuePair<string, IValueSchema<object>>> Schemas => _schemaStorage;

        /// <summary>
        /// Removes the schema.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public override bool RemoveSchema(string name)
        {
            return _schemaStorage.Remove(name);
        }

        /// <summary>
        /// Removes all schemas.
        /// </summary>
        public override void RemoveAll()
        {
            _schemaStorage.Clear();
        }

        /// <summary>
        /// Create or save the schema
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="wrapped">The wrapped.</param>
        protected override void OnSetSchema(string name, IValueSchema<object> wrapped)
        {
            _schemaStorage[name] = wrapped;
        }

        /// <summary>
        /// Try to get the schema
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="objSchema">The obj schema.</param>
        /// <returns></returns>
        protected override bool OnTryGetValue(string name, out IValueSchema<object> objSchema)
        {
            return _schemaStorage.TryGetValue(name, out objSchema);
        }

        #endregion
    }
}