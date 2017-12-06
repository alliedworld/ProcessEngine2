using System;
using System.Runtime.Serialization;

namespace KlaudWerk.ProcessEngine.Runtime
{
    [DataContract]
    public class ProcessRuntimeSummary
    {
        [DataMember]
        public Guid Id { get; set; }
        [DataMember]
        public ProcessStateEnum Status { get; set; }
        [DataMember]
        public DateTime LastUpdated { get; set; }
        [DataMember]
        public string NextStepId { get; set; }
        [DataMember]
        public string SuspendedInStep { get; set; }
        [DataMember]
        public string[] Errors { get; set; }
        [DataMember]
        public string FlowDefinitionId { get; set; }
        [DataMember]
        public string FlowDefinitionMd5 { get; set; }
    }
}