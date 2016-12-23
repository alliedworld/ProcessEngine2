using System;

namespace KlaudWerk.ProcessEngine.Persistence
{
    public class ActionPersistence
    {
        public Guid StepId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime DateUpdated { get; set; }
        public virtual AccountData UpdatedBy { get; set; }
    }
}