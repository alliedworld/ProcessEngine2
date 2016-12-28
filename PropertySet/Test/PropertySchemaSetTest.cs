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
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Klaudwerk.PropertySet.Test
{
    [TestFixture]
    public class PropertySchemaSetTest
    {
        private readonly Dictionary<string,IValueSchema<object>> _storage = new Dictionary<string, IValueSchema<object>>();

        [TearDown]
        public void TearDown()
        {
            _storage.Clear();
        }

        [Test]
        public void TestGetDefaultSchemas()
        {
            IPropertySchemaSet schemaSet = GetSchemaSet();
            IValueSchema<object> schema = schemaSet.GetDefaultSchema(typeof(int));
            Assert.IsNotNull(schema);
            Assert.AreEqual(typeof(int?), schema.Type);
            schema = schemaSet.GetDefaultSchema(typeof(int?));
            Assert.AreEqual(typeof(int?), schema.Type);
            Assert.IsNotNull(schema);
            Assert.AreEqual(typeof(int?), schema.Type);
            schema = schemaSet.GetDefaultSchema(typeof(long));
            Assert.IsNotNull(schema);
            Assert.AreEqual(typeof(long?), schema.Type);
            schema = schemaSet.GetDefaultSchema(typeof(long?));
            Assert.IsNotNull(schema);
            Assert.AreEqual(typeof(long?), schema.Type);
            schema = schemaSet.GetDefaultSchema(typeof(DateTime));
            Assert.IsNotNull(schema);
            Assert.AreEqual(typeof(DateTime?), schema.Type);
            schema = schemaSet.GetDefaultSchema(typeof(DateTime?));
            Assert.IsNotNull(schema);
            Assert.AreEqual(typeof(DateTime?), schema.Type);
            schema = schemaSet.GetDefaultSchema(typeof(bool));
            Assert.IsNotNull(schema);
            Assert.AreEqual(typeof(bool?), schema.Type);
            schema = schemaSet.GetDefaultSchema(typeof(bool));
            Assert.AreEqual(typeof(bool?), schema.Type);
            Assert.IsNotNull(schema);
            schema = schemaSet.GetDefaultSchema(typeof(string));
            Assert.IsNotNull(schema);
            Assert.AreEqual(typeof(string), schema.Type);
        }

        [Test]
        public void TestTryGetSetClassSchema()
        {
            IPropertySchemaSet schemaSet = GetSchemaSet();
            IValueSchema<string> s;
            Assert.IsFalse(schemaSet.TryGetSchema("string", out s));
            schemaSet.SetSchema("string",new StringSchema());
            Assert.IsTrue(schemaSet.TryGetSchema("string", out s));
            Assert.IsNotNull(s);
            Assert.AreEqual(typeof(string),s.Type);
        }

        [Test]
        public void TestTryGetSetValueTypeSchema()
        {
            IPropertySchemaSet schemaSet = GetSchemaSet();
            IValueSchema<int?> s;
            Assert.IsFalse(schemaSet.TryGetSchema("int", out s));
            schemaSet.SetSchema("int", new IntSchema());
            Assert.IsTrue(schemaSet.TryGetSchema("int", out s));
            Assert.IsNotNull(s);
            Assert.AreEqual(typeof(int?), s.Type);
        }

        [Test]
        public void TestEnumAllSchemas()
        {
            IPropertySchemaSet schemaSet = GetSchemaSet();
            schemaSet.SetSchema("string", new StringSchema());
            schemaSet.SetSchema("int", new IntSchema());
            IEnumerable<KeyValuePair<string,IValueSchema<object>>> sc=schemaSet.Schemas;
            Assert.IsNotNull(sc);
            Assert.AreEqual(2,sc.Count());
            foreach (KeyValuePair<string, IValueSchema<object>> pair in sc)
            {
                switch(pair.Key)
                {
                    case "string":
                        break;
                    case "int":
                        break;
                    default:
                        Assert.Fail("Unexpected schema name:"+pair.Key);
                        break;
                }
            }
        }

        [Test]
        public void TestRemoveSchema()
        {
            IPropertySchemaSet schemaSet = GetSchemaSet();
            IValueSchema<int?> s;
            Assert.IsFalse(schemaSet.TryGetSchema("int", out s));
            schemaSet.SetSchema("int", new IntSchema());
            Assert.IsTrue(schemaSet.TryGetSchema("int", out s));
            Assert.IsNotNull(s);
            Assert.AreEqual(typeof(int?), s.Type);
            schemaSet.RemoveSchema("int");
            Assert.IsFalse(schemaSet.TryGetSchema("int", out s));
        }

        [Test]
        public void TestRemoveAllSchemas()
        {
            IPropertySchemaSet schemaSet = GetSchemaSet();
            schemaSet.SetSchema("string", new StringSchema());
            schemaSet.SetSchema("int", new IntSchema());
            Assert.IsNotNull(schemaSet.Schemas);
            Assert.AreEqual(2,schemaSet.Schemas.Count());
            schemaSet.RemoveAll();
            Assert.IsNotNull(schemaSet.Schemas);
            Assert.AreEqual(0, schemaSet.Schemas.Count());
        }

        [Test]
        public void TestReplaceSchema()
        {
            IPropertySchemaSet schemaSet = GetSchemaSet();
            schemaSet.SetSchema("string", new StringSchema());
            IValueSchema<string> s;
            Assert.IsTrue(schemaSet.TryGetSchema("string", out s));
            Assert.IsNotNull(s);
            IValueSchema<int?> i;
            schemaSet.SetSchema("string", new IntSchema());
            Assert.IsTrue(schemaSet.TryGetSchema("string", out i));
        }


        [Test]
        public void TestGetSchemaType()
        {
            IPropertySchemaSet schemaSet = GetSchemaSet();
            schemaSet.SetSchema("string", new StringSchema());
            Type t = schemaSet.GetSchemaType("string");
            Assert.IsNotNull(t);
            Assert.AreEqual(typeof(string),t);
        }


        protected virtual IPropertySchemaSet GetSchemaSet()
        {
            return new ValueSetCollectionTest.MockPropertySchemaSet(new PropertySchemaFactory());
        }
    }
}
