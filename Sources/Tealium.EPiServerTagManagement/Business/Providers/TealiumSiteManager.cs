using System.Collections.Generic;
using System.Linq;
using EPiServer;
using EPiServer.Globalization;
using EPiServer.ServiceLocation;
using EPiServer.Web;

namespace Tealium.EPiServerTagManagement.Business.Providers
{
    public class TealiumSiteManager : ISiteManager
    {
        private static ICollection<string> configuredSites = new List<string>();

        private readonly SiteDefinitionRepository siteDefinitionRepository;

        public TealiumSiteManager()
        {
            this.siteDefinitionRepository = ServiceLocator.Current.GetInstance<SiteDefinitionRepository>();
        }

        public IEnumerable<string> ConfiguredSites
        {
            get
            {
                if (!configuredSites.Any())
                {
                    configuredSites = this.siteDefinitionRepository.List().Select(x => x.Name).ToList();
                }

                return configuredSites;
            }
        }

        public string ContextSiteName
        {
            get
            {
                var siteDefinition = this.siteDefinitionRepository.List().FirstOrDefault(x => x.SiteUrl.Equals(UriSupport.SiteUrl));
                return siteDefinition != null ? siteDefinition.Name : string.Empty;
            }
        }

        public string ContextLanguageName
        {
            get { return ContentLanguage.PreferredCulture.Name; }
        }
    }
}
