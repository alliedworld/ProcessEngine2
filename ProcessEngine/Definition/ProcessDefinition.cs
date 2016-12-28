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
using System.Runtime.Serialization;

namespace KlaudWerk.ProcessEngine.Definition
{
    /// <summary>
    /// Serializable immutable definition of a process
    /// </summary>
    [Serializable]
    [DataContract(Name="process")]
    public class ProcessDefinition:IProcessDefinitionVisitable
    {
        /// <summary>
        /// Gets the identifier of the process.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [DataMember(Name="id")]
        public Guid Id { get; set; }
        /// <summary>
        /// Gets the flow identifier.
        /// </summary>
        /// <value>
        /// The flow identifier.
        /// </value>
        [DataMember(Name="flow_id")]
        public string FlowId { get; set;}
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [DataMember(Name="name")]
        public string Name { get; set;}
        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        [DataMember(Name="desc")]
        public string Description { get; set;}
        /// <summary>
        /// Gets the process steps.
        /// </summary>
        /// <value>
        /// The steps.
        /// </value>
        [DataMember(Name="steps")]
        public StepDefinition[] Steps { get; set;}
        /// <summary>
        /// Gets the process links.
        /// </summary>
        /// <value>
        /// The links.
        /// </value>
        [DataMember(Name="links")]
        public LinkDefinition[] Links { get; set;}
        /// <summary>
        ///
        /// </summary>
        [DataMember(Name="vars")]
        public VariableDefinition[] Variables { get; set;}
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessDefinition"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="flowId">The flow identifier.</param>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        /// <param name="steps">The steps.</param>
        /// <param name="links">The links.</param>
        /// <param name="variables"></param>
        public ProcessDefinition(Guid id, string flowId, string name, string description,
            StepDefinition[] steps,
            LinkDefinition[] links,
            VariableDefinition[] variables
            )
        {
            Id = id;
            FlowId = flowId;
            Name = name;
            Description = description;
            Steps = steps;
            Links = links;
            Variables = variables;
        }
        /// <summary>
        /// Default constructor
        /// </summary>
        public ProcessDefinition()
        {
        }

        /// <summary>
        /// Accept the visitor
        /// </summary>
        /// <param name="visitor"></param>
        public void Accept(IProcessDefinitionVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
