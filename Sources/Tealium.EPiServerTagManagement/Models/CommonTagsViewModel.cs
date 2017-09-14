using System.Collections.Generic;
using System.Linq;
using Tealium.EPiServerTagManagement.Business.DataStore;
using Tealium.EPiServerTagManagement.Business.Extensions;
using Tealium.EPiServerTagManagement.Business.Models;
using Tealium.EPiServerTagManagement.Business.Providers;
using Tealium.EPiServerTagManagement.Business.Services;

namespace Tealium.EPiServerTagManagement.Models
{
    public class CommonTagsViewModel : BaseViewModel
    {
        public CommonTagsViewModel()
        {
            this.AllTags = new Dictionary<string, string>();
            this.SiteTags = new Dictionary<string, string>();
        }

        public Dictionary<string, string> AllTags { get; set; }

        public Dictionary<string, string> SiteTags { get; set; }

        public override bool Save(IUtagService utagService, IEnumerable<string> checkedTags)
        {
            IUtagConfigurationService utagConfigurationService = (IUtagConfigurationService)utagService;
            var siteData = utagConfigurationService.Get(this.WebsiteName, this.Language);

            if (siteData == null)
            {
                siteData = new UtagConfigurationStore();
            }

            this.AllTags = CommonPropertyTags.Instance.List;
            this.SiteTags = this.AllTags.Where(x => checkedTags.Contains(x.Key)).ToDictionary(t => t.Key, t => t.Value);
            
            siteData.WebsiteName = this.WebsiteName;
            siteData.Language = this.Language;
            siteData.CommonTags = this.SiteTags.TagsDictionaryToString();

            var result = utagConfigurationService.Update(siteData);

            //Update Cached data
            if (TealiumFactory.SettingsProvider != null)
            {
                TealiumFactory.SettingsProvider.DataChanged(this.WebsiteName, this.Language);
            }

            return result;
        }
    }
}
