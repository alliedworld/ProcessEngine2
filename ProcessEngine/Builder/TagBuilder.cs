namespace KlaudWerk.ProcessEngine.Builder
{
    /// <summary>
    /// Tag Builder
    /// </summary>
    public class TagBuilder
    {
        private readonly ProcessBuilder _parent;
        private TagHandlerBuilder _handlerBuilder;
        public TagHandlerBuilder TagHandler => _handlerBuilder;

        public TagBuilder(string id, ProcessBuilder parent)
        {
            _parent = parent;
            Id = id;
        }

        /// <summary>
        /// Tag Id
        /// </summary>
        public string Id { get; }
        /// <summary>
        /// Display NAme
        /// </summary>
        public string DisplayName { get; private set; }
        /// <summary>
        /// Done building
        /// </summary>
        /// <returns></returns>
        public ProcessBuilder Done()
        {
            return _parent;
        }
        /// <summary>
        /// Remove tag
        /// </summary>
        /// <returns></returns>
        public TagBuilder Remove()
        {

            _parent.RemoveTag(Id);
            return this;
        }

        /// <summary>
        /// Set Name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public TagBuilder Name(string name)
        {
            DisplayName = name;
            return this;
        }
        /// <summary>
        /// Create or return a handler
        /// </summary>
        /// <returns></returns>
        public TagHandlerBuilder Handler()
        {
            if(_handlerBuilder==null)
                _handlerBuilder=new TagHandlerBuilder(this);
            return _handlerBuilder;
        }
    }

    /// <summary>
    /// Tag Handler Builder
    /// </summary>
    public class TagHandlerBuilder : HandlerBuilder<TagBuilder>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parent"></param>
        public TagHandlerBuilder(TagBuilder parent) : base(parent)
        {
        }
    }
}