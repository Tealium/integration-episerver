using System.Collections.Generic;

namespace Tealium.EPiServerTagManagement.Business.Providers
{
    public interface ISiteManager
    {
        IEnumerable<string> ConfiguredSites { get; }

        string ContextSiteName { get; }

        string ContextLanguageName { get; }
    }
}
