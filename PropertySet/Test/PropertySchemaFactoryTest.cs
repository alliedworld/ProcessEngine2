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
using NUnit.Framework;

namespace Klaudwerk.PropertySet.Test
{
    [TestFixture]
    public class PropertySchemaFactoryTest
    {
        [Test]
        public void TestCreateDefaultSchemasRegisteredTypes()
        {
            IPropertySchemaFactory f = new PropertySchemaFactory();
            IValueSchema<object> schema = f.Create(typeof (int));
            Assert.IsNotNull(schema);
            Assert.AreEqual(typeof(int?), schema.Type);
            schema = f.Create(typeof(int?));
            Assert.AreEqual(typeof(int?), schema.Type);
            Assert.IsNotNull(schema);
            Assert.AreEqual(typeof (int?), schema.Type);
            schema = f.Create(typeof(long));
            Assert.IsNotNull(schema);
            Assert.AreEqual(typeof(long?), schema.Type);
            schema = f.Create(typeof(long?));
            Assert.IsNotNull(schema);
            Assert.AreEqual(typeof(long?), schema.Type);
            schema = f.Create(typeof(DateTime));
            Assert.IsNotNull(schema);
            Assert.AreEqual(typeof(DateTime?), schema.Type);
            schema = f.Create(typeof(DateTime?));
            Assert.IsNotNull(schema);
            Assert.AreEqual(typeof(DateTime?), schema.Type);
            schema = f.Create(typeof(bool));
            Assert.IsNotNull(schema);
            Assert.AreEqual(typeof(bool?), schema.Type);
            schema = f.Create(typeof(bool));
            Assert.AreEqual(typeof(bool?), schema.Type);
            Assert.IsNotNull(schema);
            schema = f.Create(typeof(string));
            Assert.IsNotNull(schema);
            Assert.AreEqual(typeof(string), schema.Type);
        }
        [Test]
        public void TestCreateDefaultSchemaAnyType()
        {
            IPropertySchemaFactory f = new PropertySchemaFactory();
            IValueSchema<object> schema = f.Create(typeof(double));
            Assert.IsNotNull(schema);
            Assert.AreEqual(typeof(double?),schema.Type);
            
        }
        [Test]
        public void TestRegisterSchemaForType()
        {
            IPropertySchemaFactory f = new PropertySchemaFactory();
            f.RegisterSchema(typeof(double), ()=>new IntSchema().Wrap());
            f.RegisterSchema(typeof(double?), () => new IntSchema().Wrap());
            IValueSchema<object> schema = f.Create(typeof(double));
            Assert.IsNotNull(schema);
            Assert.AreEqual(typeof(int?),schema.Type);

        }
    }
}
