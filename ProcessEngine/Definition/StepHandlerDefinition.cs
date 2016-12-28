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
    /// Step handler defines the logic of step execution
    /// </summary>
    [DataContract(Name = "handler")]
    [Serializable]
    public class StepHandlerDefinition:IProcessDefinitionVisitable
    {
        /// <summary>
        /// Gets the type of the step handler.
        /// </summary>
        /// <value>
        /// The type of the step handler.
        /// </value>
        [DataMember(Name = "type")]
        public StepHandlerTypeEnum StepHandlerType { get; set; }
        /// <summary>
        /// Gets the script.
        /// </summary>
        /// <value>
        /// The script.
        /// </value>
        [DataMember(Name = "script")]
        public ScriptDefinition    Script { get; set; }
        /// <summary>
        /// Gets the name of the ioc.
        /// </summary>
        /// <value>
        /// The name of the ioc.
        /// </value>
        [DataMember(Name = "ioc_name")]
        public string IocName { get; set; }
        /// <summary>
        /// Gets the full name of the class.
        /// </summary>
        /// <value>
        /// The full name of the class.
        /// </value>
        [DataMember(Name = "class_name")]
        public string ClassFullName { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StepHandlerDefinition"/> class.
        /// </summary>
        /// <param name="stepHandlerType">Type of the step handler.</param>
        /// <param name="script">The script.</param>
        /// <param name="iocName">Name of the ioc.</param>
        /// <param name="classFullName">Full name of the class.</param>
        public StepHandlerDefinition(StepHandlerTypeEnum stepHandlerType,
            ScriptDefinition script,
            string iocName,
            string classFullName)
        {
            StepHandlerType = stepHandlerType;
            Script = script;
            IocName = iocName;
            ClassFullName = classFullName;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public StepHandlerDefinition()
        {
        }

        public void Accept(IProcessDefinitionVisitor visitor)
        {
            Script?.Accept(visitor);
            visitor.Visit(this);
        }
    }
}