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