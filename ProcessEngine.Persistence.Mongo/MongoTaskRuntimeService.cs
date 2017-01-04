using System;
using KlaudWerk.ProcessEngine;
using KlaudWerk.ProcessEngine.Persistence;
using KlaudWerk.ProcessEngine.Service;
using MongoDB.Driver;

namespace Klaudwerk.ProcessEngine.Persistence.Mongo
{
    /// <summary>
    /// Mongo implementation of the service
    /// </summary>
    public class MongoTaskRuntimeService:TaskRuntimeServiceBase
    {
        public const string CollectionName = "task_runtime";
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<TaskData> _collection;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="database"></param>
        public MongoTaskRuntimeService(IMongoDatabase database)
        {
            _database = database;
            _collection = _database.GetCollection<TaskData>(CollectionName);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="data">task data</param>
        protected override void OnCreateTask(TaskData data)
        {
            var filter = Builders<TaskData>.Filter.And(
                Builders<TaskData>.Filter.Eq(r => r.ProcessId, data.ProcessId),
                Builders<TaskData>.Filter.Eq(r => r.StepId, data.StepId),
                Builders<TaskData>.Filter.Eq(r => r.Status, TaskStatus.Ready)
            );
            if (_collection.Find(filter).Count() > 0)
            {
                throw new ArgumentException($"Active Task for Process Id={data.ProcessId} Step={data.StepId} already exist.");
            }
            _collection.InsertOne(data);
        }
    }
}