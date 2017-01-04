/**
The MIT License (MIT)

Copyright (c) 2016 Igor Polouektov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
  */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KlaudWerk.ProcessEngine.Definition;

namespace KlaudWerk.ProcessEngine.Runtime
{
    /// <summary>
    /// Step Runtime
    /// </summary>
    public class StepRuntime:ICompilable
    {
        private CsScriptRuntime _onEnter;
        private CsScriptRuntime _onExit;
        private readonly StepDefinition _stepDef;
        private readonly IStepHandler _handler;
        private List<LinkRuntime> _links=new List<LinkRuntime>();

        public string StepId => _stepDef.StepId;
        public Guid Id => _stepDef.Id;
        public bool IsStart => _stepDef.IsStart;
        public bool IsEnd => _stepDef.IsEnd;
        public StepDefinition StepDefinition => _stepDef;
        /// <summary>
        /// "Compiled" indicator
        /// </summary>
        public bool IsCompiled { get; private set; }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="stepDef"></param>
        public StepRuntime(StepDefinition stepDef)
        {
            _stepDef = stepDef;
            switch (_stepDef.StepHandler.StepHandlerType)
            {
                case StepHandlerTypeEnum.None: _handler = new EmptyHandler(); break;
                case StepHandlerTypeEnum.IoC: _handler = new IoCServiceHandler(this); break;
                case StepHandlerTypeEnum.Task: _handler = new TaskServiceHandler(this); break;
                case StepHandlerTypeEnum.Script: _handler = new ScriptHandler(this); break;
                case StepHandlerTypeEnum.Service: _handler=new AsmLoadHandler(this); break;
            }
        }
        /// <summary>
        /// Try to compile the step
        /// </summary>
        /// <param name="errors"></param>
        /// <returns></returns>
        public bool TryCompile(out string[] errors)
        {
            List<string> aggregatingErrors=new List<string>();
            if (IsCompiled)
            {
                errors = aggregatingErrors.ToArray();
                return true;
            }
            if (_stepDef.OnEntry != null)
            {
                string[] onEntryErrors;
                _onEnter = new CsScriptRuntime(_stepDef.OnEntry);
                _onEnter.TryCompile(out onEntryErrors);
                aggregatingErrors.AddRange(onEntryErrors);
            }
            if (_stepDef.OnExit != null)
            {
                string[] onExitErrors;
                _onExit = new CsScriptRuntime(_stepDef.OnExit);
                _onExit.TryCompile(out onExitErrors);
                aggregatingErrors.AddRange(onExitErrors);
            }
            string[] handlerErrors;
            _handler.TryCompile(out handlerErrors);
            aggregatingErrors.AddRange(handlerErrors);
            errors = aggregatingErrors.ToArray();
            IsCompiled = true;
            return errors.Length == 0;
        }

        /// <summary>
        /// Validating Step conditions on entry
        /// </summary>
        /// <param name="env"></param>
        /// <returns></returns>
        public async Task<AsyncValidationResult> ValidateOnEnter(IProcessRuntimeEnvironment env)
        {
            return await ValidateOnEnterOnExit(env, _onEnter,
                v => (v.Required & VarRequiredEnum.OnEntry) == VarRequiredEnum.OnEntry);
        }
        /// <summary>
        /// Validate Step conditions on exit
        /// </summary>
        /// <param name="env"></param>
        /// <returns></returns>
        public async Task<AsyncValidationResult> ValidateOnExit(IProcessRuntimeEnvironment env)
        {
            return await ValidateOnEnterOnExit(env, _onExit,
                v => (v.Required & VarRequiredEnum.OnExit) == VarRequiredEnum.OnExit);
        }

        /// <summary>
        /// Executes the step asynchrnously.
        /// </summary>
        /// <param name="env">The env.</param>
        /// <returns></returns>
        public async Task<ExecutionResult> ExecuteAsync(IProcessRuntimeEnvironment env)
        {
            return await _handler.ExecuteAsync(env);
        }

        #region Private Methods
        /// <summary>
        /// Validates the on enter on exit criteria.
        /// </summary>
        /// <param name="env">The env.</param>
        /// <param name="rt">The rt.</param>
        /// <param name="f">The f.</param>
        /// <returns></returns>
        private async Task<AsyncValidationResult> ValidateOnEnterOnExit(IProcessRuntimeEnvironment env,
            CsScriptRuntime rt, Func<VariableMapDefinition, bool> f)
        {
            if (!IsCompiled)
                return new AsyncValidationResult($"The step Id {_stepDef.StepId} is not compiled.");
            string[] errors;
            if (!TryValidateVariables(env, _stepDef.VariablesMap?.Where(f) ?? new VariableMapDefinition[] { }, out errors))
            {
                return new AsyncValidationResult(errors);
            }
            if (rt == null)
                return new AsyncValidationResult();
            int result = await rt.Execute(env);
            return result == 1 ? new AsyncValidationResult() : new AsyncValidationResult("Script validation fails");
        }

        /// <summary>
        /// Tries to validate required variables.
        /// </summary>
        /// <param name="env">The env.</param>
        /// <param name="vars">The vars.</param>
        /// <param name="errors">The errors.</param>
        /// <returns></returns>
        private bool TryValidateVariables(IProcessRuntimeEnvironment env,IEnumerable<VariableMapDefinition> vars,out string[] errors)
        {
            List<string> varErrors = new List<string>();
            foreach(var v in vars)
            {
                try
                {
                    object val = env.PropertySet.Get<object>(v.Name);
                    if (val == null)
                        varErrors.Add($"Variable {v.Name} has no value.");
                }
                catch(ArgumentException ex)
                {
                    varErrors.Add(ex.Message);
                }
            }
            errors = varErrors.ToArray();
            return varErrors.Count==0;
        }
        #endregion

        #region private handlers implementations
        /// <summary>
        /// IOC service handler
        /// </summary>
        /// <seealso cref="ProcessEngine.IStepHandler" />
        private class IoCServiceHandler : IStepHandler
        {
            private readonly StepRuntime _step;
            public IoCServiceHandler(StepRuntime step)
            {
                _step = step;
            }

            public async Task<ExecutionResult> ExecuteAsync(IProcessRuntimeEnvironment env)
            {
                return await env.IocServiceAsync(_step._stepDef.StepHandler.IocName);
            }

            public bool TryCompile(out string[] errors)
            {
                errors = new string[] { };
                return true;
            }
        }
        /// <summary>
        /// Task Service Handler
        /// </summary>
        /// <seealso cref="ProcessEngine.IStepHandler" />
        private class TaskServiceHandler : IStepHandler
        {
            private StepRuntime _step;
            public TaskServiceHandler(StepRuntime step)
            {
                _step = step;
            }
            public async Task<ExecutionResult> ExecuteAsync(IProcessRuntimeEnvironment env)
            {
                return await env.TaskServiceAsync();
            }
            public bool TryCompile(out string[] errors)
            {
                errors = new string[] { };
                return true;
            }
        }
        /// <summary>
        /// Load Assembly Handler
        /// </summary>
        /// <seealso cref="ProcessEngine.IStepHandler" />
        private class AsmLoadHandler:IStepHandler
        {
            private StepRuntime _step;
            public AsmLoadHandler(StepRuntime step)
            {
                _step = step;
            }
            public async Task<ExecutionResult> ExecuteAsync(IProcessRuntimeEnvironment env)
            {
                return await env.LoadExecuteAssemplyAsync(_step._stepDef.StepHandler.ClassFullName);
            }
            public bool TryCompile(out string[] errors)
            {
                errors = new string[] { };
                return true;
            }
        }
        /// <summary>
        /// Empty handler
        /// </summary>
        /// <seealso cref="ProcessEngine.IStepHandler" />
        private class EmptyHandler : IStepHandler
        {
            public async Task<ExecutionResult> ExecuteAsync(IProcessRuntimeEnvironment env)
            {
                return new ExecutionResult(StepExecutionStatusEnum.Ready);
            }
            public bool TryCompile(out string[] errors)
            {
                errors = new string[] { };
                return true;
            }
        }
        /// <summary>
        /// Script Handler
        /// </summary>
        private class ScriptHandler : IStepHandler
        {
            private readonly CsScriptRuntime _script;
            public ScriptHandler(StepRuntime step)
            {
                _script = new CsScriptRuntime(step._stepDef.StepHandler.Script);
            }

            public async Task<ExecutionResult> ExecuteAsync(IProcessRuntimeEnvironment env)
            {
                try
                {
                    int result = await _script.Execute(env);
                    return new ExecutionResult(result == 1
                        ? StepExecutionStatusEnum.Ready
                        : StepExecutionStatusEnum.Failed);
                }
                catch (AggregateException ex)
                {
                    return new ExecutionResult(StepExecutionStatusEnum.Failed, null, ex.Flatten().Message);
                }
                catch (Exception ex)
                {
                    return new ExecutionResult(StepExecutionStatusEnum.Failed, null, ex.Message);
                }
            }

            public bool TryCompile(out string[] errors)
            {
                return _script.TryCompile(out errors);
            }
        }
        #endregion

    }
}