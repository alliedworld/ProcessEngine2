using KlaudWerk.ProcessEngine.Definition;

namespace KlaudWerk.ProcessEngine
{
    /// <summary>
    /// Tag Service Provider
    /// </summary>
    public interface ITagServiceProvider
    {
        ITageService GetTagService(TagDefinition tagDefinition);
    }
}