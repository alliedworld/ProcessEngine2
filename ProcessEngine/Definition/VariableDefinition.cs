using System;
using System.Runtime.Serialization;
using KlaudWerk.ProcessEngine.Builder;

namespace KlaudWerk.ProcessEngine.Definition
{
    /// <summary>
    /// Variable definitions
    /// </summary>
    [Serializable]
    [DataContract(Name = "variable")]
    public class VariableDefinition:IProcessDefinitionVisitable
    {
        [DataMember(Name="name")]
        public string Name { get; set; }
        [DataMember(Name="desc")]
        public string Description { get; set; }
        [DataMember(Name="type")]
        public VariableTypeEnum VariableType { get; set; }
        [DataMember(Name="class")]
        public string ClassName { get; set; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="variableType"></param>
        /// <param name="className"></param>
        public VariableDefinition(string name, string description,
            VariableTypeEnum variableType, string className)
        {
            Name = name;
            Description = description;
            VariableType = variableType;
            ClassName = className;
        }
        /// <summary>
        ///
        /// </summary>
        public VariableDefinition()
        {
        }

        public void Accept(IProcessDefinitionVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}