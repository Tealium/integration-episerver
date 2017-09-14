using System.Collections.Generic;
using System.Linq;
using EPiServer.DataAbstraction;
using EPiServer.Web;
using Tealium.EPiServerTagManagement.Business.DataStore;
using Tealium.EPiServerTagManagement.Business.Providers;
using Tealium.EPiServerTagManagement.Business.Services;

namespace Tealium.EPiServerTagManagement.Models
{
    public class MainSettingsViewModel : BaseViewModel
    {
        public bool Enabled { get; set; }

        public string Account { get; set; }

        public string Profile { get; set; }

        public string Environment { get; set; }

        public bool EnableUtagJs { get; set; }

        public static IEnumerable<KeyValuePair<string,string>> GetSites(SiteDefinitionRepository siteDefinitionRepository)
        {
            return siteDefinitionRepository.List().Select(x => new KeyValuePair<string, string>(x.Name, x.Name));
        }

        public static IEnumerable<KeyValuePair<string, string>> GetAvailableLanguages(ILanguageBranchRepository languageBranchRepository)
        {
            return languageBranchRepository.ListEnabled().Select(x => new KeyValuePair<string, string>(x.Name, x.LanguageID));
        }

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
            siteData.Enabled = this.Enabled;
            siteData.Profile = this.Profile;
            siteData.Account = this.Account;
            siteData.Environment = this.Environment;
            siteData.EnableUtagJs = this.EnableUtagJs;

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
