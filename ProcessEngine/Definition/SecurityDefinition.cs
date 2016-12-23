using System;
using System.Runtime.Serialization;
using KlaudWerk.ProcessEngine.Builder;

namespace KlaudWerk.ProcessEngine.Definition
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [DataContract(Name="security")]
    public class SecurityDefinition:IProcessDefinitionVisitable
    {
        [DataMember(Name = "acc")]
        public string Account { get; set; }
        [DataMember(Name="type")]
        public AccountTypeEnum AccountType { get; set; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="account"></param>
        /// <param name="accountType"></param>
        public SecurityDefinition(string account, AccountTypeEnum accountType)
        {
            Account = account;
            AccountType = accountType;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public SecurityDefinition()
        {
        }

        public void Accept(IProcessDefinitionVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}