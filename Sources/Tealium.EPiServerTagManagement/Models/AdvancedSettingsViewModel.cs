using System.Collections.Generic;
using Tealium.EPiServerTagManagement.Business.DataStore;
using Tealium.EPiServerTagManagement.Business.Providers;
using Tealium.EPiServerTagManagement.Business.Services;

namespace Tealium.EPiServerTagManagement.Models
{
    public class AdvancedSettingsViewModel : BaseViewModel
    {
        public bool EnableCustomUdo { get; set; }

        public string CustomUdoAssembly { get; set; }

        public override bool Save(IUtagService utagService, IEnumerable<string> tags)
        {
            IUtagConfigurationService utagConfigurationService = (IUtagConfigurationService)utagService;
            var siteData = utagConfigurationService.Get(this.WebsiteName, this.Language);

            if (siteData == null)
            {
                siteData = new UtagConfigurationStore();
            }

            siteData.WebsiteName = this.WebsiteName;
            siteData.Language = this.Language;
            siteData.EnableCustomUdo = this.EnableCustomUdo;
            siteData.CustomUdoAssembly = this.CustomUdoAssembly;
            
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
