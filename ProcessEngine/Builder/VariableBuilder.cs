using System;

namespace KlaudWerk.ProcessEngine.Builder
{
    /// <summary>
    /// Process Variables Builder
    /// </summary>
    public class VariableBuilder
    {
        private readonly ProcessBuilder _parent;
        private string _name;
        private string _description;
        private VariableTypeEnum _type;
        private string _sourceClass;
        private ConstraintBuilder _constraints;
        /// <summary>
        /// Public Properties
        /// </summary>
        /// <value>
        /// The name of the variable.
        /// </value>
        public string VariableName => _name;
        /// <summary>
        /// Gets the variable source class.
        /// </summary>
        /// <value>
        /// The variable source class.
        /// </value>
        public string VariableSourceClass => _sourceClass;
        /// <summary>
        /// Gets the type of the variable.
        /// </summary>
        /// <value>
        /// The type of the variable.
        /// </value>
        public VariableTypeEnum VariableType => _type;
        /// <summary>
        /// Gets the variable description.
        /// </summary>
        /// <value>
        /// The variable description.
        /// </value>
        public string VariableDescription => _description;
        /// <summary>
        /// Gets the variable constraints.
        /// </summary>
        /// <value>
        /// The variable constraints.
        /// </value>
        public ConstraintBuilder VariableConstraints => _constraints;

        /// <summary>
        /// Initializes a new instance of the <see cref="VariableBuilder"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public VariableBuilder(ProcessBuilder parent)
        {
            _parent = parent;
            _type=VariableTypeEnum.String;
        }

        /// <summary>
        /// Set Name 
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public VariableBuilder Name(string name)
        {
            name.NotNull("name").NotEmptyString("name");
            _name = name;
            return this;
        }
        /// <summary>
        /// Set Description
        /// </summary>
        /// <param name="description">The description.</param>
        /// <returns></returns>
        public VariableBuilder Description(string description)
        {
            _description = description;
            return this;
        }

        /// <summary>
        /// Set Type
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns></returns>
        public VariableBuilder Type(VariableTypeEnum t)
        {
            _type = t;
            return this;
        }

        /// <summary>
        /// Set the Sources class name.
        /// </summary>
        /// <param name="srcClass">The source class.</param>
        /// <returns></returns>
        public VariableBuilder SourceClass(string srcClass)
        {
            _sourceClass = srcClass;
            return this;
        }

        /// <summary>
        /// Finish the builder
        /// </summary>
        /// <returns></returns>
        public ProcessBuilder Done()
        {
            _name.NotNull("name").NotEmptyString("name");
            _parent.AddOrReplaceVariable(this);
            return _parent;
        }

        /// <summary>
        /// Create Constraints builder
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Constraints cannot be set without a type.</exception>
        public ConstraintBuilder Constraints()
        {
            if(_type==VariableTypeEnum.None)
                throw new ArgumentException("Constraints cannot be set without a type.");
            return new ConstraintBuilder(this);
        }

        /// <summary>
        /// Sets the constraint.
        /// </summary>
        /// <param name="constraintBuilder">The constraint builder.</param>
        /// <returns></returns>
        internal VariableBuilder SetConstraint(ConstraintBuilder constraintBuilder)
        {
            _constraints = constraintBuilder;
            return this;
        }
    }

    /// <summary>
    /// Variables Constraint Builder
    /// </summary>
    public class ConstraintBuilder
    {
        private readonly VariableBuilder _parent;

        /// <summary>
        /// Gets the minimum value.
        /// </summary>
        /// <value>
        /// The minimum.
        /// </value>
        public object Min { get; private set;}
        /// <summary>
        /// Gets the maximum value.
        /// </summary>
        /// <value>
        /// The maximum.
        /// </value>
        public object Max { get; private set; }
        /// <summary>
        /// Gets the default value.
        /// </summary>
        /// <value>
        /// The default.
        /// </value>
        public object Default { get; private set;}
        /// <summary>
        /// Gets the possible values.
        /// </summary>
        /// <value>
        /// The values.
        /// </value>
        public object[] Values { get;private set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="ConstraintBuilder"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public ConstraintBuilder(VariableBuilder parent)
        {
            _parent = parent;
        }

        /// <summary>
        /// Set Maximum value.
        /// </summary>
        /// <param name="mv">The mv.</param>
        /// <returns></returns>
        public ConstraintBuilder MaxValue(object mv)
        {
            Max = mv;
            return this;
        }
        /// <summary>
        /// Set Minimum value.
        /// </summary>
        /// <param name="mv">The mv.</param>
        /// <returns></returns>
        public ConstraintBuilder MinValue(object mv)
        {
            Min=mv;
            return this;
        }
        /// <summary>
        /// Set Default value
        /// </summary>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public ConstraintBuilder DefaultValue(object defaultValue)
        {
            Default = defaultValue;
            return this;
        }
        /// <summary>
        /// Set Possibe values.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public ConstraintBuilder PossibeValues(params object[] values)
        {
            Values = values;
            return this;
        }

        /// <summary>
        /// Done building.
        /// </summary>
        /// <returns></returns>
        public VariableBuilder Done()
        {
            _parent.SetConstraint(this);
            return _parent;
        }
    }
}