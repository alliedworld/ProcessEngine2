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
    /// Serializable immutable definition ofa Script
    /// </summary>
    [DataContract(Name="script")]
    [Serializable]
    public class ScriptDefinition:IProcessDefinitionVisitable
    {
        /// <summary>
        /// Gets the script.
        /// </summary>
        /// <value>
        /// The script.
        /// </value>
        [DataMember(Name="script")]
        public string Script { get; }
        /// <summary>
        /// Gets the language.
        /// </summary>
        /// <value>
        /// The language.
        /// </value>
        [DataMember(Name="lang")]
        public ScriptLanguage Lang { get; }
        /// <summary>
        /// Gets the imports.
        /// </summary>
        /// <value>
        /// The imports.
        /// </value>
        [DataMember(Name="imports")]
        public string[] Imports { get; }
        /// <summary>
        /// Gets the references.
        /// </summary>
        /// <value>
        /// The references.
        /// </value>
        [DataMember(Name="refs")]
        public string[] References { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptDefinition"/> class.
        /// </summary>
        /// <param name="script">The script.</param>
        /// <param name="lang">The language.</param>
        /// <param name="imports">The imports.</param>
        /// <param name="references">The references.</param>
        public ScriptDefinition(string script, ScriptLanguage lang, string[] imports, string[] references)
        {
            Script = script;
            Lang = lang;
            Imports = imports;
            References = references;
        }
        /// <summary>
        /// DefaultConstructor
        /// </summary>
        public ScriptDefinition()
        {
        }

        public void Accept(IProcessDefinitionVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}