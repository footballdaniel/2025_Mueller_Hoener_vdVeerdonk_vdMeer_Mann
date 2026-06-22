namespace Interactions.Config.Contracts
{
    public interface IConfig : IReadOnlyConfig
    {
        void Set(string key, string value);
    }
}
