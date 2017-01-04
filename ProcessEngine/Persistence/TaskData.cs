using System;
using System.Collections.Generic;
using KlaudWerk.ProcessEngine.Definition;

namespace KlaudWerk.ProcessEngine.Persistence
{
    /// <summary>
    /// An action element associated with the task
    /// </summary>
    public class TaskAction
    {
        public virtual ActionDefinition ActionDefinition { get; set; }
        public virtual DateTime? LastUpdated { get; set; }
        public virtual bool Completed { get; set; }
        public virtual AccountData Account { get; set; }
    }

    public class TaskComment
    {
        public virtual string Text { get; set; }
        public virtual DateTime? Updated { get; set; }
        public virtual AccountData Account { get; set; }
    }
    /// <summary>
    /// Task Data Element
    /// </summary>
    public class TaskData
    {
        public virtual Guid Id { get; set; }
        public virtual Guid ProcessId { get; set; }
        public virtual string StepId { get; set;}
        public virtual string Description { get; set; }
        public virtual DateTime Deadline { get; set; }
        public virtual DateTime Created { get; set; }
        public virtual DateTime LastUpdated { get; set; }
        public virtual int Priority { get; set; }
        public virtual AccountData CreatedeBy { get; set; }
        /// <summary>
        /// A task can be assign either to an individual account or to a group
        /// </summary>
        public virtual AccountData AssignedTo { get; set; }
        /// <summary>
        /// A task was claimed by a user
        /// </summary>
        public virtual AccountData ClaimedBy { get; set; }
        /// <summary>
        /// Followers
        /// </summary>
        public virtual IList<AccountData> Followers { get; set; }
        /// <summary>
        /// Task Actions
        /// </summary>
        public virtual IList<TaskAction> Actions { get; set; }
        /// <summary>
        ///
        /// </summary>
        public virtual IList<TaskComment> Comments { get; set; }
        /// <summary>
        /// Task Status
        /// </summary>
        public virtual TaskStatus Status { get; set; }
    }
}