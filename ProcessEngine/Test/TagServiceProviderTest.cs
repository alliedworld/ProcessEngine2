using System;
using KlaudWerk.ProcessEngine.Definition;
using KlaudWerk.ProcessEngine.Runtime;
using Moq;
using NUnit.Framework;

namespace KlaudWerk.ProcessEngine.Test
{
    [TestFixture]
    public class TagServiceProviderTest
    {

        [Test]
        public void TestIocProviderShouldCallMethod()
        {

            Mock<ITageService> mService = new Mock<ITageService>();

            TagDefinition td=new TagDefinition
            {
                DisplayName = "Line Of Business",
                Id = "LOB",
                Handler = new StepHandlerDefinition
                {
                    StepHandlerType = StepHandlerTypeEnum.IoC,
                    IocName = "Method"
                }
            };
            mService.SetupGet(m => m.DisplayName).Returns(td.DisplayName);
            mService.SetupGet(m => m.Name).Returns(td.Id);
            mService.Setup(m=>m.GetValues(It.IsAny<IProcessRuntimeEnvironment>()))
                .Returns(new string[]{"1","2"}).Verifiable();
            ITagServiceProvider provider = new TagHandlerProvider(name =>
            {
                Assert.AreEqual("LOB",name);
                return mService.Object;
            });
            var tagService = provider.GetTagService(td);
            Assert.IsNotNull(tagService);
            Assert.AreEqual("LOB",tagService.Name);
            Assert.AreEqual("Line Of Business",tagService.DisplayName);
            var values = tagService.GetValues(new ProcessRuntimeEnvironment(null, null));
            Assert.IsNotNull(values);
            Assert.AreEqual(2,values.Count);
            mService.Verify(m=>m.GetValues(It.IsAny<IProcessRuntimeEnvironment>()),Times.Once);

        }

        [Test]
        public void TestScriptProviderShoulExecuteTheScript()
        {
            TagDefinition td=new TagDefinition
            {
                DisplayName = "Line Of Business",
                Id = "LOB",
                Handler = new StepHandlerDefinition
                {
                    StepHandlerType = StepHandlerTypeEnum.Script,
                    Script = new ScriptDefinition
                    {
                        Lang = ScriptLanguage.CSharpScript,
                        Script = "return new string[]{\"1\",\"2\"};"
                    }
                }
            };
            ITagServiceProvider provider = new TagHandlerProvider(name =>
            {
                throw new NotImplementedException();
            });

            var tagService = provider.GetTagService(td);
            Assert.IsNotNull(tagService);
            Assert.AreEqual("LOB",tagService.Name);
            Assert.AreEqual("Line Of Business",tagService.DisplayName);
            var values = tagService.GetValues(new ProcessRuntimeEnvironment(null, null));
            Assert.IsNotNull(values);
            Assert.AreEqual(2,values.Count);
        }
    }
}