using System;
using System.Runtime.Serialization;

namespace KlaudWerk.ProcessEngine.Definition
{
    [DataContract(Name = "varmap")]
    [Serializable]
    public class VariableMapDefinition:IProcessDefinitionVisitable
    {
        [DataMember(Name="name")]
        public string Name { get; set; }
        [DataMember(Name="req")]
        public VarRequiredEnum Required { get; set; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <param name="required"></param>
        public VariableMapDefinition(string name, VarRequiredEnum required)
        {
            Name = name;
            Required = required;
        }
        /// <summary>
        /// Defaul
        /// </summary>
        public VariableMapDefinition()
        {
        }

        public override string ToString()
        {
            return $"VariableMap {Name}:{Required}";
        }

        public void Accept(IProcessDefinitionVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}