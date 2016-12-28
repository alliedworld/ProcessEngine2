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
using Klaudwerk.PropertySet;
using KlaudWerk.ProcessEngine.Definition;
using KlaudWerk.ProcessEngine.Runtime;
using Newtonsoft.Json;

namespace KlaudWerk.ProcessEngine.Persistence
{
    public class ProcessRuntimePersistenceService:ProcessRuntimeService
    {
        private readonly PropertySetPersistenceService _propertySepPersistenceService=new PropertySetPersistenceService();
        /// <summary>
        /// Create and persist runtime processor
        /// </summary>
        /// <param name="pd"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override IProcessRuntime Create(ProcessDefinition pd, IPropertySetCollection collection)
        {
            IProcessRuntime runtime = base.Create(pd, collection);
            Md5CalcVisitor visitor = new Md5CalcVisitor();
            pd.Accept(visitor);
            string md5 = visitor.CalculateMd5();
            using (var ctx = new ProcessDbContext())
            {
                var pdList = ctx.ProcessDefinition
                    .Where(p => p.FlowId == pd.FlowId && p.Md5 == md5).ToList();

                if(pdList.Count()!= 1)
                    throw new ArgumentException($"Process definition is not persisted.");
                // save or update the collection
                PersistentPropertyCollection persistedCollection =
                    _propertySepPersistenceService.SaveCollection(ctx, runtime.Id, collection);
                ProcessRuntimePersistence rtp=new ProcessRuntimePersistence
                {
                    Id=runtime.Id,
                    SuspendedStepId = runtime.LastExecutedStep?.StepId,
                    Status = (int)runtime.State,
                    LastUpdated = DateTime.UtcNow,
                    PropertyCollection = persistedCollection,
                    ProcessDefinition = pdList.ElementAt(0)
                };
                ctx.Process.Add(rtp);
                ctx.SaveChanges();
            }
            return new ProcessRuntimePersistenProxy(runtime,this);
        }

        public override void Freeze(IProcessRuntime runtime, IPropertySetCollection collection)
        {
            using (var ctx = new ProcessDbContext())
            {
                ProcessRuntimePersistence persistence = ctx.Process.Find(runtime.Id);
                if (persistence == null)
                {
                    throw new ArgumentException("Invalid Workflow Id:"+runtime.Id.ToString());
                }
                _propertySepPersistenceService.SaveCollection(ctx, runtime.Id, collection);
                persistence.Status = (int) runtime.State;
                persistence.LastUpdated=DateTime.UtcNow;
                persistence.SuspendedStepId = runtime.SuspendedInStep?.StepId;
                ctx.SaveChanges();
            }
        }

        /// <summary>
        /// Unfreeze the process
        /// </summary>
        /// <param name="processRuntimeId"></param>
        /// <param name="runtime"></param>
        /// <param name="nextStep"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        public override bool TryUnfreeze(Guid processRuntimeId, 
            out IProcessRuntime runtime,
            out StepRuntime nextStep,
            out IPropertySetCollection collection)
        {
            runtime = null;
            collection = null;
            nextStep = null;
            using (var ctx = new ProcessDbContext())
            {
                var rtp = ctx.Process.Find(processRuntimeId);
                if (rtp == null)
                    return false;
                var definition =
                    JsonConvert.DeserializeObject<ProcessDefinition>(rtp.ProcessDefinition.JsonProcessDefinition);
                definition.Id = rtp.ProcessDefinition.Id;
                collection=PropertySetPersistenceService.DeserializeCollection(rtp.PropertyCollection);
                if (!string.IsNullOrEmpty(rtp.NextStepId))
                {
                    StepDefinition stepDef = definition.Steps.SingleOrDefault(s => s.StepId == rtp.NextStepId);
                    nextStep = stepDef == null ? null : new StepRuntime(stepDef);
                }
                runtime = Create(rtp.Id, definition, rtp.SuspendedStepId, (ProcessStateEnum) rtp.Status);
            }
            return true;
        }

        protected virtual void OnContinue(IProcessRuntime real, Tuple<ExecutionResult, StepRuntime> result, IProcessRuntimeEnvironment env)
        {
            throw new NotImplementedException();
        }

        protected virtual void OnExecute(IProcessRuntime runtime, Tuple<ExecutionResult, StepRuntime> result, IProcessRuntimeEnvironment env)
        {
            using (var ctx = new ProcessDbContext())
            {
                ProcessRuntimePersistence persistence = ctx.Process.Find(runtime.Id);
                if (persistence == null)
                {
                    throw new ArgumentException("Invalid Workflow Id:" + runtime.Id);
                }
                _propertySepPersistenceService.SaveCollection(ctx, runtime.Id, env.PropertySet);
                persistence.Status = (int)runtime.State;
                persistence.LastUpdated = DateTime.UtcNow;
                persistence.SuspendedStepId = runtime.SuspendedInStep?.StepId;
                persistence.NextStepId = runtime.State==ProcessStateEnum.Completed? null: result.Item2?.StepId;
                ctx.SaveChanges();
            }

        }

        private class ProcessRuntimePersistenProxy : IProcessRuntime
        {
            private readonly IProcessRuntime _real;
            private readonly ProcessRuntimePersistenceService _persistenceService;
            public Guid Id => _real.Id;

            public ProcessStateEnum State => _real.State;

            public StepRuntime SuspendedInStep => _real.SuspendedInStep;

            public StepRuntime LastExecutedStep => _real.LastExecutedStep;

            public IReadOnlyList<StepRuntime> StartSteps => _real.StartSteps;

            public IReadOnlyList<string> Errors => _real.Errors;

            public ProcessRuntimePersistenProxy(IProcessRuntime real,
                ProcessRuntimePersistenceService persistenceService)
            {
                _real = real;
                _persistenceService = persistenceService;
            }

            public bool TryCompile(out string[] errors)
            {
                return _real.TryCompile(out errors);
            }

            public Tuple<ExecutionResult, StepRuntime> Execute(StepRuntime rt, IProcessRuntimeEnvironment env)
            {
                var result=_real.Execute(rt, env);
                _persistenceService.OnExecute(_real, result, env);
                return result;

            }

            public Tuple<ExecutionResult, StepRuntime> Continue(IProcessRuntimeEnvironment env)
            {
                var result =_real.Continue(env);
                _persistenceService.OnContinue(_real, result, env);
                return result;
            }
        }
    }
}