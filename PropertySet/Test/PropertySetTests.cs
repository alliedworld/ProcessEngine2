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
using NUnit.Framework;

namespace Klaudwerk.PropertySet.Test
{
    [TestFixture]
    public class PropertySetTests
    {

        [Test]
        public void TestPropertyAddIntNoSchema()
        {
            IPropertySetCollection provider = GetValueProvider();
            provider.Set("n",(int?)100);
            provider.Set("n","hello");

            provider.Add("val",100,new IntSchema());
            provider.Add("val1", "100", new StringSchema());

            int? result=provider.Get<int?>("val");
            Assert.IsNotNull(result);
            Assert.AreEqual(100,result.Value);
            provider["val"] = 300;
            object val = provider["val"];
            Assert.IsNotNull(val);
            result = val as int?;
            Assert.IsNotNull(result);
            Assert.AreEqual(300, result.Value);
        }

        [Test]
        public void TestPropertyAddIntWithSchemaDefaultVal()
        {
            IPropertySetCollection provider = GetValueProvider();
            provider.Add("val1", new IntSchema
            {
                DefaultValue = 40
            });
            int? result = provider.Get<int?>("val1");
            Assert.IsNotNull(result);
            Assert.AreEqual(40,result.Value);
            object val = provider["val1"];
            Assert.IsNotNull(val);
            result = val as int?;
            Assert.IsNotNull(result);
            Assert.AreEqual(40, result.Value);
        }

        [Test]
        public void TestPropertyAddIntWithSchemaValidation()
        {

        }

        [Test]
        public void TestPropertyAddIntDefaultSchemaAddValidation()
        {
        }

        protected virtual IPropertySetCollection GetValueProvider()
        {
            return new ValueSetCollectionTest.MockPropertySetCollection(new PropertySchemaSet(new PropertySchemaFactory()));
        }
    }
}
