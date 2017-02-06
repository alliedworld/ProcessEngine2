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
using KlaudWerk.ProcessEngine.Definition;

namespace KlaudWerk.ProcessEngine.Builder
{
    /// <summary>
    /// Process builder allows to use fluent interface to build a Process Definition
    /// </summary>
    public class ProcessBuilder
    {
        private readonly Dictionary<string,StepContainer> _steps=new Dictionary<string, StepContainer>();
        private readonly Dictionary<Tuple<string,string>,LinkBuilder> _links=new Dictionary<Tuple<string, string>, LinkBuilder>();
        private readonly List<VariableBuilder> _variables=new List<VariableBuilder>();
        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; }
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; }
        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; }

        /// <summary>
        /// Gloabal "Due In Days" value
        /// </summary>
        public int? DueInDays { get; private set; }
        /// <summary>
        /// Gets the start steps defined in the process.
        /// </summary>
        /// <value>
        /// The start steps.
        /// </value>
        public IReadOnlyList<StepBuilder> StartSteps => _steps.Values.Where(v => v.StepType == StepTypeEnum.Start)
            .Select(s => s.Builder)
            .ToList();
        /// <summary>
        /// Gets the end steps defined in the process.
        /// </summary>
        /// <value>
        /// The end steps.
        /// </value>
        public IReadOnlyList<StepBuilder> EndSteps => _steps.Values.Where(v => v.StepType == StepTypeEnum.End)
            .Select(s => s.Builder)
            .ToList();
        /// <summary>
        /// Gets the steps.
        /// </summary>
        /// <value>
        /// The steps.
        /// </value>
        public IReadOnlyList<Tuple<StepBuilder, StepTypeEnum>> Steps
            => _steps.Values.Select(s => new Tuple<StepBuilder, StepTypeEnum>(s.Builder, s.StepType)).ToList();
        /// <summary>
        /// Gets the links.
        /// </summary>
        /// <value>
        /// The links.
        /// </value>
        public IReadOnlyList<LinkBuilder> Links => _links.Values.ToList();
        /// <summary>
        /// Gets the process variables.
        /// </summary>
        /// <value>
        /// The process variables.
        /// </value>
        public IReadOnlyList<VariableBuilder> ProcessVariables => _variables.ToList();

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessBuilder"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public ProcessBuilder(string id):this(id,string.Empty,string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessBuilder"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        public ProcessBuilder(string id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }

        /// <summary>
        /// Set global "Due in Days" value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ProcessBuilder SetDueInDays(int value)
        {

            DueInDays = value;
            return this;
        }
        /// <summary>
        /// Create Start Step,
        /// </summary>
        /// <param name="id"></param>
        /// <returns><see cref="StepBuilder"/></returns>
        /// <exception cref="ArgumentException"></exception>
        public StepBuilder Start(string id)
        {
            return AddStep(id, StepTypeEnum.Start);
        }

        /// <summary>
        /// Create End Step.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns><see cref="StepBuilder"/></returns>
        public StepBuilder End(string id)
        {
            return AddStep(id, StepTypeEnum.End);
        }

        /// <summary>
        /// Create a Step Builder
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns><see cref="StepBuilder"/></returns>
        public StepBuilder Step(string id)
        {
            return AddStep(id, StepTypeEnum.Regular);
        }

        /// <summary>
        /// Tries the find step.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="step">The step.</param>
        /// <returns>true if a step can be found. false otherwise</returns>
        public bool TryFindStep(string id, out Tuple<StepBuilder, StepTypeEnum> step)
        {
            step = null;
            StepContainer sc;
            if (_steps.TryGetValue(id, out sc))
                step=new Tuple<StepBuilder, StepTypeEnum>(sc.Builder,sc.StepType);
            return step != null;
        }
        /// <summary>
        /// Create Link Builder
        /// </summary>
        /// <returns><see cref="LinkBuilder"/></returns>
        public LinkBuilder Link()
        {
            return new LinkBuilder(this);
        }

        /// <summary>
        /// Tries to find a link.
        /// </summary>
        /// <param name="fromStepId">From step identifier.</param>
        /// <param name="toStepId">To step identifier.</param>
        /// <param name="link">The link.</param>
        /// <returns>true if the link was found</returns>
        public bool TryFindLink(string fromStepId, string toStepId, out LinkBuilder link)
        {
             Tuple<string,string> key=new Tuple<string, string>(fromStepId,toStepId);
            return _links.TryGetValue(key, out link);
        }

        /// <summary>
        /// Adds the link.
        /// </summary>
        /// <param name="linkBuilder">The link builder.</param>
        /// <exception cref="ArgumentException">
        /// The link does not have \Form\ step property.
        /// or
        /// The link does not have \To\ step property.
        /// or
        /// </exception>
        public void AddLink(LinkBuilder linkBuilder)
        {
            if(linkBuilder.StepFrom==null)
                throw new ArgumentException("The link does not have \"Form\" step property.");
            if(linkBuilder.StepTo==null)
                throw new ArgumentException("The link does not have \"To\" step property.");
            LinkBuilder link;
            if(TryFindLink(linkBuilder.StepFrom.Id,linkBuilder.StepTo.Id, out link))
                throw new ArgumentException($"The link from {linkBuilder.StepFrom.Id} to {linkBuilder.StepTo.Id} already exist.");
            _links[new Tuple<string, string>(linkBuilder.StepFrom.Id,linkBuilder.StepTo.Id)] = linkBuilder;
        }

        /// <summary>
        /// Gets the outgoing liks from a step
        /// </summary>
        /// <param name="sourceStepId">The source step identifier.</param>
        /// <returns></returns>
        public IReadOnlyList<LinkBuilder> GetLinksFrom(string sourceStepId)
        {
            List<LinkBuilder> foundLinks=new List<LinkBuilder>();
            foundLinks.AddRange(_links.Where(l=>l.Key.Item1==sourceStepId).Select(l=>l.Value));
            return foundLinks;
        }

        /// <summary>
        /// Gets incoming links to a stewp.
        /// </summary>
        /// <param name="targetStepId">The target step identifier.</param>
        /// <returns></returns>
        public IReadOnlyList<LinkBuilder> GetLinksTo(string targetStepId)
        {
            List<LinkBuilder> foundLinks=new List<LinkBuilder>();
            foundLinks.AddRange(_links.Where(l=>l.Key.Item2==targetStepId).Select(l=>l.Value));
            return foundLinks;
        }

        /// <summary>
        /// Tries to validate the builder.
        /// </summary>
        /// <param name="validationResults">The validation results.</param>
        /// <returns></returns>
        public bool TryValidate(out IReadOnlyList<ProcessValidationResult> validationResults)
        {
            validationResults = null;
            List<ProcessValidationResult> results=new List<ProcessValidationResult>();
            results.AddRange(ValidateEmpty());
            if (results.Count == 0)
            {
                results.AddRange(ValidateStartStep());
                results.AddRange(ValidateEndStep());
                results.AddRange(ValidateSteps());
                results.AddRange(ValidateLinks());
            }
            validationResults = results;
            return results.Count==0;
        }

        /// <summary>
        /// Start building process variable
        /// </summary>
        /// <returns></returns>
        public VariableBuilder Variables()
        {
            return new VariableBuilder(this);
        }
        /// <summary>
        /// Builds the process defintion
        /// Process definition may be serialized and stored
        /// </summary>
        /// <returns><see cref="ProcessDefinition"/></returns>
        public ProcessDefinition Build()
        {
            var stepDefs = BuildStepDefinitions();
            var linkDefs = BuildLiknsDefinition(stepDefs);
            var variables=BuildVariables();
            return new ProcessDefinition(Guid.NewGuid(), Id, Name, Description, stepDefs, linkDefs, variables);
        }


        /// <summary>
        /// Removes the specified step builder.
        /// </summary>
        /// <param name="stepBuilder">The step builder.</param>
        /// <returns></returns>
        internal bool Remove(StepBuilder stepBuilder)
        {
            if (_steps.Remove(stepBuilder.Id))
            {
                var outgoings = _links.Where(l => l.Key.Item1 == stepBuilder.Id).Select(l => l.Key).ToList();
                var incoming = _links.Where(l => l.Key.Item2 == stepBuilder.Id).Select(l => l.Key).ToList();
                foreach (var k in outgoings)
                    _links.Remove(k);
                foreach (var k in incoming)
                    _links.Remove(k);
            }
            return false;
        }

        /// <summary>
        /// Add or replace the variable in the builder
        /// </summary>
        /// <param name="variableBuilder"></param>
        /// <returns></returns>
        internal ProcessBuilder AddOrReplaceVariable(VariableBuilder variableBuilder)
        {
            var variable = _variables.SingleOrDefault(v => v.VariableName == variableBuilder.VariableName);
            if(variable!=null)
                _variables.Remove(variable);
            _variables.Add(variableBuilder);
            return this;
        }


        #region Validation Methods

        private static readonly ProcessValidationResult[] EmptyResult = {};
        private IEnumerable<ProcessValidationResult> ValidateEmpty()
        {
            return _links.Count == 0 && _steps.Count == 0
                ? new[]
                {
                    new ProcessValidationResult(ProcessValidationResult.ItemEnum.Process, Id,
                        $"Process Definition {Id} {Name} {Description} is empty")
                }
                : EmptyResult;
        }
        private IEnumerable<ProcessValidationResult> ValidateEndStep()
        {
            return EndSteps.Count == 0 ? new[]
            {
                new ProcessValidationResult(ProcessValidationResult.ItemEnum.Process, Id,
                    $"Process Definition {Id} {Name} {Description} does not have End Steps")
            } : EmptyResult;
        }

        private IEnumerable<ProcessValidationResult> ValidateStartStep()
        {
            return StartSteps.Count == 0 ? new[]
            {
                new ProcessValidationResult(ProcessValidationResult.ItemEnum.Process, Id,
                    $"Process Definition {Id} {Name} {Description} does not have Start Steps")
            } : EmptyResult;
        }
        private IEnumerable<ProcessValidationResult> ValidateSteps()
        {
            List<ProcessValidationResult> results=new List<ProcessValidationResult>();
            foreach (Tuple<StepBuilder,StepTypeEnum> step in Steps)
            {
                results.AddRange(from variableMapBuilder in step.Item1.RequiredVars
                    where _variables.Count(v => v.VariableName == variableMapBuilder.VarName) == 0
                    select new ProcessValidationResult(ProcessValidationResult.ItemEnum.Step, step.Item1.Id,
                        $"Variable {variableMapBuilder.VarName} defined as 'required' for Step {step.Item1.Id}"+
                        " is not defiend for this Process"));
            }
            return results;
        }

        private IEnumerable<ProcessValidationResult> ValidateLinks()
        {
            List<ProcessValidationResult> results=new List<ProcessValidationResult>();
            // Validate that all "start" steps have outgoing links
            foreach (var sb in StartSteps)
                if(_links.Count(l=>l.Key.Item1==sb.Id)==0)
                    results.Add(new ProcessValidationResult(ProcessValidationResult.ItemEnum.Link,Id,
                    $"Start Step {sb.Id} {sb.Name} does not have outgoing links"));
            // Validate that all "end" steps have incoming links
            foreach(var sb in EndSteps)
                if(_links.Count(l=>l.Key.Item2==sb.Id)==0)
                    results.Add(new ProcessValidationResult(ProcessValidationResult.ItemEnum.Link,Id,
                        $"End Step {sb.Id} {sb.Name} does not have incoming links"));
            // Validate that all other steps have incoming and outgoing links
            foreach (var sb in Steps.Where(l=>l.Item2!=StepTypeEnum.End && l.Item2!=StepTypeEnum.Start).Select(s=>s.Item1))
            {
                if(_links.Count(l=>l.Key.Item1==sb.Id)==0)
                    results.Add(new ProcessValidationResult(ProcessValidationResult.ItemEnum.Link,Id,
                        $"Step {sb.Id} {sb.Name} does not have outgoing links"));
                if(_links.Count(l=>l.Key.Item2==sb.Id)==0)
                    results.Add(new ProcessValidationResult(ProcessValidationResult.ItemEnum.Link,Id,
                        $"Step {sb.Id} {sb.Name} does not have incoming links"));

            }
            return results;
        }
#endregion

        #region Private Methods
        private StepBuilder AddStep(string id,StepTypeEnum stepType)
        {
            if(_steps.ContainsKey(id))
                throw new ArgumentException($"Step with id={id} already exists");
            var container=new StepContainer(new StepBuilder(this,id),stepType);
            _steps[id] = container;
            return container.Builder;
        }

        private class StepContainer
        {
            internal StepBuilder Builder { get;  private set; }
            internal StepTypeEnum StepType { get; private set; }
            internal StepContainer(StepBuilder builder, StepTypeEnum stepTypeEnum)
            {
                Builder = builder;
                StepType = stepTypeEnum;
            }
        }

        private StepDefinition[] BuildStepDefinitions()
        {
            List<StepDefinition> steps=new List<StepDefinition>();
            foreach (StepContainer container in _steps.Values)
            {
                StepBuilder sb = container.Builder;
                if (sb.RequiredVars != null)
                {
                    StepDefinition sd = new StepDefinition(id: GenerateStepId(), stepId: sb.Id, description: sb.Description,
                        actions:sb.Actions.Select(a=>new ActionDefinition(a.ActionName,a.ActionDescription,a.IsSkippable)).ToArray(),
                        name:sb.Name,isStart:container.StepType==StepTypeEnum.Start,isEnd:container.StepType==StepTypeEnum.End,
                        potentialOwners:BuildPotentialOwners(sb),
                        businessManagers:BuildBusinessManagers(sb),
                        onEntry:BuildScript(sb.OnEntry()),
                        onExit:BuildScript(sb.OnExit()),
                        varMaps:BuildVarMaps(sb.RequiredVars),
                        stepHandler:BuildStepHandler(sb.StepHandler));
                    steps.Add(sd);
                }
            }
            return steps.ToArray();
        }

        private StepHandlerDefinition BuildStepHandler(StepHandlerBuilder sbStepHandler)
        {
            return new StepHandlerDefinition(stepHandlerType:sbStepHandler.StepHandlerType,
            script:sbStepHandler.ScriptBuilder==null?null:BuildScript(sbStepHandler.ScriptBuilder),
                iocName:sbStepHandler.IocName,
                classFullName:sbStepHandler.FullClassName);
        }

        private VariableMapDefinition[] BuildVarMaps(IReadOnlyList<VariableMapBuilder> sbRequiredVars)
        {
            return sbRequiredVars.Select(v => new VariableMapDefinition(v.VarName,v.VarRequire)).ToArray();
        }

        private LinkDefinition[] BuildLiknsDefinition(StepDefinition[] steps)
        {
            List<LinkDefinition> links=new List<LinkDefinition>();
            foreach (LinkBuilder linkBuilder in Links)
            {
                var from = steps.First(s => s.StepId == linkBuilder.StepFrom.Id);
                var to=steps.First(s => s.StepId == linkBuilder.StepTo.Id);
                links.Add(new LinkDefinition(
                    new StepDefinitionId(from.Id,from.StepId),new StepDefinitionId(to.Id,to.StepId)
                    ,linkBuilder.LinkName,linkBuilder.LinkDescription,
                BuildScript(linkBuilder.Script())));
            }
            return links.ToArray();
        }

        private ScriptDefinition BuildScript<T>(ScriptBuilder<T> scriptBuilder)
        {
           return new ScriptDefinition(scriptBuilder.ScriptBody,
               scriptBuilder.ScriptLanguage,scriptBuilder.Imports.ToArray(),scriptBuilder.References.ToArray());
        }

        private SecurityDefinition[] BuildBusinessManagers(StepBuilder sb)
        {
            var s=sb.Security();
            return s?.BusinessAdministrators.Select(a => new SecurityDefinition(a.Item1, a.Item2)).ToArray() ??
                   new SecurityDefinition[]{};
        }

        private SecurityDefinition[] BuildPotentialOwners(StepBuilder sb)
        {
            var s=sb.Security();
            return s?.PotentialOwners.Select(a => new SecurityDefinition(a.Item1, a.Item2)).ToArray() ??
                   new SecurityDefinition[]{};
        }

        private Guid GenerateStepId()
        {
            return Guid.NewGuid();
        }

        private VariableDefinition[] BuildVariables()
        {
            return _variables.Select(variableBuilder => new VariableDefinition(variableBuilder.VariableName,
                variableBuilder.VariableDescription, variableBuilder.VariableType,
                variableBuilder.VariableSourceClass)).ToArray();
        }

        #endregion
    }
}