using System;
using System.Runtime.Serialization;

namespace KlaudWerk.ProcessEngine.Definition
{
    /// <summary>
    /// Tag Definition
    /// </summary>
    [DataContract(Name="tag")]
    [Serializable]
    public class TagDefinition : IProcessDefinitionVisitable
    {
        /// <summary>
        /// Id
        /// </summary>
        [DataMember(Name="id")]
        public string Id { get; set; }
        /// <summary>
        /// Disolay Name
        /// </summary>
        [DataMember(Name="dn")]
        public string DisplayName { get; set; }
        /// <summary>
        /// Handler
        /// </summary>
        [DataMember(Name = "handler")]
        public StepHandlerDefinition Handler { get; set; }
        public void Accept(IProcessDefinitionVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}