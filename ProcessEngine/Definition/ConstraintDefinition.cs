using System;
using System.Linq;
using System.Runtime.Serialization;
using KlaudWerk.ProcessEngine.Builder;
using Newtonsoft.Json;

namespace KlaudWerk.ProcessEngine.Definition
{
    /// <summary>
    /// Constraint Definition class contains JSON-serialized constraints
    /// </summary>
    [Serializable]
    [DataContract(Name = "constraints")]
    public class ConstraintDefinition:IProcessDefinitionVisitable
    {
        [DataMember(Name="jsmin")]
        public string MaxValue { get; set; }
        [DataMember(Name="jsmax")]
        public string MinValue { get; set; }
        [DataMember(Name="jsvals")]
        public string[] PossibleValues { get; set; }
        [DataMember(Name="jsdef")]
        public string DefaultValue { get; set; }
        [DataMember(Name="hint")]
        public DisplayHintEnum DisplayHint { get; set; }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="builder"></param>
        public ConstraintDefinition(ConstraintBuilder builder)
        {
            if (builder == null)
                return;
            DisplayHint = builder.DisplayHint;
            if (builder.HasDefault && builder.Default != null)
                DefaultValue = JsonConvert.SerializeObject(builder.Default);
            if (builder.HasMin && builder.Min != null)
                MinValue = JsonConvert.SerializeObject(builder.Min);
            if (builder.HasMax && builder.Max != null)
                MaxValue = JsonConvert.SerializeObject(builder.Max);
            if (builder.HasPossibleValuesList)
            {
                PossibleValues = builder.Values?.Select(JsonConvert.SerializeObject).ToArray();
            }
        }
        /// <summary>
        /// Default constructor
        /// </summary>
        public ConstraintDefinition()
        {
        }
        /// <summary>
        /// Visitor
        /// </summary>
        /// <param name="visitor"></param>
        public void Accept(IProcessDefinitionVisitor visitor)
        {
          visitor.Visit(this);
        }
    }
}