using System;
using System.Runtime.Serialization;

namespace KlaudWerk.ProcessEngine.Definition
{
    /// <summary>
    /// Action Relations Definition
    /// </summary>
    [Serializable]
    [DataContract(Name="actionsrel")]
    public class ActionRelationDefinition : IProcessDefinitionVisitable
    {
        /// <summary>
        /// Id of the source action
        /// </summary>
        [DataMember(Name="sourceaction")]
        public string SourceActionId { get; set; }
        /// <summary>
        /// Id Of the target action
        /// </summary>
        [DataMember(Name="targetaction")]
        public string TargetActionId { get; set; }
        /// <summary>
        /// Id of the source action setp
        /// </summary>
        [DataMember(Name="sourceastep")]
        public string SourceStepId { get; set; }
        /// <summary>
        /// Id of the target action setp 
        /// </summary>
        [DataMember(Name="targetstep")]
        public string TargetStepId { get; set; }

        /// <summary>
        /// Accept the visitor
        /// </summary>
        /// <param name="visitor">
        /// <see cref="IProcessDefinitionVisitor"/>
        /// </param>
        public void Accept(IProcessDefinitionVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override string ToString()
        {
            return $"{SourceActionId}:{SourceStepId}->{TargetActionId}:{TargetStepId}";
        }
    }
}