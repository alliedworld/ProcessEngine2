using KlaudWerk.ProcessEngine.Definition;

namespace KlaudWerk.ProcessEngine.Runtime
{
    /// <summary>
    /// Runtime Tag Definition
    /// </summary>
    public class TagRuntime
    {
        private readonly TagDefinition _td;

        public TagDefinition Tag => _td;

        public TagRuntime(TagDefinition td)
        {
            _td = td;
        }
    }
}