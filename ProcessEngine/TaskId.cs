using System;

namespace KlaudWerk.ProcessEngine
{
    /// <summary>
    /// Type-safe task id
    /// </summary>
    [Serializable]
    public class TaskId : GuidId<TaskId>, IEquatable<TaskId>
    {
        public TaskId(Guid id) : base(id)
        {
        }

        public TaskId()
        {
        }

        public bool Equals(TaskId other)
        {
            if (other == null)
                return false;
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as TaskId);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(TaskId left, TaskId right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TaskId left, TaskId right)
        {
            return !Equals(left, right);
        }
    }
}