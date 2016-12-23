using System;
using System.Runtime.Serialization;

namespace KlaudWerk.ProcessEngine.Definition
{
    /// <summary>
    /// Serializable immutable link definition
    /// </summary>
    [DataContract(Name="link")]
    [Serializable]
    public class LinkDefinition:IProcessDefinitionVisitable
    {
        /// <summary>
        /// Gets the source.
        /// </summary>
        /// <value>
        /// The source.
        /// </value>
        [DataMember(Name="src")]
        public StepDefinitionId Source { get; set; }
        /// <summary>
        /// Gets the target.
        /// </summary>
        /// <value>
        /// The target.
        /// </value>
        [DataMember(Name="target")]
        public StepDefinitionId Target { get; set;}
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [DataMember(Name="name")]
        public string Name { get; set;}
        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        [DataMember(Name="desc")]
        public string Description { get; set;}
        /// <summary>
        /// Gets the script.
        /// </summary>
        /// <value>
        /// The script.
        /// </value>
        [DataMember(Name="script")]
        public ScriptDefinition Script { get; set;}

        /// <summary>
        /// Initializes a new instance of the <see cref="LinkDefinition"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        /// <param name="script">The script.</param>
        public LinkDefinition(StepDefinitionId source, StepDefinitionId target, string name, string description, ScriptDefinition script)
        {
            Source = source;
            Target = target;
            Name = name;
            Description = description;
            Script = script;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public LinkDefinition()
        {
        }

        public override string ToString()
        {
            return $"Link: {Source}->{Target} [{Name} {Description}] {Script}";
        }

        public void Accept(IProcessDefinitionVisitor visitor)
        {
            Script?.Accept(visitor);
            visitor.Visit(this);
        }
    }
}