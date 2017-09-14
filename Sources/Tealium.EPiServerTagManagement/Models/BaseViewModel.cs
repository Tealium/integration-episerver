using System.Collections.Generic;
using Tealium.EPiServerTagManagement.Business.Services;

namespace Tealium.EPiServerTagManagement.Models
{
    public abstract class BaseViewModel
    {
        public string WebsiteName { get; set; }

        public string Language { get; set; }

        public abstract bool Save(IUtagService utagService, IEnumerable<string> tags);
    }
}
