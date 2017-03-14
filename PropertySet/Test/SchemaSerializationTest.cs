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
using Klaudwerk.PropertySet.Serialization;
using KlaudWerk.PropertySet.Serialization;
using NUnit.Framework;

namespace Klaudwerk.PropertySet.Test
{
    [TestFixture]
    public class SchemaSerializationTest
    {
        [Test]
        public void TestSerializeStringSchema()
        {
            IValueSchema<string> schema = new StringSchema {AllowNull = true, 
                DefaultValue = "abc",
                MaxLength = 100,
                MinLength = 2,
                PossibleValues = new[]{"abc","def"}
            };
            SchemaJsonSerializationVisitor visitor=new SchemaJsonSerializationVisitor();
            schema.Accept(visitor);
            Assert.AreEqual(typeof(string),visitor.ValueType);
            Assert.AreEqual(typeof(StringSchema),visitor.SchemaType);
            Assert.IsNotNull(visitor.JsonValue);
            Assert.AreNotEqual(0,visitor.JsonValue.Length);
        }
        [Test]
        public void TestSerializeIntSchema()
        {
            IValueSchema<int?> schema = new IntSchema
                                            {
                                                AllowNull = false,
                                                DefaultValue = 10,
                                                MaxValue = 100,
                                                MinValue = 2,
                                                PossibleValues = new int?[] {10, 20, 30, 100}
                                            };
            SchemaJsonSerializationVisitor visitor=new SchemaJsonSerializationVisitor();
            schema.Accept(visitor);
            Assert.AreEqual(typeof(int?), visitor.ValueType);
            Assert.AreEqual(typeof(IntSchema), visitor.SchemaType);
            Assert.IsNotNull(visitor.JsonValue);
            Assert.AreNotEqual(0, visitor.JsonValue.Length);
        }

        [Test]
        public void TestDeserializeStringSchema()
        {
            IValueSchema<string> schema = new StringSchema
            {
                AllowNull = true,
                DefaultValue = "abc",
                MaxLength = 100,
                MinLength = 2,
                PossibleValues = new[] { "abc", "def" }
            };
            SchemaJsonSerializationVisitor visitor=new SchemaJsonSerializationVisitor();
            schema.Accept(visitor);

            IValueSchema<object> vs=JsonSchemaDeserializer.Deserialize(visitor.SchemaType, visitor.JsonValue);
            Assert.IsNotNull(vs);
            Assert.AreEqual(typeof(string),vs.Type);
        }

        [Test]
        public void TestDeserializeIntSchema()
        {
            IValueSchema<int?> schema = new IntSchema
            {
                AllowNull = false,
                DefaultValue = 10,
                MaxValue = 100,
                MinValue = 2,
                PossibleValues = new int?[] { 10, 20, 30, 100 }
            };
            SchemaJsonSerializationVisitor visitor=new SchemaJsonSerializationVisitor();
            schema.Accept(visitor);
            IValueSchema<object> vs = JsonSchemaDeserializer.Deserialize(visitor.SchemaType, visitor.JsonValue);
            Assert.IsNotNull(vs);
            Assert.AreEqual(typeof(int?), vs.Type);
        }

        [Test]
        public void TestStringSerializeProperties()
        {
            JsonPropertySerializer serializer=new JsonPropertySerializer(new PropertySchemaFactory());
            StringSchema schema=new StringSchema{MaxLength = 100,MinLength = 1, DefaultValue = "hello"};
            PropertyElement element = serializer.Serialize("hello", schema.Wrap());
            Assert.IsNotNull(element);
            Assert.AreEqual(typeof(StringSchema).FullName,element.Schema.SchemaType);
            Assert.AreEqual(SerializationTypeHint.String,element.SerializationHint);
            Assert.AreEqual("hello",element.Value);
        }

        [Test]
        public void TestStringDeSerializeProperties()
        {
            JsonPropertySerializer serializer = new JsonPropertySerializer(new PropertySchemaFactory());
            StringSchema schema = new StringSchema { MaxLength = 100, MinLength = 1, DefaultValue = "hello" };
            PropertyElement element = serializer.Serialize("hello", schema.Wrap());
            Assert.IsNotNull(element);
            Assert.AreEqual(typeof(StringSchema).FullName, element.Schema.SchemaType);
            Assert.AreEqual(SerializationTypeHint.String, element.SerializationHint);
            Assert.AreEqual("hello", element.Value);

            IValueSchema<object> vs = serializer.DeserializeSchema(element.Schema);
            Assert.IsNotNull(vs);
            IValueSchema<string> strSchema = vs.UnwrapRefType<string>();
            Assert.IsNotNull(strSchema);
        }

        [Test]
        public void TestSerializeRolesSchema()
        {
            IValueSchema<string> schema = new RolesSchema()
            {
                AllowNull = false,
                DefaultValue = "abc",
                MaxLength = 100,
                MinLength = 2,
                PossibleValues = new[] { "abc", "def" }
            };
            SchemaJsonSerializationVisitor visitor = new SchemaJsonSerializationVisitor();
            schema.Accept(visitor);
            Assert.AreEqual(typeof(string), visitor.ValueType);
            Assert.AreEqual(typeof(RolesSchema), visitor.SchemaType);
            Assert.IsNotNull(visitor.JsonValue);
            Assert.AreNotEqual(0, visitor.JsonValue.Length);
        }

        [Test]
        public void TestSerializeGroupsSchema()
        {
            IValueSchema<string> schema = new GroupsSchema()
            {
                AllowNull = false,
                DefaultValue = "abc",
                MaxLength = 100,
                MinLength = 2,
                PossibleValues = new[] { "abc", "def" }
            };
            SchemaJsonSerializationVisitor visitor = new SchemaJsonSerializationVisitor();
            schema.Accept(visitor);
            Assert.AreEqual(typeof(string), visitor.ValueType);
            Assert.AreEqual(typeof(GroupsSchema), visitor.SchemaType);
            Assert.IsNotNull(visitor.JsonValue);
            Assert.AreNotEqual(0, visitor.JsonValue.Length);

        }

        [Test]
        public void TestSerializeUsersSchema()
        {
            IValueSchema<string> schema = new UsersSchema
            {
                AllowNull = false,
                DefaultValue = "abc",
                MaxLength = 100,
                MinLength = 2,
                PossibleValues = new[] { "abc", "def" }
            };
            SchemaJsonSerializationVisitor visitor = new SchemaJsonSerializationVisitor();
            schema.Accept(visitor);
            Assert.AreEqual(typeof(string), visitor.ValueType);
            Assert.AreEqual(typeof(UsersSchema), visitor.SchemaType);
            Assert.IsNotNull(visitor.JsonValue);
            Assert.AreNotEqual(0, visitor.JsonValue.Length);

        }
        [Test]
        public void TestDeserializeRolesSchema()
        {
            IValueSchema<string> schema = new RolesSchema
            {
                AllowNull = true,
                DefaultValue = "abc",
                MaxLength = 100,
                MinLength = 2,
                PossibleValues = new[] { "abc", "def" }
            };
            SchemaJsonSerializationVisitor visitor = new SchemaJsonSerializationVisitor();
            schema.Accept(visitor);

            IValueSchema<object> vs = JsonSchemaDeserializer.Deserialize(visitor.SchemaType, visitor.JsonValue);
            Assert.IsNotNull(vs);
            Assert.AreEqual(typeof(string), vs.Type);
        }

        [Test]
        public void TestDeserializeGroupsSchema()
        {
            IValueSchema<string> schema = new GroupsSchema
            {
                AllowNull = true,
                DefaultValue = "abc",
                MaxLength = 100,
                MinLength = 2,
                PossibleValues = new[] { "abc", "def" }
            };
            SchemaJsonSerializationVisitor visitor = new SchemaJsonSerializationVisitor();
            schema.Accept(visitor);

            IValueSchema<object> vs = JsonSchemaDeserializer.Deserialize(visitor.SchemaType, visitor.JsonValue);
            Assert.IsNotNull(vs);
            Assert.AreEqual(typeof(string), vs.Type);
        }

        [Test]
        public void TestDeserializeUsersSchema()
        {
            IValueSchema<string> schema = new UsersSchema
            {
                AllowNull = true,
                DefaultValue = "abc",
                MaxLength = 100,
                MinLength = 2,
                PossibleValues = new[] { "abc", "def" }
            };
            SchemaJsonSerializationVisitor visitor = new SchemaJsonSerializationVisitor();
            schema.Accept(visitor);

            IValueSchema<object> vs = JsonSchemaDeserializer.Deserialize(visitor.SchemaType, visitor.JsonValue);
            Assert.IsNotNull(vs);
            Assert.AreEqual(typeof(string), vs.Type);
        }


    }

}
