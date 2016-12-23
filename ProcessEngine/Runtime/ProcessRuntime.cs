using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;                                                                                            

namespace KlaudWerk.ProcessEngine.Runtime
{
    /// <summary>
    /// Every instance of Process runtime is single-threaded object
    /// that holds the currently executed status of the process.
    /// The state of the runtime can be restored from a persistent storage.
    /// </summary>
    public class ProcessRuntime: IEquatable<ProcessRuntime>, IProcessRuntime
    {
        private static readonly string[] EmptyStrArr={};
        private static readonly Dictionary<ProcessStateEnum,Func<StateActionParams, Tuple<ExecutionResult, StepRuntime>>> 
            StatusActions=new Dictionary<ProcessStateEnum, Func<StateActionParams, Tuple<ExecutionResult, StepRuntime>>>
            {
                {ProcessStateEnum.NotStarted, OnStart},
                {ProcessStateEnum.Ready, OnReady},
                {ProcessStateEnum.Suspended, OnSuspended},
                {ProcessStateEnum.Failed, OnFailed},
                {ProcessStateEnum.Completed, OnCompleted},
            };
        private static readonly Dictionary<StepExecutionStatusEnum, Func<StateActionParams, Tuple<ExecutionResult, StepRuntime>>>
            PostStatusActions = new Dictionary<StepExecutionStatusEnum, Func<StateActionParams, Tuple<ExecutionResult, StepRuntime>>>
            {
                {StepExecutionStatusEnum.Ready, OnPostReady},
                {StepExecutionStatusEnum.Suspend, OnPostSuspended},
                {StepExecutionStatusEnum.Failed, OnPostFailed}
            };

        private readonly IReadOnlyList<LinkRuntime> _links;
        private readonly IReadOnlyList<StepRuntime> _steps;
        private StepRuntime _lastExecutedStep;
        private readonly List<string> _errors=new List<string>();
        /// <summary>
        /// Process Id
        /// </summary>
        public Guid Id { get; }
        /// <summary>
        /// Process State
        /// </summary>
        public ProcessStateEnum State { get; private set; }
        /// <summary>
        /// Process was suspended in step
        /// </summary>
        public StepRuntime SuspendedInStep { get; private set; }
        /// <summary>
        /// Last Executed Step in the process
        /// </summary>
        public StepRuntime LastExecutedStep => _lastExecutedStep;
        /// <summary>
        /// Start Steps in the workflow
        /// </summary>
        public IReadOnlyList<StepRuntime> StartSteps => _steps.Where(s => s.IsStart).ToImmutableList();
        /// <summary>
        /// List of errors
        /// </summary>
        public IReadOnlyList<string> Errors => _errors;
        /// <summary>
        /// Process Runtime Constructor
        /// </summary>
        /// <param name="id">Id of the process</param>
        /// <param name="links">List of all links</param>
        /// <param name="steps">List of all steps</param>
        public ProcessRuntime(
            Guid id,
            IEnumerable<LinkRuntime> links,
            IEnumerable<StepRuntime> steps
            )
        {
            _links = links.ToList();
            _steps = steps.ToList();
            State=ProcessStateEnum.NotStarted;
            Id = id;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="links"></param>
        /// <param name="steps"></param>
        /// <param name="suspendedInStep"></param>
        /// <param name="status"></param>
        public ProcessRuntime(Guid id,
            IEnumerable<LinkRuntime> links,
            IEnumerable<StepRuntime> steps,
            StepRuntime suspendedInStep,
            ProcessStateEnum status
        )
        {
            _links = links.ToList();
            _steps = steps.ToList();
            State = status;
            Id = id;
            SuspendedInStep = suspendedInStep;
        }

        //public ExecutionResult
        /// <summary>
        /// Try to compile all compilabe artifacts
        /// </summary>
        /// <param name="errors"></param>
        /// <returns></returns>
        public bool TryCompile(out string[] errors)
        {
            List<string> err=new List<string>();
            foreach (ICompilable artifact in _links.Cast<ICompilable>()
                .Concat(_steps))
            {
                string[] result;
                if(!artifact.TryCompile(out result))
                    err.AddRange(result);
            }
            errors = err.ToArray();
            return errors.Length == 0;
        }
        /// <summary>
        /// Execute Single Step in the workflow
        /// </summary>
        /// <returns>
        /// This method returns a tuple of two elements:
        /// <see cref="ExecutionResult"/>
        /// <see cref="StepRuntime"/>
        /// Next Step in Execution flow
        /// If the execution result has value of Suspend
        ///  </returns>
        public virtual Tuple<ExecutionResult,StepRuntime> Execute(StepRuntime rt,IProcessRuntimeEnvironment env)
        {

            Func<StateActionParams, Tuple<ExecutionResult, StepRuntime>> f;
            if (!StatusActions.TryGetValue(State, out f))
            {
                throw new InvalidProcessStateException($"Process Id={Id} State={State} is not supported.");
            }
            return f(new StateActionParams
            {
                Process = this,
                Step = rt,
                Env = env
            });
        }



        /// <summary>
        /// Continue the process execution from Suspended state
        /// </summary>
        /// <param name="env"></param>
        /// <returns></returns>
        public virtual Tuple<ExecutionResult, StepRuntime> Continue(IProcessRuntimeEnvironment env)
        {
            if (State != ProcessStateEnum.Suspended)
            {
                throw new InvalidProcessStateException($"To be able to continue, the Process Id={Id} must be in Suspended state. Current state is {State}.");
            }
            return OnStepExecutionReady(SuspendedInStep,env);

        }

        protected virtual bool TryEnter(StepRuntime rt, IProcessRuntimeEnvironment env, out string[] messages)
        {
            messages = EmptyStrArr;
            AsyncValidationResult onEnterResult = rt.ValidateOnEnter(env).Result;
            if (!onEnterResult.Valid)
                messages = onEnterResult.Messages;
            return onEnterResult.Valid;
        }

        /// <summary>
        /// On Execute step
        /// </summary>
        /// <param name="rt"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        protected virtual Tuple<ExecutionResult, StepRuntime> OnExecute(StepRuntime rt, IProcessRuntimeEnvironment env)
        {
            BeforeExecute();
            string[] errors;
            if (!TryEnter(rt, env, out errors))
            {
                State=ProcessStateEnum.Failed;
                OnEntryValidationError(errors);
                return new Tuple<ExecutionResult,
                    StepRuntime>(new ExecutionResult(StepExecutionStatusEnum.Failed), null);

            }
            State=ProcessStateEnum.Ready;
            _lastExecutedStep = rt;
            var stepExecutionResult = rt.ExecuteAsync(env).Result;
            Func<StateActionParams, Tuple<ExecutionResult, StepRuntime>> f;
            if (!PostStatusActions.TryGetValue(stepExecutionResult.Status, out f))
            {
                throw new InvalidProcessStateException(
                    $"Step Execution status {stepExecutionResult.Status} is not supported for Process Id={Id}");

            }
            try
            {
                return f(new StateActionParams
                {
                    Step = rt,
                    Process = this,
                    Env = env,
                    Result = stepExecutionResult
                });
            }
            finally
            {
                AfterExecute();
            }
        }

        /// <summary>
        /// After Execute Hook Method
        /// </summary>
        protected virtual void AfterExecute()
        {
        }

        /// <summary>
        /// Before Execute hook method
        /// </summary>
        protected virtual void BeforeExecute()
        {
            _errors.Clear();
        }

        #region Status Actions
        /// <summary>
        /// OnStart Action ensures that the step is the start step
        /// </summary>
        /// <param name="stateActionParams"></param>
        private static Tuple<ExecutionResult, StepRuntime> OnStart(StateActionParams stateActionParams)
        {
            stateActionParams.NotNull("OnStart:StateActionParams");
            stateActionParams.Step.NotNull("OnStart:Step");
            if (!stateActionParams.Step.IsStart)
            {
                throw new InvalidProcessStateException($"Process Id {stateActionParams.Process.Id} shoud start from a start step.");
            }
            return stateActionParams.Process.OnExecute(stateActionParams.Step, stateActionParams.Env);
        }

        /// <summary>
        /// OnReady ensures that passed step has links from previously executed step
        /// </summary>
        /// <param name="stateActionParams"></param>
        /// <returns></returns>
        private static Tuple<ExecutionResult, StepRuntime> OnReady(StateActionParams stateActionParams)
        {
            stateActionParams.NotNull("OnReady:StateActionParams");
            stateActionParams.Step.NotNull("OnReady:Step");
            return stateActionParams.Process.OnExecute(stateActionParams.Step, stateActionParams.Env);
        }

        /// <summary>
        /// Execution of Completed workflow doesnt' perform any actions
        /// </summary>
        /// <param name="stateActionParams"></param>
        /// <returns></returns>
        private static Tuple<ExecutionResult, StepRuntime> OnCompleted(StateActionParams stateActionParams)
        {
            return new Tuple<ExecutionResult, StepRuntime>(new ExecutionResult(StepExecutionStatusEnum.Completed)
                ,stateActionParams.Process.LastExecutedStep);
        }

        /// <summary>
        /// Execution of Failed workflow does not 
        /// </summary>
        /// <param name="stateActionParams"></param>
        /// <returns></returns>
        private static Tuple<ExecutionResult, StepRuntime> OnFailed(StateActionParams stateActionParams)
        {
            return new Tuple<ExecutionResult, StepRuntime>(new ExecutionResult(StepExecutionStatusEnum.Completed)
                , stateActionParams.Process.LastExecutedStep);
        }
        /// <summary>
        /// Execution from suspended state
        /// </summary>
        /// <param name="stateActionParams"></param>
        /// <returns></returns>
        private static Tuple<ExecutionResult, StepRuntime> OnSuspended(StateActionParams stateActionParams)
        {
            throw new InvalidProcessStateException($"Process Id={stateActionParams.Process.Id} is in Suspended state and cannot be executed.");
        }
        #endregion
        #region Post Status Actions
        private static Tuple<ExecutionResult, StepRuntime> OnPostReady(StateActionParams arg)
        {
            return arg.Process.OnStepExecutionReady(arg.Step, arg.Env);
        }
        private static Tuple<ExecutionResult, StepRuntime> OnPostSuspended(StateActionParams arg)
        {
            arg.Process._lastExecutedStep = arg.Step;
            arg.Process.State = ProcessStateEnum.Suspended;
            arg.Process.SuspendedInStep = arg.Step;
            arg.Process.OnSuspend(arg.Step);
            return new Tuple<ExecutionResult, StepRuntime>(
                new ExecutionResult(StepExecutionStatusEnum.Suspend), arg.Step);
        }
        private static Tuple<ExecutionResult, StepRuntime> OnPostFailed(StateActionParams arg)
        {
            arg.Process.OnStepExecutionFailed(arg.Step, arg.Result.ErrorMessages);
            arg.Process.State=ProcessStateEnum.Failed;
            return new Tuple<ExecutionResult, StepRuntime>
                (new ExecutionResult(StepExecutionStatusEnum.Failed), arg.Step);
        }

        #endregion
        /// <summary>
        /// Step Execution Read moves the workflow to its next step.
        /// </summary>
        /// <param name="rt"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        protected virtual Tuple<ExecutionResult, StepRuntime> OnStepExecutionReady(StepRuntime rt,IProcessRuntimeEnvironment env)
        {
            _lastExecutedStep = rt;
            var onExitResult = rt.ValidateOnExit(env).Result;
            if (onExitResult.Valid)
            {
                StepRuntime next;
                if (TryGetNextStep(rt, env, out next))
                    return new Tuple<ExecutionResult, StepRuntime>(
                        new ExecutionResult(StepExecutionStatusEnum.Ready), next);
                if (rt.IsEnd)
                {
                    SuspendedInStep = null;
                    State=ProcessStateEnum.Completed;
                    return new Tuple<ExecutionResult, StepRuntime>(new ExecutionResult(StepExecutionStatusEnum.Completed),rt);
                }
                else
                {
                    return new Tuple<ExecutionResult, StepRuntime>(new ExecutionResult(StepExecutionStatusEnum.Ready), next);
                }
            }
            OnExitValidationError(onExitResult.Messages);
            State=ProcessStateEnum.Failed;
            return new Tuple<ExecutionResult,StepRuntime>(
                new ExecutionResult(StepExecutionStatusEnum.Failed, null, onExitResult.Messages), rt);
        }

        /// <summary>
        /// The flow is suspended;
        /// It can be persisted and destroyed or keep in memory and "rectified"
        /// </summary>
        protected virtual void OnSuspend(StepRuntime suspendedStep)
        {
            _lastExecutedStep = suspendedStep;
            State = ProcessStateEnum.Suspended;
            SuspendedInStep = suspendedStep;
        }

        protected virtual void OnEntryValidationError(string[] messages)
        {
            _errors.AddRange(messages);
        }

        protected virtual void OnExitValidationError(string[] messages)
        {
            _errors.AddRange(messages);
        }

        protected virtual void OnStepExecutionFailed(StepRuntime failedStep, string[] messages)
        {
            _lastExecutedStep = failedStep;
            _errors.AddRange(messages);
        }

        /// <summary>
        /// Try to get next step to continue execution
        /// </summary>
        /// <param name="step"></param>
        /// <param name="env"></param>
        /// <param name="nextStep"></param>
        /// <returns></returns>
        private bool TryGetNextStep(StepRuntime step, IProcessRuntimeEnvironment env, out StepRuntime nextStep)
        {
            nextStep = null;
            // check for specific transition name
            var linkRuntimes = _links.Where(l => l.SourceStepId == step.Id).ToList();

            if (linkRuntimes.Count == 1)
            {
                nextStep = _steps.FirstOrDefault(s => s.Id == linkRuntimes[0].TargetStepId);
                return true;
            }
            if (!string.IsNullOrEmpty(env.Transition))
            {
                LinkRuntime linkRuntime = linkRuntimes.FirstOrDefault(l => l.Definition.Name == env.Transition);
                nextStep = linkRuntime == null ? null : _steps.FirstOrDefault(s => s.Id == linkRuntime.TargetStepId);
                return nextStep != null;
            }
            foreach (LinkRuntime link in linkRuntimes)
            {
                if (link.Evaluate(env).Result)
                {
                    nextStep = _steps.FirstOrDefault(s => s.Id == link.TargetStepId);
                    break;
                }
            }
            return nextStep != null;
        }

        /// <summary>
        /// Equality by Id
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(ProcessRuntime other)
        {
            return other != null && other.Id == Id;
        }
        #region Private Classes
        private class StateActionParams
        {
            public ProcessRuntime Process { get; set; }
            public StepRuntime Step { get; set; }
            public IProcessRuntimeEnvironment Env { get; set; }
            public ExecutionResult Result { get; set; }
        }
        #endregion
    }
}