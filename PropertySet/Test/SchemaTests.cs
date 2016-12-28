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
    public class SchemaTests
    {
        [Test]
        public void TestStringSchemaValidValue()
        {
            StringSchema schema = new StringSchema();
            schema.Validate("hello");
        }

        [Test]
        public void TestStringSchemaSetInvalidValueType()
        {
            StringSchema schema=new StringSchema();
            Assert.Throws<PropertyValidationException>(()=> {
                schema.Validate(1);
            });
            
        }

        [Test]
        public void TestStringSchemaMaxLengthValid()
        {
            StringSchema schema = new StringSchema {MaxLength = 5};
            schema.Validate(string.Empty);
            schema.Validate("1");
            schema.Validate("12");
            schema.Validate("123");
            schema.Validate("1234");
            schema.Validate("12345");
            
        }
        [Test]
        public void TestStringSchemaMaxLengthInvalid()
        {
            StringSchema schema = new StringSchema { MaxLength = 5 };
            schema.Validate(string.Empty);
            schema.Validate("1");
            schema.Validate("12");
            schema.Validate("123");
            schema.Validate("1234");
            schema.Validate("12345");
            Assert.Throws<PropertyValidationException>(()=> { schema.Validate("123456"); });
            
        }

        [Test]
        public void TestStringSchemaMinLengthValid()
        {
            StringSchema schema = new StringSchema { MinLength = 2 };
            schema.Validate("12");
            schema.Validate("123");
            schema.Validate("1234");
            schema.Validate("12345");
            schema.Validate("123456");
        }

        [Test]
        public void TestStringSchemaMinLengthInvalidEmpty()
        {
            StringSchema schema = new StringSchema { MinLength = 2 };
            Assert.Throws<PropertyValidationException>(() => { schema.Validate(string.Empty); });
        }

        [Test]
        public void TestStringSchemaMinLengthInvalidOne()
        {
            StringSchema schema = new StringSchema { MinLength = 2 };
            Assert.Throws<PropertyValidationException>(() => { schema.Validate("1"); });
            
        }

        [Test]
        public void TestStringSchemaMinMaxLengthValid()
        {
            StringSchema schema = new StringSchema { MinLength = 2, MaxLength = 5};
            schema.Validate("12");
            schema.Validate("123");
            schema.Validate("1234");
            schema.Validate("12345");
        }

        [Test]
        public void TestStringSchemaMinMaxLengthInvalidMin()
        {
            StringSchema schema = new StringSchema { MinLength = 2, MaxLength = 5 };
            Assert.Throws<PropertyValidationException>(() => { schema.Validate("1"); });
        }
        [Test]
        public void TestStringSchemaMinMaxLengthInvalidMax()
        {
            StringSchema schema = new StringSchema { MinLength = 2, MaxLength = 5};
            schema.Validate("12");
            schema.Validate("123");
            schema.Validate("1234");
            schema.Validate("12345");
            Assert.Throws<PropertyValidationException>(() => { schema.Validate("123456"); });
            
        }

        [Test]
        public void TestStringSchemaDefaultValue()
        {
            new StringSchema {DefaultValue = "123"};
        }


        [Test]
        public void TestStringSchemaDefaultValueInvalidMin()
        {
            Assert.Throws<PropertyValidationException>(() =>
            {
                new StringSchema { MinLength = 2, MaxLength = 5, DefaultValue = "1" };
            });
        }
        [Test]
        public void TestStringSchemaDefaultValueInvalidMax()
        {
            Assert.Throws<PropertyValidationException>(() => {
                new StringSchema { MinLength = 2, MaxLength = 5, DefaultValue = "123456" }; });
            
        }
        [Test]
        public void TestStringSchemaDefaultValueInAllowedValues()
        {
            new StringSchema
            {
                PossibleValues = new[] { "one", "two", "three", "four" },
                DefaultValue = "two"
            };
        }
        [Test]
        public void TestStringSchemaDefaultValueNotInAllowedValues()
        {
            Assert.Throws<PropertyValidationException>(() =>
            {
                new StringSchema
                {
                    PossibleValues = new[] { "one", "two", "three", "four" },
                    DefaultValue = "five"
                };
            });
        }

        [Test]
        public void TestStringSchemaAllowedValuesValid()
        {
            StringSchema schema = new StringSchema
                                      {
                                          PossibleValues = new[]{"one","two","three","four"}
                                      };
            schema.Validate("three");
        }

        [Test]
        public void TestStringSchemaAllowedValuesInvalid()
        {
            StringSchema schema = new StringSchema
            {
                PossibleValues = new[] { "one", "two", "three", "four" }
            };
            Assert.Throws<PropertyValidationException>(() =>
            {
                schema.Validate("five");
            });
        }

        [Test]
        public void TestStringSchemaAllowNull()
        {
            StringSchema schema = new StringSchema {AllowNull = true};
            schema.Validate(null);
        }

        [Test]
        public void TestStringSchemaNoNullAllowed()
        {
            StringSchema schema = new StringSchema { AllowNull = false };
            Assert.Throws<PropertyValidationException>(() =>
            {
                schema.Validate(null);
            });
        }

        [Test]
        public void TestStringSchemaEquality()
        {
            StringSchema s1=new StringSchema();
            StringSchema s2 = new StringSchema();  
            Assert.IsTrue(s1.Equals(s2));
            Assert.IsTrue(s2.Equals(s1));
            Assert.AreEqual(s1.GetHashCode(),s2.GetHashCode());
            s1=new StringSchema
            {
                DefaultValue = "two",
                MaxLength = 100,
                MinLength = 1,
                PossibleValues = new[] { "one", "two", "three", "four" }
            };
            s2 = new StringSchema
            {
                DefaultValue = "two",
                MaxLength = 100,
                MinLength = 1,
                PossibleValues = new[] { "one", "two", "three", "four" }
            };

            Assert.IsTrue(s1.Equals(s2));
            Assert.IsTrue(s2.Equals(s1));
            Assert.AreEqual(s1.GetHashCode(), s2.GetHashCode());

            s2 = new StringSchema
            {
                DefaultValue = "two",
                MaxLength = 100,
                MinLength = 1,
                PossibleValues = new[] { "one", "2", "three", "four" }
            };
            Assert.IsFalse(s1.Equals(s2));
            Assert.IsFalse(s2.Equals(s1));
            Assert.AreNotEqual(s1.GetHashCode(), s2.GetHashCode());

            s2=new StringSchema{AllowNull = true};

            Assert.IsFalse(s1.Equals(s2));
            Assert.IsFalse(s2.Equals(s1));
            Assert.AreNotEqual(s1.GetHashCode(), s2.GetHashCode());


        }


    }
}
