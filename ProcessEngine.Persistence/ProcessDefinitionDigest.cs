using System;

namespace KlaudWerk.ProcessEngine.Persistence
{
    /// <summary>
    /// Digest of Process Definition
    /// </summary>
    public class ProcessDefinitionDigest
    {
        public Guid Id { get; set; }
        public int Version { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Md5 { get; set; }
        public DateTime LastUpdated { get; set; }
        public ProcessDefStatusEnum Status { get; set; }
    }
}