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
    /// Serializable immutable link definition
    /// </summary>
    [DataContract(Name="link")]
    [Serializable]
    public class LinkDefinition:IProcessDefinitionVisitable
    {
        /// <summary>
        /// Gets the source.
        /// </summary>
        /// <value>
        /// The source.
        /// </value>
        [DataMember(Name="src")]
        public StepDefinitionId Source { get; set; }
        /// <summary>
        /// Gets the target.
        /// </summary>
        /// <value>
        /// The target.
        /// </value>
        [DataMember(Name="target")]
        public StepDefinitionId Target { get; set;}
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
        /// Gets the script.
        /// </summary>
        /// <value>
        /// The script.
        /// </value>
        [DataMember(Name="script")]
        public ScriptDefinition Script { get; set;}

        /// <summary>
        /// Initializes a new instance of the <see cref="LinkDefinition"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        /// <param name="script">The script.</param>
        public LinkDefinition(StepDefinitionId source, StepDefinitionId target, string name, string description, ScriptDefinition script)
        {
            Source = source;
            Target = target;
            Name = name;
            Description = description;
            Script = script;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public LinkDefinition()
        {
        }

        public override string ToString()
        {
            return $"Link: {Source}->{Target} [{Name} {Description}] {Script}";
        }

        public void Accept(IProcessDefinitionVisitor visitor)
        {
            Script?.Accept(visitor);
            visitor.Visit(this);
        }
    }
}