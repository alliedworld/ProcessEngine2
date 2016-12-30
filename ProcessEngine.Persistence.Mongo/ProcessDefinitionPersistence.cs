using System.Collections.Generic;
using KlaudWerk.ProcessEngine.Persistence;
using MongoDB.Bson;

namespace Klaudwerk.ProcessEngine.Persistence.Mongo
{
    public class ProcessDefinitionPersistence:ProcessDefinitionPersistenceBase
    {
        /// <summary>
        /// Associated Accounts
        /// </summary>
        public virtual IList<AccountData> Accounts { get; set; }
    }



}