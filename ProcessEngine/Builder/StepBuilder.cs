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

namespace KlaudWerk.ProcessEngine.Builder
{
    /// <summary>
    /// Step Builder allows to build individual definition of a step 
    /// it a workflow
    /// </summary>
    public class StepBuilder
    {
        private readonly ProcessBuilder _parent;
        private ScriptBuilder<StepBuilder> _onEntry;
        private ScriptBuilder<StepBuilder> _onExit;
        private SecurityBuilder<StepBuilder> _securityBuilder;
        private readonly List<StepActionBuilder> _actions = new List<StepActionBuilder>();
        private readonly List<VariableMapBuilder> _requiredVars=new List<VariableMapBuilder>();
        /// <summary>
        /// Step handler
        /// By default, every step builder constructs empty step handler
        /// </summary>
        public StepHandlerBuilder StepHandler { get; }
        /// <summary>
        /// List of required variables
        /// </summary>
        public IReadOnlyList<VariableMapBuilder> RequiredVars => _requiredVars;
        /// <summary>
        /// Gets the actions.
        /// </summary>
        /// <value>
        /// The actions.
        /// </value>
        public IReadOnlyList<StepActionBuilder> Actions => _actions;
        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; private set; }
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; private set; }
        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StepBuilder"/> class.
        /// </summary>
        /// <param name="parent">Parent Builder</param>
        /// <param name="id">The identifier.</param>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        public StepBuilder(ProcessBuilder parent, string id, string name = "", string description = "")
        {
            parent.NotNull("parent");
            _parent = parent;
            id.NotNull("id").NotEmptyString("id");
            Id = id;
            Name = name;
            Description = description;
            StepHandler=new StepHandlerBuilder(this);
        }
        /// <summary>
        /// Sets the name.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public StepBuilder SetName(string value)
        {
            Name = value;
            return this;
        }
        /// <summary>
        /// Sets the description.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public StepBuilder SetDescription(string value)
        {
            Description = value;
            return this;
        }
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public VariableMapBuilder Vars()
        {
            return new VariableMapBuilder(this);
        }
        /// <summary>
        /// On Entry Script.
        /// </summary>
        /// <returns></returns>
        public ScriptBuilder<StepBuilder> OnEntry()
        {
            return _onEntry ?? (_onEntry = new ScriptBuilder<StepBuilder>(this));
        }
        /// <summary>
        /// On Exit Script
        /// </summary>
        /// <returns></returns>
        public ScriptBuilder<StepBuilder> OnExit()
        {
            return _onExit ?? (_onExit = new ScriptBuilder<StepBuilder>(this));
        }

        /// <summary>
        /// Security Builder
        /// </summary>
        /// <returns></returns>
        public SecurityBuilder<StepBuilder> Security()
        {
            return _securityBuilder ??
                   (_securityBuilder = new SecurityBuilder<StepBuilder>(this));
        }
        /// <summary>
        ///  Start Building an Action
        /// </summary>
        /// <returns></returns>
        public StepActionBuilder Action()
        {
            return new StepActionBuilder(this);
        }

        /// <summary>
        /// Step Handler
        /// </summary>
        /// <returns></returns>
        public StepHandlerBuilder Handler()
        {
            return StepHandler;
        }
        /// <summary>
        /// Adds the replace action.
        /// </summary>
        /// <param name="stepActionBuilder">The step action builder.</param>
        internal void AddReplaceAction(StepActionBuilder stepActionBuilder)
        {
           var action = _actions.SingleOrDefault(t => t.ActionName == stepActionBuilder.ActionName);
            if (action != null)
                _actions.Remove(action);
            _actions.Add(stepActionBuilder);
        }

        /// <summary>
        /// Removes the action.
        /// </summary>
        /// <param name="stepActionBuilder">The step action builder.</param>
        /// <exception cref="NotImplementedException"></exception>
        internal void RemoveAction(StepActionBuilder stepActionBuilder)
        {
            var action = _actions.SingleOrDefault(t => t.ActionName == stepActionBuilder.ActionName);
            if (action != null)
                _actions.Remove(action);
        }

        /// <summary>
        /// Removes this instance.
        /// </summary>
        /// <returns></returns>
        public ProcessBuilder Remove()
        {
            _parent.Remove(this);
            return _parent;
        }
        /// <summary>
        /// Done Building
        /// </summary>
        /// <returns></returns>
        public ProcessBuilder Done()
        {
            return _parent;
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="varBuilder"></param>
        internal void AddReplaceVarMap(VariableMapBuilder varBuilder)
        {
            RemoveVarMap(varBuilder);
           _requiredVars.Add(varBuilder);
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="varBuilder"></param>
        internal void RemoveVarMap(VariableMapBuilder varBuilder)
        {
            var rv = _requiredVars.SingleOrDefault(v => v.VarName == varBuilder.VarName);
            if (rv != null)
                _requiredVars.Remove(rv);

        }
    }
}
