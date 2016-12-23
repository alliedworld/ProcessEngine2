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