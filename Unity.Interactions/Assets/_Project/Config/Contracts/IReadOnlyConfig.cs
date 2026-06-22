using System.Collections.Generic;

namespace Interactions.Config.Contracts
{
    public interface IReadOnlyConfig : IEnumerable<(string, string)>
    {
        string Get(string key, string fallback = null);
    }
}
