using NUnit.Framework;

namespace KlaudWerk.ProcessEngine.Test
{
    [TestFixture]
    public class TagBuilderTest
    {
        [Test]
        public void CreateTagBuilder()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "p_001", name: "Renewal", description: "Policy Renewal");
            var tagBuilder= builder.Tag("Lob").Name("Line of Business");
            Assert.IsNotNull(tagBuilder);
            Assert.IsNotNull(builder.Tags);
            Assert.AreEqual(1,builder.Tags.Count);
            Assert.AreEqual("Lob",builder.Tags[0].Id);
            Assert.AreEqual("Line of Business",builder.Tags[0].DisplayName);
            Assert.AreEqual(builder,tagBuilder.Done());
        }

        [Test]
        public void CreateRetrieveTagBuilder()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "p_001", name: "Renewal", description: "Policy Renewal");
            var tagBuilder= builder.Tag("Lob");
            Assert.IsNotNull(tagBuilder);
            var retrieved = builder.Tag("Lob").Name("Line Of Business");
            Assert.IsNotNull(retrieved);
            Assert.IsNotNull(builder.Tags);
            Assert.AreEqual(1,builder.Tags.Count);
            Assert.AreEqual(tagBuilder,retrieved);
            Assert.AreEqual(builder,tagBuilder.Done());
        }

        [Test]
        public void CreateRetrieveRemoveTag()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "p_001", name: "Renewal", description: "Policy Renewal");
            var tagBuilder= builder.Tag("Lob");
            Assert.IsNotNull(tagBuilder);
            var newTagBuilder = builder.Tag("NewTag");
            Assert.IsNotNull(builder.Tags);
            Assert.AreEqual(2,builder.Tags.Count);
            Assert.AreNotEqual(tagBuilder,newTagBuilder);
            newTagBuilder.Remove();
            Assert.AreEqual(1,builder.Tags.Count);
            Assert.AreEqual(tagBuilder,builder.Tags[0]);
            Assert.AreEqual(builder,tagBuilder.Done());

        }

        [Test]
        public void CreateTagBuilderHandler()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "p_001", name: "Renewal", description: "Policy Renewal");
            var tagBuilder= builder.Tag("Lob");
            var handler=tagBuilder.Handler();
            Assert.IsNotNull(handler);
            var iochandler = handler.IocService("LineOfBusinessService");
            Assert.AreEqual(handler,iochandler);
            Assert.IsNotNull(tagBuilder.TagHandler);
            Assert.AreEqual(handler,tagBuilder.TagHandler);
        }

    }
}