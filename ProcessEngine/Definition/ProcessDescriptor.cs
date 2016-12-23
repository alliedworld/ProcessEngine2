using System;
using System.Runtime.Serialization;

namespace KlaudWerk.ProcessEngine.Definition
{
    [DataContract(Name = "process_descriptor")]
    public class ProcessDescriptor
    {
        public Guid Id { get; }
        public string FlowId { get; }
        public string Name { get; }
        public string Description { get; }
        public string Md5 { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="flowId"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="md5"></param>
        public ProcessDescriptor(Guid id, string flowId, string name, string description, string md5)
        {
            Id = id;
            FlowId = flowId;
            Name = name;
            Description = description;
            Md5 = md5;
        }
        /// <summary>
        /// To String
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Id}|{Md5}|{Name} - {Description}";
        }
    }
}