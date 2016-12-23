using System;
using System.Collections.Generic;
using System.IO;
using Klaudwerk.PropertySet;
using Klaudwerk.PropertySet.Serialization;
using Newtonsoft.Json;

namespace KlaudWerk.PropertySet.Serialization
{
    /// <summary>
    /// JSON serialization visitor
    /// </summary>
    public class SchemaJsonSerializationVisitor:IValueSchemaVisitor
    {
        private readonly  Dictionary<Type,SerializationTypeHint> _hintSchemaMap=new Dictionary<Type,SerializationTypeHint>
        {
            {typeof(int?),SerializationTypeHint.Int},
            {typeof(long?),SerializationTypeHint.Long},
            {typeof(bool?),SerializationTypeHint.Bool},
            {typeof(double?),SerializationTypeHint.Double},
            {typeof(DateTime?),SerializationTypeHint.DateTime},
            {typeof(string),SerializationTypeHint.String}
        };
        public Type SchemaType { get; private set; }
        public Type ValueType { get; private set; }
        public string JsonValue { get; private set; }
        public SerializationTypeHint Hint { get; private set; }
        /// <summary>
        /// Visits the specified schema.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="schema">The schema.</param>
        public void Visit<T>(IValueSchema<T> schema) where T : class
        {
            SchemaType = schema.GetType();
            ValueType = schema.Type;
            Hint = GetHint(schema);
            JsonValue = Serialize(schema);
        }

        /// <summary>
        /// Visits the specified schema.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="schema">The schema.</param>
        public void Visit<T>(IValueSchema<T?> schema) where T : struct
        {
            SchemaType = schema.GetType();
            ValueType = schema.Type;
            Hint = GetHint(schema);
            JsonValue = Serialize(schema);

        }

        private SerializationTypeHint GetHint<T>(IValueSchema<T?> schema) where T : struct
        {
            SerializationTypeHint hint;
            if (!_hintSchemaMap.TryGetValue(schema.Type, out hint))
            {
                hint=SerializationTypeHint.Object;
            }
            return hint;
        }

        private SerializationTypeHint GetHint<T>(IValueSchema<T> schema) where T : class
        {
            SerializationTypeHint hint;
            if (!_hintSchemaMap.TryGetValue(schema.Type, out hint))
            {
                hint=SerializationTypeHint.Object;
            }
            return hint;
        }

        /// <summary>
        /// Serializes the specified schema to JSON .
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <returns></returns>
        private static string Serialize(object schema)
        {
            JsonSerializer json=new JsonSerializer();
            StringWriter writer = new StringWriter();
            json.Serialize(writer,schema);
            writer.Flush();
            return writer.ToString();
        }

    }
}
