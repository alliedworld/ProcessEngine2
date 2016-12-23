using System;
using System.Collections.Generic;
using System.Linq;
using KlaudWerk.ProcessEngine.Definition;
using KlaudWerk.ProcessEngine.Persistence;

namespace Klaudwerk.ProcessDeployer
{
    public class Deployer
    {
        private readonly List<IProcessDefinitionRegistry> _registries=new List<IProcessDefinitionRegistry>();
        private readonly ProcessDefinitionPersisnenceService _persistence=new ProcessDefinitionPersisnenceService();

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
    }
}