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
using Klaudwerk.PropertySet;
using NUnit.Framework;

namespace KlaudWerk.ProcessEngine.Persistence.Test
{
    [TestFixture]
    public class TestCollectionPersistance
    {
        [SetUp]
        public void SetUp()
        {

        }

        [TearDown]
        public void TearDown()
        {
            // remove collection data
            using (var ctx = new ProcessDbContext())
            {
                try
                {
                    ctx.Database.ExecuteSqlCommand("delete from PROPERTY_SCHEMA_ELEMENTS");
                    ctx.Database.ExecuteSqlCommand("delete from PROPERTY_ELEMENTS");
                    ctx.Database.ExecuteSqlCommand("delete from PROPERTIES_COLLECTIONS");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        [Test]
        public void TestSavePropertyCollection()
        {
            PropertySchemaSet schemaSet=new PropertySchemaSet(new PropertySchemaFactory());
            PropertySetCollection collection = new PropertySetCollection(schemaSet)
            {
                {"V_string", "hello", schemaSet.SchemaFactory.Create(typeof(string))},
                {"V_int", 100, schemaSet.SchemaFactory.Create(typeof(int?))},
                {"V_bool", true, schemaSet.SchemaFactory.Create(typeof(bool?))},
                {"V_date", DateTime.Now, schemaSet.SchemaFactory.Create(typeof(DateTime))}
            };
            PropertySetPersistenceService persistenceService=new PropertySetPersistenceService();
            Guid id = Guid.NewGuid();
            persistenceService.SaveCollection(id,collection);
        }

        [Test]
        public void TestLoadSavedPropertyCollection()
        {
            PropertySchemaSet schemaSet=new PropertySchemaSet(new PropertySchemaFactory());
            DateTime date = DateTime.Today;
            PropertySetCollection collection = new PropertySetCollection(schemaSet)
            {
                {"V_string", "hello", schemaSet.SchemaFactory.Create(typeof(string))},
                {"V_int", 100, schemaSet.SchemaFactory.Create(typeof(int?))},
                {"V_bool", true, schemaSet.SchemaFactory.Create(typeof(bool?))},
                {"V_date", date, schemaSet.SchemaFactory.Create(typeof(DateTime))}
            };
            PropertySetPersistenceService persistenceService=new PropertySetPersistenceService();
            Guid id = Guid.NewGuid();
            persistenceService.SaveCollection(id,collection);
            IPropertySetCollection retrieved=persistenceService.FindCollection(id);
            Assert.IsNotNull(retrieved);
            Assert.AreEqual("hello",retrieved.Get<string>("V_string"));
            Assert.AreEqual(100,retrieved.Get<int>("V_int"));
            Assert.AreEqual(date,retrieved.Get<DateTime>("V_date"));
            Assert.IsTrue(retrieved.Get<bool>("V_bool"));

        }

        [Test]
        public void TestUpdatePropertyCollection()
        {
            PropertySchemaSet schemaSet = new PropertySchemaSet(new PropertySchemaFactory());
            DateTime date = DateTime.Today;
            PropertySetCollection collection = new PropertySetCollection(schemaSet);
            collection.Add("v_int", new IntSchema());
            collection.Add("v_string", new StringSchema());
            collection.Add("v_date", new DateTimeSchema());
            PropertySetPersistenceService persistenceService = new PropertySetPersistenceService();
            Guid id = Guid.NewGuid();
            persistenceService.SaveCollection(id, collection);
            IPropertySetCollection retrieved = persistenceService.FindCollection(id);
            Assert.IsNotNull(retrieved);
            Assert.IsNull(retrieved.Get<int?>("v_int"));
            Assert.AreEqual(DateTime.MinValue, retrieved.Get<DateTime?>("v_date"));
            Assert.IsNull(retrieved.Get<string>("v_string"));
            retrieved.Set("v_int",(int?)100);
            retrieved.Set("v_string","hello");
            retrieved.Set("v_date",(DateTime?)DateTime.Today);
            persistenceService.SaveCollection(id, retrieved);
            IPropertySetCollection retrieved1 = persistenceService.FindCollection(id);
            Assert.IsNotNull(retrieved1);
            Assert.AreEqual(100, retrieved1.Get<int?>("v_int"));
            Assert.AreEqual(DateTime.Today, retrieved1.Get<DateTime?>("v_date"));
            Assert.AreEqual("hello", retrieved1.Get<string>("v_string"));
        }

    }
}