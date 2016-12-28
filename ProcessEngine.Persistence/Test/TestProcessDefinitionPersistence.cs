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
using System.Collections.Generic;
using System.Linq;
using KlaudWerk.ProcessEngine.Builder;
using KlaudWerk.ProcessEngine.Definition;
using KlaudWerk.ProcessEngine.Test;
using NUnit.Framework;

namespace KlaudWerk.ProcessEngine.Persistence.Test
{
    [TestFixture]
    public class TestProcessDefinitionPersistence
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
            ProcessDefinitionPersisnenceService service=new ProcessDefinitionPersisnenceService();
            var processDefinition = BuildProcessdefinition();
            Assert.IsNotNull(processDefinition);
            service.Create(processDefinition,ProcessDefStatusEnum.Active, 1);
            IReadOnlyList<ProcessDefinitionDigest> flows = service.LisAlltWorkflows();
            Assert.IsNotNull(flows);
            Assert.AreEqual(1,flows.Count);
            Assert.AreEqual(processDefinition.Name,flows[0].Name);
        }

        [Test]
        public void TestCreateFlowWithAssociatedSecurityAccounts()
        {
            ProcessDefinitionPersisnenceService service=new ProcessDefinitionPersisnenceService();
            var processDefinition = BuildProcessdefinition();
            Assert.IsNotNull(processDefinition);
            AccountData[] accounts=new[]
            {
                new AccountData
                {
                    AccountType = 1,
                    Id=Guid.NewGuid(),
                    Name = "Underwriters",
                    SourceSystem = "ActiveDirectory"
                },
                new AccountData
                {
                    AccountType = 1,
                    Id=Guid.NewGuid(),
                    Name = "Modeler",
                    SourceSystem = "ActiveDirectory"
                },

            };
            service.Create(processDefinition,ProcessDefStatusEnum.Active, 1, accounts);
            ProcessDefStatusEnum status;
            AccountData[] pdAccounts;
            Assert.IsTrue(service.TryFind(processDefinition.Id,1,out processDefinition, out status,out pdAccounts));
            Assert.IsNotNull(pdAccounts);
            Assert.AreEqual(2,pdAccounts.Length);
            Assert.IsNotNull(pdAccounts.FirstOrDefault(d=>d.Id==accounts[0].Id));
            Assert.IsNotNull(pdAccounts.FirstOrDefault(d=>d.Id==accounts[1].Id));

        }

        [Test]
        public void TestCreatePorcessUsingExistingAccounts()
        {
            ProcessDefinitionPersisnenceService service=new ProcessDefinitionPersisnenceService();
            var processDefinition = BuildProcessdefinition();
            Assert.IsNotNull(processDefinition);
            AccountData[] accounts=new[]
            {
                new AccountData
                {
                    AccountType = 1,
                    Id=Guid.NewGuid(),
                    Name = "Underwriters",
                    SourceSystem = "ActiveDirectory"
                },
                new AccountData
                {
                    AccountType = 1,
                    Id=Guid.NewGuid(),
                    Name = "Modeler",
                    SourceSystem = "ActiveDirectory"
                },

            };
            service.CreateAccounts(accounts);
            service.Create(processDefinition,ProcessDefStatusEnum.Active, 1, accounts);
            ProcessDefStatusEnum status;
            AccountData[] pdAccounts;
            Assert.IsTrue(service.TryFind(processDefinition.Id,1,out processDefinition, out status,out pdAccounts));
            Assert.IsNotNull(pdAccounts);
            Assert.AreEqual(2,pdAccounts.Length);
            Assert.IsNotNull(pdAccounts.FirstOrDefault(d=>d.Id==accounts[0].Id));
            Assert.IsNotNull(pdAccounts.FirstOrDefault(d=>d.Id==accounts[1].Id));
        }

        public void TestAddRemoveSecurityAccounts()
        {

        }
        [Test]
        public void TestChangePorcessDefinitionStatus()
        {
            ProcessDefinitionPersisnenceService service=new ProcessDefinitionPersisnenceService();
            var processDefinition = BuildProcessdefinition();
            Assert.IsNotNull(processDefinition);
            service.Create(processDefinition,ProcessDefStatusEnum.Active, 1);
            IReadOnlyList<ProcessDefinitionDigest> flows = service.LisAlltWorkflows();
            Assert.IsNotNull(flows);
            Assert.AreEqual(1,flows.Count);
            flows = service.ActivetWorkflows();
            Assert.IsNotNull(flows);
            Assert.AreEqual(1,flows.Count);
            Guid id = flows[0].Id;
            Assert.IsTrue(service.SetStatus(id, 1, ProcessDefStatusEnum.NotActive));
            flows = service.ActivetWorkflows();
            Assert.IsNotNull(flows);
            Assert.AreEqual(0,flows.Count);
            flows = service.LisAlltWorkflows();
            Assert.IsNotNull(flows);
            Assert.AreEqual(1,flows.Count);
        }

        [Test]
        public void TestTryFindFlow()
        {
            ProcessDefinitionPersisnenceService service=new ProcessDefinitionPersisnenceService();
            var processDefinition = BuildProcessdefinition();
            Assert.IsNotNull(processDefinition);
            service.Create(processDefinition,ProcessDefStatusEnum.Active, 1);
            ProcessDefinition definition;
            ProcessDefStatusEnum stat;
            AccountData[] pdAccounts;
            Assert.IsTrue(service.TryFind(processDefinition.Id,1,out definition,out stat, out pdAccounts));
            Assert.IsNotNull(definition);
            Md5CalcVisitor visitor=new Md5CalcVisitor();
            processDefinition.Accept(visitor);
            string md5 = visitor.CalculateMd5();
            visitor.Reset();
            definition.Accept(visitor);
            string restoredMd5 = visitor.CalculateMd5();
            Assert.AreEqual(md5,restoredMd5);
        }

        [Test]
        public void TestFindNonExistingFlowShouldReturnFalse()
        {
            ProcessDefinitionPersisnenceService service=new ProcessDefinitionPersisnenceService();
            ProcessDefStatusEnum status;
            ProcessDefinition def;
            AccountData[] pdAccounts;
            Assert.IsFalse(service.TryFind(Guid.NewGuid(),1, out def, out status, out pdAccounts));
            Assert.IsNull(def);
            Assert.AreEqual(ProcessDefStatusEnum.NotActive,status);
        }

        [Test]
        public void SaveDefinitionWithTheSameMd5SameVersionShouldFail()
        {
            ProcessDefinitionPersisnenceService service=new ProcessDefinitionPersisnenceService();
            var processDefinition = BuildProcessdefinition();
            service.Create(processDefinition,ProcessDefStatusEnum.Active, 1);
            processDefinition.Id=Guid.NewGuid();
            Assert.Throws<System.Data.Entity.Infrastructure.DbUpdateException>(() =>
            {
                service.Create(processDefinition,ProcessDefStatusEnum.Active, 1);
            });
        }

        [Test]
        public void ShouldBePossibleToUpdateNameAndDescription()
        {
            ProcessDefinitionPersisnenceService service=new ProcessDefinitionPersisnenceService();
            var processDefinition = BuildProcessdefinition();
            service.Create(processDefinition,ProcessDefStatusEnum.Active, 1);
            ProcessDefinition definition;
            ProcessDefStatusEnum stat;
            AccountData[] pdAccounts;
            Assert.IsTrue(service.TryFind(processDefinition.Id,1,out definition,out stat, out pdAccounts));
            service.Update(processDefinition.Id, 1, pd => { pd.Name = "New Name"; });
            var flows=service.ActivetWorkflows();
            Assert.IsNotNull(flows);
            Assert.AreEqual("New Name",flows[0].Name);

        }

        private ProcessDefinition BuildProcessdefinition()
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id: "com.klaudwerk.workflow.renewal",
                name: "Renewal", description: "Policy Renewal");
            IReadOnlyList<ProcessValidationResult> result;
            bool isValid=builder.Variables().Name("PolicyNumber").Type(VariableTypeEnum.String).Done()
                .Start("s_1").SetName("Start").Vars().Name("PolicyNumber").OnExit().Done().Done()
                .End("e_1").SetName("End Process").Done()
                .Link().From("s_1").To("e_1").Name("finish").Done()
                .TryValidate(out result);
            Assert.IsTrue(isValid);
            return builder.Build();
        }
    }
}