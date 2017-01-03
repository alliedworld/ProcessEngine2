using System;
using System.Collections.Generic;
using System.Linq;
using Klaudwerk.PropertySet.Serialization;
using KlaudWerk.PropertySet.Serialization;


namespace Klaudwerk.PropertySet.Persistence
{
    public static class PropertyCollectionExt
    {
        private static readonly
            Dictionary<SerializationTypeHint, Func<PersistentPropertyElement, object>>
            _serializationMap = new Dictionary
                <SerializationTypeHint, Func<PersistentPropertyElement, object>>
                {
                    {SerializationTypeHint.Null, (e) => null},
                    {SerializationTypeHint.Int, (e) => e.IntValue},
                    {SerializationTypeHint.Long, (e) => e.LongValue},
                    {SerializationTypeHint.Bool, (e) => e.IntValue != null && e.IntValue == 1},
                    {SerializationTypeHint.Double, (e) => e.DoubleValue},
                    {SerializationTypeHint.DateTime, (e) => e.DateTimeValue},
                    {SerializationTypeHint.String, (e) => e.StringValue},
                    {SerializationTypeHint.ByteArray, ( e) => e.RawValue},
                    {SerializationTypeHint.JsonString,(e)=>e.StringValue},
                    {SerializationTypeHint.BinaryObject,(e)=>e.RawValue},
                    {SerializationTypeHint.Object,(e)=>e.RawValue}
                };
        /// <summary>
        /// Create Persistent Schema Elements
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="schemaElements"></param>
        /// <param name="dataElements"></param>
        public static void CreatePersistentSchemaElements(this IPropertySetCollection collection, out List<PersistentSchemaElement> schemaElements,
            out List<PersistentPropertyElement> dataElements)
        {
            schemaElements = new List<PersistentSchemaElement>();
            dataElements = new List<PersistentPropertyElement>();
            foreach (var schema in collection.Schemas.Schemas)
            {
                SchemaJsonSerializationVisitor visitor = new SchemaJsonSerializationVisitor();
                schema.Value.Accept(visitor);
                PersistentSchemaElement se = new PersistentSchemaElement
                {
                    SchemaName = schema.Key,
                    SchemaType = visitor.SchemaType.ToString(),
                    SchemaBody = visitor.JsonValue,
                    SerializationHint = (int) visitor.Hint
                };
                schemaElements.Add(se);
            }
            foreach (string k in collection.Keys)
            {
                PersistentPropertyElement element = new PersistentPropertyElement
                {
                    Name = k
                };
                ValueSerializationTarget target = new ValueSerializationTarget(element);
                collection.Schemas.GetSchema(k).Serializer.Serialize(collection[k], target);
                dataElements.Add(element);
            }
        }

        /// <summary>
        /// Deserialize schemas and collection elements
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static IPropertySetCollection Deserialize(this PersistentPropertyCollection collection)
        {
            PropertySchemaSet schemaSet=new PropertySchemaSet(new PropertySchemaFactory());
            PropertySetCollection restored = new PropertySetCollection(schemaSet);
            //restore schemas
            foreach (PersistentPropertyElement element in collection.Elements)
            {
                PersistentSchemaElement schemaElement = collection.Schemas.First(s => s.SchemaName == element.Name);
                var valueSchema = JsonSchemaDeserializer.Deserialize(schemaElement.SchemaType, schemaElement.SchemaBody);
                SerializationTypeHint hint =(SerializationTypeHint) schemaElement.SerializationHint;
                Func<PersistentPropertyElement, object> valueRetriever;
                if (!_serializationMap.TryGetValue(hint, out valueRetriever))
                {
                    throw new ArgumentException($"{element.Name} {hint}");
                }
                object val =  valueRetriever(element);
                switch (hint)
                {
                    case SerializationTypeHint.JsonString:
                        val = valueSchema.Serializer.Deserialize(val.ToString());
                        break;
                    case SerializationTypeHint.BinaryObject:
                    case SerializationTypeHint.Object:
                        byte[] data = (byte[]) val;
                        val=valueSchema.Serializer.Deserialize(data);
                        break;
                }
                restored.Add(element.Name, val, valueSchema);
            }
            return restored;
        }
    }
}