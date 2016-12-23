using System;
using System.Runtime.Serialization;

namespace KlaudWerk.ProcessEngine.Definition
{
    [Serializable]
    [DataContract(Name = "stepdefid")]
    public class StepDefinitionId
    {
        [DataMember(Name="id")]
        public Guid Id { get; set; }
        [DataMember(Name = "step_id")]
        public string StepId { get; set; }

        public StepDefinitionId(Guid id, string stepId)
        {
            Id = id;
            StepId = stepId;
        }

        public StepDefinitionId()
        {
        }
    }
}