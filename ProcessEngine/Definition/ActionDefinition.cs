using System;
using System.Runtime.Serialization;

namespace KlaudWerk.ProcessEngine.Definition
{
    /// <summary>
    /// Serializable immutable action definition
    /// </summary>
    [DataContract(Name="action")]
    [Serializable]
    public class ActionDefinition:IProcessDefinitionVisitable
    {
        [DataMember(Name="name")]
        public string Name { get; set; }
        [DataMember(Name="desc")]
        public string Description { get; set;}
        [DataMember(Name="canSkip")]
        public bool Skippable { get; set;}

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionDefinition"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        /// <param name="skippable">if set to <c>true</c> [skippable].</param>
        public ActionDefinition(string name, string description, bool skippable)
        {
            Name = name;
            Description = description;
            Skippable = skippable;
        }
        /// <summary>
        /// Default constructor
        /// </summary>
        public ActionDefinition()
        {
        }

        public override string ToString()
        {
            return $"Action: {Name}:{Description}:{Skippable}";
        }

        public void Accept(IProcessDefinitionVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}