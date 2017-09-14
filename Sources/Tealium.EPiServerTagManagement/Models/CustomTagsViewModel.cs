using System.Collections.Generic;
using Tealium.EPiServerTagManagement.Business.DataStore;
using Tealium.EPiServerTagManagement.Business.Extensions;
using Tealium.EPiServerTagManagement.Business.Providers;
using Tealium.EPiServerTagManagement.Business.Services;

namespace Tealium.EPiServerTagManagement.Models
{
    public class CustomTagsViewModel : BaseViewModel
    {
        public CustomTagsViewModel()
        {
            this.Tags = new Dictionary<string, string>();
        }

        public Dictionary<string, string> Tags { get; set; }

        public override bool Save(IUtagService utagService, IEnumerable<string> tags)
        {
            IUtagConfigurationService utagConfigurationService = (IUtagConfigurationService) utagService;
            var siteData = utagConfigurationService.Get(this.WebsiteName, this.Language);

            if (siteData == null)
            {
                siteData = new UtagConfigurationStore();
            }

            this.Tags = tags.TagsCollectionToDictionary();
            siteData.WebsiteName = this.WebsiteName;
            siteData.Language = this.Language;
            siteData.CustomTags = this.Tags.TagsDictionaryToString();

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
