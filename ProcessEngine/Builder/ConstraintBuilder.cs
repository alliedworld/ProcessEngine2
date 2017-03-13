namespace KlaudWerk.ProcessEngine.Builder
{
    /// <summary>
    /// Variables Constraint Builder
    /// </summary>
    public class ConstraintBuilder
    {
        private readonly VariableBuilder _parent;

        /// <summary>
        /// Indicates that the builder has default value
        /// </summary>
        public bool HasDefault { get; private set; }
        /// <summary>
        /// Indicates that the builder has minimum value
        /// </summary>
        public bool HasMin { get; private set; }
        /// <summary>
        /// Indicates that the builder has Max value
        /// </summary>
        public bool HasMax { get; private set; }
        /// <summary>
        /// Indicates that the builder has the list of possible values
        /// </summary>
        public bool HasPossibleValuesList { get; private set; }
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
            HasMax = true;
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
            HasMin = true;
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
            HasDefault = true;
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
            HasPossibleValuesList = true;
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