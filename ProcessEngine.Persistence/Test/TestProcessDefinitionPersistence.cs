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
using NUnit.Framework;

namespace KlaudWerk.ProcessEngine.Persistence.Test
{
    [TestFixture]
    public class TestProcessDefinitionPersistence : TestProcessDefinitionPersistenceBase
    {
        [SetUp]
        public void SetUp()
        {

        }

        [TearDown]
        public void TearDown()
        {
            using (var ctx = new ProcessDbContext())
            {
                ctx.Database.ExecuteSqlCommand("delete from PROCESS_DEFINITION_ACCOUNT");
                ctx.Database.ExecuteSqlCommand("delete from ACCOUNTS");
                ctx.Database.ExecuteSqlCommand("delete from  PROCESS_DEFINITIONS");
            }
        }

        [Test]
        public void TestCreateAndListProcessDefinitions()
        {
            IProcessDefinitionPersisnenceService service=InstService();
            OnCreateAndListProcessDefinition(service);
        }

        [Test]
        public void TestCreateFlowWithAssociatedSecurityAccounts()
        {
            IProcessDefinitionPersisnenceService service=InstService();
            OnCreateFlowWithAssociatedSecutityAccounts(service,new[]
                {
                    new AccountData
                    {
                        AccountType = 1,
                        Id = Guid.NewGuid(),
                        Name = "Underwriters",
                        SourceSystem = "ActiveDirectory"
                    },
                    new AccountData
                    {
                        AccountType = 1,
                        Id = Guid.NewGuid(),
                        Name = "Modeler",
                        SourceSystem = "ActiveDirectory"
                    },
                }
                );
        }
        [Test]
        public void TestCreatePorcessUsingExistingAccounts()
        {
            IProcessDefinitionPersisnenceService service=InstService();
            OnCreateProcessUsingExistingAccounts(service);
        }
        [Test]
        public void TestChangePorcessDefinitionStatus()
        {
            IProcessDefinitionPersisnenceService service=InstService();
            OnChangeProcessDefinitionStatus(service);
        }
        [Test]
        public void TestTryFindFlow()
        {
            IProcessDefinitionPersisnenceService service=InstService();
            OnTryFindFlow(service);
        }
        [Test]
        public void TestFindNonExistingFlowShouldReturnFalse()
        {
            IProcessDefinitionPersisnenceService service=InstService();
            OnFindNonExistingFlowShouldReturnFalse(service);
        }
        [Test]
        public void SaveDefinitionWithTheSameMd5SameVersionShouldFail()
        {
            IProcessDefinitionPersisnenceService service=InstService();
            OnSaveDefinitionWithTheSameMd5ShouldFail(service);
        }
        [Test]
        public void ShouldBePossibleToUpdateNameAndDescription()
        {
            IProcessDefinitionPersisnenceService service=InstService();
            OnPossibleToUpdateNameAndDescription(service);
        }

        protected override IProcessDefinitionPersisnenceService InstService()
        {
            return new ProcessDefinitionPersisnenceService();
        }
    }
}