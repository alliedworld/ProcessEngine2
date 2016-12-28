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
using System.Collections;
using System.Linq;
using NUnit.Framework;

namespace Klaudwerk.PropertySet.Test
{
    [TestFixture]
    public class TestSchemaExt
    {
        [Test]
        public void TestWrapObject()
        {
            StringSchema  schema=new StringSchema{AllowNull = true,DefaultValue = "Hello",PossibleValues = new[]{"one","two","Hello","Goodbye"}};
            IValueSchema<object> wrapper = schema.Wrap();
            Assert.IsNotNull(wrapper);
            Assert.AreEqual(typeof(string),wrapper.Type);
            Assert.IsTrue(wrapper.AllowNull);
            Assert.AreEqual("Hello",wrapper.DefaultValue);
            Assert.IsNotNull(wrapper.PossibleValues);
            Assert.AreEqual(4,wrapper.PossibleValues.Count());
            Assert.AreEqual("one",wrapper.PossibleValues.ElementAt(0));
            Assert.AreEqual("two", wrapper.PossibleValues.ElementAt(1));
            Assert.AreEqual("Hello", wrapper.PossibleValues.ElementAt(2));
            Assert.AreEqual("Goodbye", wrapper.PossibleValues.ElementAt(3));
        }

        [Test]
        public void TestWrapStruct()
        {
            IntSchema schema = new IntSchema{AllowNull = true,DefaultValue = 100,PossibleValues = new int?[]{1,2,3}};
            IValueSchema<object> wrapper = schema.Wrap();
            Assert.IsNotNull(wrapper);
            Assert.AreEqual(typeof(int?), wrapper.Type);
            Assert.IsTrue(wrapper.AllowNull);
            Assert.AreEqual(100,wrapper.DefaultValue);
            Assert.IsNotNull(wrapper.PossibleValues);
            Assert.AreEqual(3,wrapper.PossibleValues.Count());
            Assert.AreEqual(1,wrapper.PossibleValues.ElementAt(0));
            Assert.AreEqual(2, wrapper.PossibleValues.ElementAt(1));
            Assert.AreEqual(3, wrapper.PossibleValues.ElementAt(2));
        }


        [Test]
        public void TestUnwrapObject()
        {
            StringSchema schema = new StringSchema();
            IValueSchema<object> wrapper = schema.Wrap();
            Assert.IsNotNull(wrapper);
            Assert.AreEqual(typeof(string), wrapper.Type);
            IValueSchema<string> vs = wrapper.UnwrapRefType<string>();
            Assert.IsNotNull(vs);
            StringSchema stringSchema = vs as StringSchema;
            Assert.IsNotNull(stringSchema);
        }

        [Test]
        public void TestUnwrapStruct()
        {
            IntSchema schema = new IntSchema { AllowNull = true, DefaultValue = 100, PossibleValues = new int?[] { 1, 2, 3 } };
            IValueSchema<object> wrapper = schema.Wrap();
            Assert.IsNotNull(wrapper);
            Assert.AreEqual(typeof(int?), wrapper.Type);
            IValueSchema<int?> vs = wrapper.UnwrapValueType<int>();
            Assert.IsNotNull(vs);
            Assert.IsTrue(vs.AllowNull);
            Assert.AreEqual(100,vs.DefaultValue);
            IntSchema intSchema = vs as IntSchema;
            Assert.IsNotNull(intSchema);
        }

        [Test]
        public void TestUnwrapStructPassingObjecSchema()
        {
            StringSchema schema = new StringSchema();
            IValueSchema<object> wrapper = schema.Wrap();
            Assert.IsNotNull(wrapper);
            Assert.AreEqual(typeof(string), wrapper.Type);
            Assert.Throws<InvalidCastException>(() =>
            {
                wrapper.UnwrapValueType<int>();
            });
        }

        [Test]
        public void TestUnrapStructPassingInvalidType()
        {
            IntSchema schema = new IntSchema { AllowNull = true, DefaultValue = 100, PossibleValues = new int?[] { 1, 2, 3 } };
            IValueSchema<object> wrapper = schema.Wrap();
            Assert.IsNotNull(wrapper);
            Assert.AreEqual(typeof(int?), wrapper.Type);
            Assert.Throws<InvalidCastException>(() =>
            {
                wrapper.UnwrapValueType<DateTime>();
            });
        }

        [Test]
        public void TestUnwrapObjectPassingInvalidObject()
        {
            StringSchema schema = new StringSchema();
            IValueSchema<object> wrapper = schema.Wrap();
            Assert.IsNotNull(wrapper);
            Assert.AreEqual(typeof(string), wrapper.Type);
            Assert.Throws<InvalidCastException>(() =>
            {
                wrapper.UnwrapRefType<ArrayList>();
            });
        }

    }
}
