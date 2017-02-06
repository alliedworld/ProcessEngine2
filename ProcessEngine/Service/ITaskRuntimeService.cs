using System;
using System.Collections.Generic;
using System.Linq;
using KlaudWerk.ProcessEngine.Definition;
using KlaudWerk.ProcessEngine.Persistence;

namespace KlaudWerk.ProcessEngine.Service
{
    public interface ITaskRuntimeService
    {
        TaskId CreateTask(IProcessRuntimeEnvironment env);
    }

    /// <summary>
    /// Task Runtime service
    /// </summary>
    public abstract class TaskRuntimeServiceBase : ITaskRuntimeService
    {
        /// <summary>
        /// Create a task
        /// </summary>
        public TaskId CreateTask(IProcessRuntimeEnvironment env)
        {
            TaskId id = new TaskId();
            TaskData data=new TaskData
            {
                Id = id.Id,
                ProcessId = env.ProcessId,
                StepId = env.Step.StepDefinition.StepId,
                CreatedeBy = env.Caller,
                Actions = CreateTaskActions(env.Step.StepDefinition)
            };
            OnCreateTask(data);
            return id;
        }
        /// <summary>
        /// Create task in Persistent storage
        /// </summary>
        /// <param name="data"></param>
        protected abstract void OnCreateTask(TaskData data);

        /// <summary>
        /// Create Task Actions
        /// </summary>
        /// <param name="stepDefinition"></param>
        /// <returns></returns>
        private IList<TaskAction> CreateTaskActions(StepDefinition stepDefinition)
        {
            return stepDefinition.Actions?.Select(a => new TaskAction
            {
                Account = null,
                ActionDefinition = a,
                Completed = false,
                LastUpdated = DateTime.UtcNow
            }).ToList();
        }
    }
}