namespace KlaudWerk.ProcessEngine.Builder
{
    /// <summary>
    /// Step Action Builder
    /// </summary>
    public class StepActionBuilder
    {
        private string _name;
        private string _description;
        private bool _skippable;

        /// <summary>
        /// Gets the name of the action.
        /// </summary>
        /// <value>
        /// The name of the action.
        /// </value>
        public string ActionName => _name;
        /// <summary>
        /// Gets the action description.
        /// </summary>
        /// <value>
        /// The action description.
        /// </value>
        public string ActionDescription => _description;
        /// <summary>
        /// Gets a value indicating whether this instance is skippable.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is skippable; otherwise, <c>false</c>.
        /// </value>
        public bool IsSkippable => _skippable;

        private readonly StepBuilder _parent;

        /// <summary>
        /// Initializes a new instance of the <see cref="StepActionBuilder"/> class.
        /// Action will be created as Skippable by default
        /// </summary>
        /// <param name="parent">The parent.</param>
        public StepActionBuilder(StepBuilder parent)
        {
            _parent = parent;
            _skippable = true;
        }

        /// <summary>
        /// Set Name
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public StepActionBuilder Name(string name)
        {
            _name = name;
            return this;
        }
        /// <summary>
        /// Set Description
        /// </summary>
        /// <param name="description">The description.</param>
        /// <returns></returns>
        public StepActionBuilder Description(string description)
        {
            _description = description;
            return this;
        }
        /// <summary>
        /// Set Skippable
        /// </summary>
        /// <param name="skippable">if set to <c>true</c> [skippable].</param>
        /// <returns></returns>
        public StepActionBuilder Skippable(bool skippable)
        {
            _skippable = skippable;
            return this;
        }
        /// <summary>
        /// Done building Action
        /// </summary>
        /// <returns></returns>
        public StepBuilder Done()
        {
            _name.NotNull("name").NotEmptyString("name");
            _parent.AddReplaceAction(this);
            return _parent;
        }

        /// <summary>
        /// Removes the action from its parent.
        /// </summary>
        /// <returns></returns>
        public StepBuilder Remove()
        {
            _parent.RemoveAction(this);
            return _parent;
        }
    }
}
