/**
The MIT License (MIT)

Copyright (c) 2013 Igor Polouektov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
  */
using System.Collections.Generic;

namespace Klaudwerk.PropertySet
{
    /// <summary>
    /// Default implementation of PropertySchema set
    /// </summary>
    /// <seealso cref="Klaudwerk.PropertySet.PropertySchemaSetBase" />
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