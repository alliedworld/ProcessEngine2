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
using KlaudWerk.ProcessEngine.Builder;

namespace KlaudWerk.ProcessEngine.Definition
{
    /// <summary>
    /// Variable definitions
    /// </summary>
    [Serializable]
    [DataContract(Name = "variable")]
    public class VariableDefinition:IProcessDefinitionVisitable
    {
        [DataMember(Name="name")]
        public string Name { get; set; }
        [DataMember(Name="desc")]
        public string Description { get; set; }
        [DataMember(Name="type")]
        public VariableTypeEnum VariableType { get; set; }
        [DataMember(Name = "handler")]
        public StepHandlerDefinition HandlerDefinition { get; set; }

        ///  <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">variable name</param>
        /// <param name="description">variable description</param>
        /// <param name="variableType">variable type</param>
        /// <param name="handler">a handler that can be use to validate or to provide list of possibel values</param>
        public VariableDefinition(
            string name, string description,
            VariableTypeEnum variableType, 
            StepHandlerDefinition handler)
        {
            Name = name;
            Description = description;
            VariableType = variableType;
            HandlerDefinition = handler;
        }
        /// <summary>
        ///
        /// </summary>
        public VariableDefinition()
        {
        }

        public void Accept(IProcessDefinitionVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}