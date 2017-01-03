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
using Klaudwerk.PropertySet.Persistence;
using KlaudWerk.ProcessEngine;
using KlaudWerk.ProcessEngine.Definition;
using KlaudWerk.ProcessEngine.Persistence;
using KlaudWerk.ProcessEngine.Runtime;
using MongoDB.Driver;

namespace Klaudwerk.ProcessEngine.Persistence.Mongo
{
    /// <summary>
    /// Process Runtime Persistence Service
    /// </summary>
    public class MongoProcessRuntimePersistenceService:ProcessRuntimeService
    {
        public const string CollectionName = "process_runtime";
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<MongoProcessRuntimePersistence> _collection;
        private readonly MongoProcessDefinitionPersistenceService _processDefinitionService;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="database"></param>
        public MongoProcessRuntimePersistenceService(IMongoDatabase database)
        {
            _database = database;
            _collection = _database.GetCollection<MongoProcessRuntimePersistence>(CollectionName);
            _processDefinitionService=new MongoProcessDefinitionPersistenceService(_database);
        }
        /// <summary>
        /// Create process runtime definition
        /// </summary>
        /// <param name="pd"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public override IProcessRuntime Create(ProcessDefinition pd, IPropertySetCollection collection)
        {
            IProcessRuntime runtime = base.Create(pd, collection);
            Md5CalcVisitor visitor = new Md5CalcVisitor();
            pd.Accept(visitor);
            string md5 = visitor.CalculateMd5();
            // save the collection
            var processDefinitions = _processDefinitionService.ActivetWorkflows();
            var processDefinitionDigest=processDefinitions.SingleOrDefault(d => d.FlowId == pd.FlowId && d.Md5 == md5 && d.Status == ProcessDefStatusEnum.Active);
            if (processDefinitionDigest == null)
            {
                throw new ArgumentException($"Error find Process Definition Id={pd.FlowId} Md5={md5}");
            }
            var rtp = CreateMongoProcessRuntimePersistence(runtime, collection,
                null,
                null,
                processDefinitionDigest);
            _collection.InsertOne(rtp);
           return new ProcessRuntimePersistenProxy(runtime,this);
        }

        /// <summary>
        /// Freeze (persist) the runtime process
        /// </summary>
        /// <param name="runtime"></param>
        /// <param name="collection"></param>
        public override void Freeze(IProcessRuntime runtime, IPropertySetCollection collection)
        {
            var persistedCollection = CreatePersistentPropertyCollection(runtime, collection);
            var filter = Builders<MongoProcessRuntimePersistence>.Filter.Eq(r=>r.Id, runtime.Id);
            var updater = Builders<MongoProcessRuntimePersistence>.Update
                .Set(r => r.Status, (int) runtime.State)
                .Set(r => r.SuspendedStepId, runtime.SuspendedInStep?.StepId)
                .Set(r => r.PropertyCollection, persistedCollection)
                .CurrentDate(r => r.LastUpdated);
            UpdateResult updateResult = _collection.UpdateOne(filter,updater);
            if (updateResult.ModifiedCount != 1)
            {
                throw new ArgumentException($"Cannot modify the process Id={runtime.Id}");
            }
        }

        /// <summary>
        /// Un-freeze the runtime process
        /// </summary>
        /// <param name="processRuntimeId"></param>
        /// <param name="runtime"></param>
        /// <param name="nextStep"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        public override bool TryUnfreeze(Guid processRuntimeId, out IProcessRuntime runtime, out StepRuntime nextStep,
            out IPropertySetCollection collection)
        {
            runtime = null;
            nextStep = null;
            collection = null;
            var filter = Builders<MongoProcessRuntimePersistence>.Filter.Eq(r => r.Id, processRuntimeId);
            MongoProcessRuntimePersistence rtp = _collection.Find(filter).SingleOrDefault();
            if (rtp == null)
            {
                return false;
            }
            var defFilter = Builders<ProcessDefinitionPersistence>.Filter.And(
                Builders<ProcessDefinitionPersistence>.Filter.Eq(r => r.FlowId, rtp.DefinitionFlowId),
                Builders<ProcessDefinitionPersistence>.Filter.Eq(r => r.Md5, rtp.DefinitionMd5)
            );

            ProcessDefinition definition;
            ProcessDefStatusEnum status;
            AccountData[] accounts;
            if (!_processDefinitionService.TryFind(defFilter, out definition, out status, out accounts))
            {
                return false;
            }
            collection = rtp.PropertyCollection.Deserialize();
            if (!string.IsNullOrEmpty(rtp.NextStepId))
            {
                StepDefinition stepDef = definition.Steps.SingleOrDefault(s => s.StepId == rtp.NextStepId);
                nextStep = stepDef == null ? null : new StepRuntime(stepDef);
            }
            runtime = Create(rtp.Id, definition, rtp.SuspendedStepId, (ProcessStateEnum) rtp.Status);
            return true;
        }

        private void OnContinue(IProcessRuntime real,  Tuple<ExecutionResult, StepRuntime> result, IProcessRuntimeEnvironment env)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="runtime"></param>
        /// <param name="result"></param>
        /// <param name="env"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnExecute(IProcessRuntime runtime, Tuple<ExecutionResult, StepRuntime> result, IProcessRuntimeEnvironment env)
        {
            var persistedCollection = CreatePersistentPropertyCollection(runtime, env.PropertySet);
            var filter = Builders<MongoProcessRuntimePersistence>.Filter.Eq(r=>r.Id, runtime.Id);
            var updater = Builders<MongoProcessRuntimePersistence>.Update
                .Set(r => r.Status, (int) runtime.State)
                .Set(r => r.SuspendedStepId, runtime.SuspendedInStep?.StepId)
                .Set(r => r.PropertyCollection, persistedCollection)
                .Set(r=>r.NextStepId,runtime.State==ProcessStateEnum.Completed? null: result.Item2?.StepId)
                .CurrentDate(r => r.LastUpdated);
            UpdateResult updateResult = _collection.UpdateOne(filter,updater);
            if (updateResult.ModifiedCount != 1)
            {
                throw new ArgumentException($"Cannot modify the process Id={runtime.Id}");
            }
        }

        /// <summary>
        /// Create persistent element
        /// </summary>
        /// <param name="runtime"></param>
        /// <param name="collection"></param>
        /// <param name="errors"></param>
        /// <param name="processDefinitionDigest"></param>
        /// <param name="nextStepId"></param>
        /// <returns></returns>
        private MongoProcessRuntimePersistence CreateMongoProcessRuntimePersistence(
            IProcessRuntime runtime,
            IPropertySetCollection collection,
            string nextStepId,
            List<string> errors,
            ProcessDefinitionDigest processDefinitionDigest)
        {
            var persistedCollection = CreatePersistentPropertyCollection(runtime, collection);
            MongoProcessRuntimePersistence rtp = new MongoProcessRuntimePersistence
            {
                Id = runtime.Id,
                SuspendedStepId = runtime.LastExecutedStep?.StepId,
                Status = (int) runtime.State,
                LastUpdated = DateTime.UtcNow,
                PropertyCollection = persistedCollection,
                DefinitionFlowId = processDefinitionDigest.FlowId,
                DefinitionMd5 = processDefinitionDigest.Md5,
                NextStepId = null,
                Errors = errors
            };
            return rtp;
        }

        private static PersistentPropertyCollection CreatePersistentPropertyCollection(IProcessRuntime runtime,
            IPropertySetCollection collection)
        {
            List<PersistentSchemaElement> schemaElements;
            List<PersistentPropertyElement> dataElements;
            collection.CreatePersistentSchemaElements(out schemaElements, out dataElements);
            PersistentPropertyCollection persistedCollection = new PersistentPropertyCollection
            {
                Id = runtime.Id,
                Version = 1,
                Elements = dataElements,
                Schemas = schemaElements
            };
            return persistedCollection;
        }

        private class ProcessRuntimePersistenProxy : IProcessRuntime
        {
            private readonly IProcessRuntime _real;
            private readonly MongoProcessRuntimePersistenceService _persistenceService;
            public Guid Id => _real.Id;

            public ProcessStateEnum State => _real.State;

            public StepRuntime SuspendedInStep => _real.SuspendedInStep;

            public StepRuntime LastExecutedStep => _real.LastExecutedStep;

            public IReadOnlyList<StepRuntime> StartSteps => _real.StartSteps;

            public IReadOnlyList<string> Errors => _real.Errors;

            public ProcessRuntimePersistenProxy(IProcessRuntime real,
                MongoProcessRuntimePersistenceService persistenceService)
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