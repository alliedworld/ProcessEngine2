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
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using KlaudWerk.ProcessEngine.Definition;
using KlaudWerk.ProcessEngine.Persistence;
using Newtonsoft.Json;

namespace Klaudwerk.ProcessDeployer
{
    public class Deployer
    {
        private static WindsorContainer _container=new WindsorContainer(new XmlInterpreter());
        private readonly List<IProcessDefinitionRegistry> _registries=new List<IProcessDefinitionRegistry>();

        private readonly IProcessDefinitionPersisnenceService
            _persistence = GetPersistence();

        private static IProcessDefinitionPersisnenceService GetPersistence()
        {
            return _container.Resolve<IProcessDefinitionPersisnenceService>("IProcessDefinitionPersisnenceService");
        }

        public IEnumerable<IProcessDefinitionRegistry> Registries
        {
            get { return _registries; }
            set
            {
                _registries.Clear();
                _registries.AddRange(value);
                _registries.ForEach(r =>
                {
                    string[] errors;
                    r.TryInitialize(out errors);
                });
            }
        }
        /// <summary>
        /// List of assemblies
        /// </summary>
        public IEnumerable<ProcessDescriptor> Processes {
            get
            {
                List<ProcessDescriptor> pd=new List<ProcessDescriptor>();
                Registries?.ToList().ForEach(r=>pd.AddRange(r.Processes));
                return pd;
            }
        }

        public IEnumerable<ProcessDefinitionDigest> DeployedWorkflows => _persistence.LisAlltWorkflows();

        public void DeployWorkflow(Guid guid)
        {
            List<ProcessDefinition> defs=new List<ProcessDefinition>();
            _registries.ForEach(r =>
            {
                ProcessDefinition pd;
                if (r.TryGet(guid, out pd))
                    defs.Add(pd);
            });
            if (defs.Count == 0)
                throw new ArgumentException($"No Process Definition with ID={guid} was found.");
            if (defs.Count > 1)
                throw new ArgumentException($"{defs.Count} Process Definitions with ID={guid} was found.");
            Md5CalcVisitor visitor=new Md5CalcVisitor();
            defs[0].Accept(visitor);
            string md5 = visitor.CalculateMd5();
            IReadOnlyList<ProcessDefinitionDigest> processes = _persistence.LisAlltWorkflows();
            var processDefinitionDigests = processes.Where(p => p.Id == guid).ToArray();
            int nextVersion = processDefinitionDigests.Length>0?processDefinitionDigests.Max(p => p.Version)+1:1;
            foreach (ProcessDefinitionDigest p in processDefinitionDigests)
            {
                 if(p.Md5==md5)
                     throw new ArgumentException($"Process ID={defs[0].Id} MD5={md5} already deployed. Cannot deploy the same process with the same MD5.");
            }
            foreach (ProcessDefinitionDigest p in processes)
            {
                _persistence.SetStatus(p.Id, p.Version, ProcessDefStatusEnum.NotActive);
            }
            _persistence.Create(defs[0],ProcessDefStatusEnum.Active,nextVersion);
        }

        public void SetWorkflowStatus(Guid guid, int version, ProcessDefStatusEnum status)
        {
            _persistence.SetStatus(guid, version, status);
        }

        public void AddRole(Guid flowId, int version, string roleString)
        {
            // parse the role string
            string[] tokens = roleString.Split('&');
            if (tokens.Length < 4)
            {
                throw new ArgumentException("Role string should have following format: <typenumber>&<id>&<name>&<sourcesystem>?");
            }
            AccountData account=new AccountData
            {
                AccountType = Int32.Parse(tokens[0]),
                Id = new Guid(tokens[1]),
                Name = tokens[2],
                SourceSystem = tokens[3]
            };
            _persistence.AddRoles(flowId,version, new [] {account});
        }

        public void RemoveWorkflow(Guid guid, int aVersion)
        {
            _persistence.Remove(guid, aVersion);
        }

        public void PrintDefinition(Guid guid, int aVersion)
        {
            ProcessDefinition pd;
            ProcessDefStatusEnum stat;
            AccountData[] accounts;
            if (!_persistence.TryFind(guid, aVersion, out pd, out stat, out accounts))
            {
                Console.WriteLine($"Cannot find workflow Id={guid} version={aVersion}" );
                return;
            }
            Console.WriteLine(JsonConvert.SerializeObject(pd, Formatting.Indented));
        }
    }
}