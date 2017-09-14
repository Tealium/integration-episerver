using System.Collections.Generic;
using Tealium.EPiServerTagManagement.Business.DataStore;
using Tealium.EPiServerTagManagement.Business.Extensions;
using Tealium.EPiServerTagManagement.Business.Providers;
using Tealium.EPiServerTagManagement.Business.Services;

namespace Tealium.EPiServerTagManagement.Models
{
    public class PageTypeViewModel : BaseViewModel
    {
        public PageTypeViewModel()
        {
            this.Tags = new Dictionary<string, string>();
        }

        public string PageTypeName { get; set; }

        public Dictionary<string, string> Tags { get; set; }

        public static List<KeyValuePair<string, string>> GetPageTypes()
        {
            var pageTypes = new List<KeyValuePair<string, string>>();
            pageTypes.AddRange(TealiumFactory.PageTypeSettingsProvider.GetPageTypes());
            pageTypes.AddRange(TealiumFactory.PageTypeSettingsProvider.GetCommerceTypes());
            
            return pageTypes;
        }

        public override bool Save(IUtagService utagService, IEnumerable<string> tags)
        {
            IUtagPageTypeService utagpageTypeService = (IUtagPageTypeService)utagService;
            var siteData = utagpageTypeService.Get(this.WebsiteName, this.Language, this.PageTypeName);

            if (siteData == null)
            {
                siteData = new UtagPageTypeStore();
            }

            this.Tags = tags.TagsCollectionToDictionary(true);
            
            siteData.WebsiteName = this.WebsiteName;
            siteData.Language = this.Language;
            siteData.PageType = this.PageTypeName;
            siteData.ContentTypeTags = this.Tags.TagsDictionaryToString();

            var result = utagpageTypeService.Update(siteData);

            //Update Cached data
            if (TealiumFactory.PageTypeSettingsProvider != null)
            {
                TealiumFactory.PageTypeSettingsProvider.DataChanged(this.WebsiteName, this.Language);
            }

            return result;
        }
    }
}
