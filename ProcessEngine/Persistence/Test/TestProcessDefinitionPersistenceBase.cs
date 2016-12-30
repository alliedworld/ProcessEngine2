using System;
using System.Collections.Generic;
using System.Linq;
using KlaudWerk.ProcessEngine.Builder;
using KlaudWerk.ProcessEngine.Definition;
using KlaudWerk.ProcessEngine.Test;
using NUnit.Framework;

namespace KlaudWerk.ProcessEngine.Persistence.Test
{
    /// <summary>
    /// Base test class
    /// </summary>
    public abstract class TestProcessDefinitionPersistenceBase
    {

        protected virtual void OnCreateAndListProcessDefinition(IProcessDefinitionPersisnenceService service)
        {

            var processDefinition=OnCreateProcessDefinition(service);
            IReadOnlyList<ProcessDefinitionDigest> flows = service.LisAlltWorkflows();
            Assert.IsNotNull(flows);
            Assert.AreEqual(1, flows.Count);
            Assert.AreEqual(processDefinition.Name, flows[0].Name);
        }

        protected virtual ProcessDefinition OnCreateProcessDefinition(IProcessDefinitionPersisnenceService service)
        {
            var processDefinition = BuildProcessdefinition();
            Assert.IsNotNull(processDefinition);
            service.Create(processDefinition, ProcessDefStatusEnum.Active, 1);
            return processDefinition;

        }
        protected virtual void OnCreateFlowWithAssociatedSecutityAccounts(IProcessDefinitionPersisnenceService service,AccountData[] accounts)
        {
            var processDefinition = BuildProcessdefinition();
            Assert.IsNotNull(processDefinition);
            service.Create(processDefinition, ProcessDefStatusEnum.Active, 1, accounts);
            ProcessDefStatusEnum status;
            AccountData[] pdAccounts;
            Assert.IsTrue(service.TryFind(processDefinition.Id, 1, out processDefinition, out status, out pdAccounts));
            Assert.IsNotNull(pdAccounts);
            Assert.AreEqual(2, pdAccounts.Length);
            Assert.IsNotNull(pdAccounts.FirstOrDefault(d => d.Id == accounts[0].Id));
            Assert.IsNotNull(pdAccounts.FirstOrDefault(d => d.Id == accounts[1].Id));
        }


        protected virtual void OnCreateProcessUsingExistingAccounts(IProcessDefinitionPersisnenceService service)
        {
            var processDefinition = BuildProcessdefinition();
            Assert.IsNotNull(processDefinition);
            AccountData[] accounts = new[]
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
            };
            service.CreateAccounts(accounts);
            service.Create(processDefinition, ProcessDefStatusEnum.Active, 1, accounts);
            ProcessDefStatusEnum status;
            AccountData[] pdAccounts;
            Assert.IsTrue(service.TryFind(processDefinition.Id, 1, out processDefinition, out status, out pdAccounts));
            Assert.IsNotNull(pdAccounts);
            Assert.AreEqual(2, pdAccounts.Length);
            Assert.IsNotNull(pdAccounts.FirstOrDefault(d => d.Id == accounts[0].Id));
            Assert.IsNotNull(pdAccounts.FirstOrDefault(d => d.Id == accounts[1].Id));
        }

        public void TestAddRemoveSecurityAccounts()
        {

        }

        protected virtual void OnChangeProcessDefinitionStatus(IProcessDefinitionPersisnenceService service)
        {
            var processDefinition = BuildProcessdefinition();
            Assert.IsNotNull(processDefinition);
            service.Create(processDefinition, ProcessDefStatusEnum.Active, 1);
            IReadOnlyList<ProcessDefinitionDigest> flows = service.LisAlltWorkflows();
            Assert.IsNotNull(flows);
            Assert.AreEqual(1, flows.Count);
            flows = service.ActivetWorkflows();
            Assert.IsNotNull(flows);
            Assert.AreEqual(1, flows.Count);
            Guid id = flows[0].Id;
            Assert.IsTrue(service.SetStatus(id, 1, ProcessDefStatusEnum.NotActive));
            flows = service.ActivetWorkflows();
            Assert.IsNotNull(flows);
            Assert.AreEqual(0, flows.Count);
            flows = service.LisAlltWorkflows();
            Assert.IsNotNull(flows);
            Assert.AreEqual(1, flows.Count);
        }

        protected virtual void OnTryFindFlow(IProcessDefinitionPersisnenceService service)
        {
            var processDefinition = BuildProcessdefinition();
            Assert.IsNotNull(processDefinition);
            service.Create(processDefinition, ProcessDefStatusEnum.Active, 1);
            ProcessDefinition definition;
            ProcessDefStatusEnum stat;
            AccountData[] pdAccounts;
            Assert.IsTrue(service.TryFind(processDefinition.Id, 1, out definition, out stat, out pdAccounts));
            Assert.IsNotNull(definition);
            Md5CalcVisitor visitor = new Md5CalcVisitor();
            processDefinition.Accept(visitor);
            string md5 = visitor.CalculateMd5();
            visitor.Reset();
            definition.Accept(visitor);
            string restoredMd5 = visitor.CalculateMd5();
            Assert.AreEqual(md5, restoredMd5);
        }

        protected virtual void OnFindNonExistingFlowShouldReturnFalse(IProcessDefinitionPersisnenceService service)
        {
            ProcessDefStatusEnum status;
            ProcessDefinition def;
            AccountData[] pdAccounts;
            Assert.IsFalse(service.TryFind(Guid.NewGuid(), 1, out def, out status, out pdAccounts));
            Assert.IsNull(def);
            Assert.AreEqual(ProcessDefStatusEnum.NotActive, status);
        }

        protected virtual void OnSaveDefinitionWithTheSameMd5ShouldFail(IProcessDefinitionPersisnenceService service)
        {
            var processDefinition = BuildProcessdefinition();
            service.Create(processDefinition, ProcessDefStatusEnum.Active, 1);
            processDefinition.Id = Guid.NewGuid();
            Assert.Throws<ArgumentException>(() =>
            {
                service.Create(processDefinition, ProcessDefStatusEnum.Active, 1);
            });
        }

        protected virtual void OnPossibleToUpdateNameAndDescription(IProcessDefinitionPersisnenceService service)
        {
            var processDefinition = BuildProcessdefinition();
            service.Create(processDefinition, ProcessDefStatusEnum.Active, 1);
            ProcessDefinition definition;
            ProcessDefStatusEnum stat;
            AccountData[] pdAccounts;
            Assert.IsTrue(service.TryFind(processDefinition.Id, 1, out definition, out stat, out pdAccounts));
            service.Update(processDefinition.Id, 1, pd => { pd.Name = "New Name"; });
            var flows = service.ActivetWorkflows();
            Assert.IsNotNull(flows);
            Assert.AreEqual("New Name", flows[0].Name);
        }

        protected ProcessDefinition BuildProcessdefinition()
        {
            return BuildProcessdefinition(id: "com.klaudwerk.workflow.renewal",
                name: "Renewal", description: "Policy Renewal");
        }

        protected ProcessDefinition BuildProcessdefinition(string id,string name,string description)
        {
            var factory = new ProcessBuilderFactory();
            var builder = factory.CreateProcess(id:id,
                name: name, description: description);
            IReadOnlyList<ProcessValidationResult> result;
            bool isValid=builder.Variables().Name("PolicyNumber").Type(VariableTypeEnum.String).Done()
                .Start("s_1").SetName("Start").Vars().Name("PolicyNumber").OnExit().Done().Done()
                .End("e_1").SetName("End Process").Done()
                .Link().From("s_1").To("e_1").Name("finish").Done()
                .TryValidate(out result);
            Assert.IsTrue(isValid);
            return builder.Build();
        }

        protected abstract IProcessDefinitionPersisnenceService InstService();
    }
}