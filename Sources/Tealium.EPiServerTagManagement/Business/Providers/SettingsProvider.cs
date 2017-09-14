using System.Configuration;
using Tealium.EPiServerTagManagement.Business.Models;
using Tealium.EPiServerTagManagement.Business.Services;

namespace Tealium.EPiServerTagManagement.Business.Providers
{
    public class SettingsProvider : ISettingsProvider
    {
        private readonly IUtagConfigurationService configurationService;

        public SettingsProvider()
        {
            this.configurationService = new UtagConfigurationService();
        }

        /// <summary>
        /// Gets the tealium utag js URI format.
        /// </summary>
        /// <value>
        /// The tealium utag js URI format.
        /// </value>
        public string TealiumUtagJsUriFormat
        {
            get
            {
                return ConfigurationManager.AppSettings.Get("Tealium.Utag.Js.UriFormat") ?? string.Empty;
            }
        }

        /// <summary>
        /// Gets the tealium utag synchronize js URI format.
        /// </summary>
        /// <value>
        /// The tealium utag synchronize js URI format.
        /// </value>
        public string TealiumUtagSyncJsUriFormat
        {
            get
            {
                return ConfigurationManager.AppSettings.Get("Tealium.Utag.Sync.Js.UriFormat") ?? string.Empty;
            }
        }

        /// <summary>
        /// Gets the tealium settings.
        /// </summary>
        /// <value>
        /// The tealium settings.
        /// </value>
        /// <returns>Tealium settings.</returns>
        public IUtagConfiguration TealiumSettings
        {
            get
            {
                var contextLanguage = TealiumFactory.SiteManager.ContextLanguageName;
                var contextSiteName = TealiumFactory.SiteManager.ContextSiteName;
                
                return this.configurationService.Get(contextSiteName, contextLanguage);
            }
        }

        public void DataChanged(string websitename, string language)
        {
        }
    }
}
