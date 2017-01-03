using System.Security.Cryptography;
using KlaudWerk.ProcessEngine.Persistence;

namespace Klaudwerk.ProcessEngine.Persistence.Mongo
{
    public class MongoProcessRuntimePersistence:ProcessRuntimePersistenceBase
    {
        /// <summary>
        /// Workflow Definition Flow Id
        /// </summary>
        public virtual string DefinitionFlowId { get; set; }
        /// <summary>
        /// Workflow Definition MD5
        /// </summary>
        public virtual string DefinitionMd5 { get; set; }
        /// <summary>
        /// Get Process Definition
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override ProcessDefinitionPersistenceBase GetProcessDefinition()
        {
            throw new System.NotImplementedException();
        }
    }
}