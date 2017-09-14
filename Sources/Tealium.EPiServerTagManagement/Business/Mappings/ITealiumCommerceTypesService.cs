using System.Collections.Generic;
using EPiServer.Core;

namespace Tealium.EPiServerTagManagement.Business.Mappings
{
    public interface ITealiumCommerceTypesService
    {
        Dictionary<int, string> GetTypes();

        string GetMetaClassName(IContent content);
    }
}
