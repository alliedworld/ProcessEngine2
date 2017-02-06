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
using System.Linq;
using System.Runtime.Serialization;

namespace KlaudWerk.ProcessEngine.Definition
{
    /// <summary>
    /// Serializable class that defines a step in a process
    /// </summary>
    [DataContract(Name="step")]
    [Serializable]
    public class StepDefinition:IProcessDefinitionVisitable
    {
        /// <summary>
        /// Gets the actions.
        /// </summary>
        /// <value>
        /// The actions.
        /// </value>
        [DataMember(Name = "actions")]
        public ActionDefinition[] Actions { get; set; }
        /// <summary>
        /// Gets the on exit.
        /// </summary>
        /// <value>
        /// The on exit.
        /// </value>
        [DataMember(Name = "script_exit")]
        public ScriptDefinition OnExit { get; set; }
        /// <summary>
        /// Gets the on entry.
        /// </summary>
        /// <value>
        /// The on entry.
        /// </value>
        [DataMember(Name = "script_entry")]
        public ScriptDefinition OnEntry { get; set; }
        /// <summary>
        /// Gets the business managers accounts.
        /// </summary>
        /// <value>
        /// The business managers.
        /// </value>
        [DataMember(Name = "business_mgr")]
        public SecurityDefinition[] BusinessManagers { get; set; }
        /// <summary>
        /// Gets the potential owners accounts.
        /// </summary>
        /// <value>
        /// The potential owners.
        /// </value>
        [DataMember(Name = "potential_owners")]
        public SecurityDefinition[] PotentialOwners { get; set; }
        /// <summary>
        /// Gets a value indicating whether this instance is end.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is end; otherwise, <c>false</c>.
        /// </value>
        [DataMember(Name = "is_end_step")]
        public bool IsEnd { get; set; }
        /// <summary>
        /// Gets a value indicating whether this instance is start.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is start; otherwise, <c>false</c>.
        /// </value>
        [DataMember(Name = "is_end_start")]
        public bool IsStart { get; set; }
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [DataMember(Name = "name")]
        public string Name { get; set; }
        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        [DataMember(Name = "descr")]
        public string Description { get; set; }
        /// <summary>
        /// Gets the step identifier.
        /// </summary>
        /// <value>
        /// The step identifier.
        /// </value>
        [DataMember(Name = "step_id")]
        public string StepId { get; set; }
        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [DataMember(Name = "id")]
        public Guid Id { get; set; }
        /// <summary>
        /// Step Variables Requirements
        /// </summary>
        [DataMember(Name = "vars_map")]
        public VariableMapDefinition[] VariablesMap { get; set; }
        /// <summary>
        ///
        /// </summary>
        [DataMember(Name = "handler")]
        public StepHandlerDefinition StepHandler { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StepDefinition"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="stepId">The step identifier.</param>
        /// <param name="description">The description.</param>
        /// <param name="name">The name.</param>
        /// <param name="isStart">if set to <c>true</c> [is start].</param>
        /// <param name="isEnd">if set to <c>true</c> [is end].</param>
        /// <param name="potentialOwners">The potential owners.</param>
        /// <param name="businessManagers">The business managers.</param>
        /// <param name="onEntry">The on entry.</param>
        /// <param name="onExit">The on exit.</param>
        /// <param name="actions">The actions.</param>
        /// <param name="varMaps"></param>
        /// <param name="stepHandler"></param>
        public StepDefinition(Guid id, string stepId, string description, string name,
            bool isStart, bool isEnd,
            SecurityDefinition[] potentialOwners,
            SecurityDefinition[] businessManagers,
            ScriptDefinition onEntry,
            ScriptDefinition onExit,
            ActionDefinition[] actions,
            VariableMapDefinition[] varMaps,
            StepHandlerDefinition stepHandler)
        {
            Id = id;
            StepId = stepId;
            Description = description;
            Name = name;
            IsStart = isStart;
            IsEnd = isEnd;
            PotentialOwners = potentialOwners;
            BusinessManagers = businessManagers;
            OnEntry = onEntry;
            OnExit = onExit;
            Actions = actions;
            VariablesMap = varMaps;
            StepHandler = stepHandler;
        }


        /// <summary>
        /// Default constructor
        /// </summary>
        public StepDefinition()
        {
        }

        public void Accept(IProcessDefinitionVisitor visitor)
        {
            Actions?.ToList().ForEach(a=>a.Accept(visitor));
            OnEntry?.Accept(visitor);
            OnExit?.Accept(visitor);
            BusinessManagers?.ToList().ForEach(m=>m.Accept(visitor));
            PotentialOwners?.ToList().ForEach(o=>o.Accept(visitor));
            VariablesMap?.ToList().ForEach(m=>m.Accept(visitor));
            StepHandler.Accept(visitor);
            visitor.Visit(this);
        }
    }
}