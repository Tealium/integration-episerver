using System.Collections.Generic;

namespace Tealium.EPiServerTagManagement.Business.Models
{
    public interface IUtagConfiguration
    {
        string WebsiteName { get; set; }

        string Language { get; set; }

        bool Enabled { get; set; }

        string Account { get; set; }

        string Profile { get; set; }

        string Environment { get; set; }

        bool EnableUtagJs { get; set; }

        bool EnableCustomUdo { get; set; }

        string CustomUdoAssembly { get; set; }

        string CommonTags { get; set; }

        string CustomTags { get; set; }

        Dictionary<string, string> GetCommonTags();

        Dictionary<string, string> GetCustomTags();
    }
}
