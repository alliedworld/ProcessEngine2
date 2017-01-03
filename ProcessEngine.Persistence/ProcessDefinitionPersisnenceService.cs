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
using KlaudWerk.ProcessEngine.Definition;
using KlaudWerk.ProcessEngine.Runtime;
using Newtonsoft.Json;

namespace KlaudWerk.ProcessEngine.Persistence
{
    /// <summary>
    /// The service that saves, list, remove and load Workflow Definitions
    /// </summary>
    public class ProcessDefinitionPersisnenceService : IProcessDefinitionPersisnenceService
    {
        /// <summary>
        /// List all Available workflow
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<ProcessDefinitionDigest> LisAlltWorkflows(params AccountData[] accounts)
        {
            using (var ctx = new ProcessDbContext())
            {
                return ctx.ProcessDefinition.Select(
                    r => new ProcessDefinitionDigest
                        {
                            Id=r.Id,
                            FlowId = r.FlowId,
                            Name = r.Name,
                            Description = r.Description,
                            Md5 = r.Md5,
                            LastUpdated = r.LastModified,
                            Status = (ProcessDefStatusEnum)r.Status,
                            Version = r.Version
                        })
                    .OrderBy(r=>r.Name).ToList();
            }
        }
        /// <summary>
        /// List of all active workflow
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<ProcessDefinitionDigest> ActivetWorkflows(params AccountData[] accounts)
        {
            using (var ctx = new ProcessDbContext())
            {
                return ctx.ProcessDefinition
                    .Where(r=>r.Status==(int)ProcessDefStatusEnum.Active)
                    .Select(
                        r => new ProcessDefinitionDigest
                        {
                            Id=r.Id,
                            FlowId = r.FlowId,
                            Name = r.Name,
                            Description = r.Description,
                            Md5 = r.Md5,
                            LastUpdated = r.LastModified,
                            Status = (ProcessDefStatusEnum)r.Status,
                            Version = r.Version
                        }
                    )
                    .OrderBy(r=>r.Name).ToList();
            }
        }

        /// <summary>
        /// Save process definition
        /// </summary>
        /// <param name="definition"></param>
        /// <param name="status"></param>
        /// <param name="version"></param>
        /// <param name="accounts">Security accounts that have access to the flow</param>
        public void Create(ProcessDefinition definition,
            ProcessDefStatusEnum status,
            int version,params AccountData[] accounts)
        {
            Md5CalcVisitor visitor = new Md5CalcVisitor();
            definition.Accept(visitor);
            var accountsList = new List<ProcessDefinitionAccount>();
            ProcessDefinitionPersistence pd=new ProcessDefinitionPersistence
            {
                Id = definition.Id,
                FlowId=definition.FlowId,
                Version = version,
                Name = definition.Name,
                Description = definition.Description,
                LastModified = DateTime.UtcNow,
                Status = (int)status,
                Md5 = visitor.CalculateMd5(),
                JsonProcessDefinition = JsonConvert.SerializeObject(definition),
                Accounts = accountsList
            };
            using (var ctx = new ProcessDbContext())
            {
                SetupAccounts(ctx, ctx.ProcessDefinition.Add(pd), accounts);
                ctx.SaveChanges();
            }
        }

        private void SetupAccounts(ProcessDbContext ctx,
            ProcessDefinitionPersistence persistence,
            AccountData[] accounts)
        {
            accounts?.ToList().ForEach(a =>
            {
                var acc = ctx.Accounts.Find(a.Id) ?? ctx.Accounts.Add(a);
                ProcessDefinitionAccount pdAccount=new ProcessDefinitionAccount
                {
                    Account = acc,
                    ProcessDefinition = persistence,
                    AccountDataId = acc.Id,
                    ProcessDefinitionId = persistence.Id,
                    ProcessDefinitionVersion = persistence.Version
                };
                var pd=ctx.ProcessDefinitionAccounts.Add(pdAccount);
                persistence.Accounts.Add(pd);
            });
        }


        /// <summary>
        /// Set or update status
        /// </summary>
        /// <param name="id"></param>
        /// <param name="version"></param>
        /// <param name="status"></param>
        public bool SetStatus(Guid id, int version, ProcessDefStatusEnum status)
        {
            using (var ctx = new ProcessDbContext())
            {
                ProcessDefinitionPersistence pd = ctx.ProcessDefinition.Find(id, version);
                if (pd == null)
                    return false;
                pd.Status = (int) status;
                pd.LastModified=DateTime.UtcNow;
                ctx.SaveChanges();
                return true;
            }
        }

        /// <summary>
        /// Load and deserialize the process definition
        /// </summary>
        /// <param name="id"></param>
        /// <param name="version"></param>
        /// <param name="definition"></param>
        /// <param name="status"></param>
        /// <param name="accounts"></param>
        /// <returns></returns>
        public bool TryFind(Guid id, int version, out ProcessDefinition definition,
            out ProcessDefStatusEnum status,
            out AccountData[] accounts)
        {
            using (var ctx = new ProcessDbContext())
            {
                ProcessDefinitionPersistence pd = ctx.ProcessDefinition.Find(id, version);
                if (pd == null)
                {
                    definition = null;
                    accounts=new AccountData[]{};
                    status = ProcessDefStatusEnum.NotActive;
                    return false;
                }
                definition = JsonConvert.DeserializeObject<ProcessDefinition>(pd.JsonProcessDefinition);
                status = (ProcessDefStatusEnum) pd.Status;
                accounts = pd.Accounts?.Select(a => new AccountData
                {
                    Id = a.AccountDataId,
                    Name = a.Account.Name,
                    SourceSystem = a.Account.SourceSystem
                }).ToArray();
                return true;
            }
        }

        /// <summary>
        /// Find the definition
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public IEnumerable<ProcessDefinitionPersistence> Find(ProcessDbContext ctx,
            Func<ProcessDefinitionPersistence, bool> comparer)
        {
            return ctx.ProcessDefinition.Where(p => comparer(p)).ToList();
        }
        /// <summary>
        /// Update Process Definition
        /// </summary>
        /// <param name="id"></param>
        /// <param name="version"></param>
        /// <param name="action"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Update(Guid id, int version, Action<ProcessDefinitionPersistenceBase> action)
        {
            using (var ctx = new ProcessDbContext())
            {
                ProcessDefinitionPersistence pd = ctx.ProcessDefinition.Find(id, version);
                if (pd == null)
                    throw new ArgumentException($"Process Definition id={id} version={version} not found.");
                action(pd);
                ctx.SaveChanges();
            }
        }

        /// <summary>
        /// Create Security Account Records
        /// </summary>
        /// <param name="accounts"></param>
        public void CreateAccounts(params AccountData[] accounts)
        {
            using (var ctx = new ProcessDbContext())
            {
                accounts.ToList().ForEach(a=>ctx.Accounts.Add(a));
                ctx.SaveChanges();
            }
        }

        /// <summary>
        /// Save Runtime Process
        /// </summary>
        /// <param name="process"></param>
        public void SaveRuntimeProcess(ProcessRuntime process)
        {
            using (var ctx = new ProcessDbContext())
            {
                var persistedProcess = ctx.Process.Find(process.Id);
                if (persistedProcess == null)
                {
                    var processDefinitionPersistence =
                        ctx.ProcessDefinition.SingleOrDefault(d => d.Id == Guid.NewGuid() && d.Md5 == "123");
                }
            }
        }
    }
}