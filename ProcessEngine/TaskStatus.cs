namespace KlaudWerk.ProcessEngine
{
    /// <summary>
    /// Task Status
    /// </summary>
    public enum TaskStatus
    {
        None,
        // Upon creation. Remains CREATED if there are no potential owners.
        Created,
        // Created task with multiple potential owners.
        Ready,
        // Created task with single potential owner. Work started. Actual owner set.
        Reserved,
        // Work started and task is being worked on now. Actual owner set.
        InProgress,
        // In any of its active states (Ready, Reserved, InProgress), a task can be suspended, 
        // transitioning it into the Suspended state. On resumption of the task, it transitions 
        // back to the original state from which it had been suspended.
        Suspended,
        // Successful completion of the work. One of the final states.
        Completed,
        // Unsuccessful completion of the work. One of the final states.
        Failed,
        // Unrecoverable error in human task processing. One of the final states.
        Error,
        // Task is no longer needed - skipped. This is considered a â€œgoodâ€ outcome of a task. One of the final states.
        Obsolete
    }
}