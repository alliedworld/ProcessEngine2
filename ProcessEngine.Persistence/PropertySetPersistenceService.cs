/**
The MIT License (MIT)

Copyright (c) 2016 Igor Polouektov

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
using System;
using System.Collections.Generic;
using System.Linq;
using Klaudwerk.PropertySet;
using Klaudwerk.PropertySet.Persistence;
using Klaudwerk.PropertySet.Serialization;
using KlaudWerk.PropertySet.Serialization;

namespace KlaudWerk.ProcessEngine.Persistence
{
    public class PropertySetPersistenceService
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
        /// Store the collection
        /// </summary>
        /// <param name="collectionId"></param>
        /// <param name="collection"></param>
        public void SaveCollection(Guid collectionId, IPropertySetCollection collection)
        {
            List<PersistentSchemaElement> schemaElements;
            List<PersistentPropertyElement> dataElements;
            collection.CreatePersistentSchemaElements(out schemaElements, out dataElements);
            using (ProcessDbContext ctx = new ProcessDbContext())
            {
                UpdateOrCreatePropertyCollection(collectionId, ctx, schemaElements, dataElements);
                ctx.SaveChanges();
            }
        }


        /// <summary>
        /// Save or Update Collection
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="collectionId"></param>
        /// <param name="collection"></param>
        public PersistentPropertyCollection SaveCollection(ProcessDbContext ctx,Guid collectionId, IPropertySetCollection collection)
        {
            List<PersistentSchemaElement> schemaElements;
            List<PersistentPropertyElement> dataElements;
            collection.CreatePersistentSchemaElements(out schemaElements, out dataElements);
            return UpdateOrCreatePropertyCollection(collectionId, ctx, schemaElements, dataElements);
        }
        /// <summary>
        /// Find and re-load the collection
        /// </summary>
        /// <param name="collectionId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IPropertySetCollection FindCollection(Guid collectionId)
        {
            using (ProcessDbContext ctx = new ProcessDbContext())
            {
                var persistancePropertyCollection = ctx.PropertySet.Find(collectionId);
                if (persistancePropertyCollection == null)
                    throw new ArgumentException($"Cannot find the property collection id={collectionId}");
                return persistancePropertyCollection.Deserialize();
            }
        }




        protected virtual PersistentPropertyCollection UpdateOrCreatePropertyCollection(Guid collectionId, ProcessDbContext ctx, List<PersistentSchemaElement> schemaElements,
            List<PersistentPropertyElement> dataElements)
        {
            PersistentPropertyCollection persistancePropertyCollection = ctx.PropertySet.Find(collectionId);
            if (persistancePropertyCollection == null)
            {
                ctx.PropertySet.Add(persistancePropertyCollection = new PersistentPropertyCollection
                {
                    Id = collectionId,
                    Schemas = schemaElements,
                    Elements = dataElements
                });
            }
            else
            {
                schemaElements.ForEach(
                    s =>
                        persistancePropertyCollection.Schemas.FirstOrDefault(c => c.SchemaName == s.SchemaName)?
                            .CopyFrom(s));
                dataElements.ForEach(
                    e => persistancePropertyCollection.Elements.FirstOrDefault(c => c.Name == e.Name)?.CopyFrom(e));
            }
            return persistancePropertyCollection;
        }
    }
}