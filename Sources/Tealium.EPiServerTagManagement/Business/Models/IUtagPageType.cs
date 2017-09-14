using System.Collections.Generic;

namespace Tealium.EPiServerTagManagement.Business.Models
{
    public interface IUtagPageType
    {
        string WebsiteName { get; set; }

        string Language { get; set; }

        string PageType { get; set; }

        string ContentTypeTags { get; set; }

        Dictionary<string, string> GetContentTypeTags();
    }
}
