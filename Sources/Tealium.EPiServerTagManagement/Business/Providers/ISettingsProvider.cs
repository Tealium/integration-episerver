using Tealium.EPiServerTagManagement.Business.Models;

namespace Tealium.EPiServerTagManagement.Business.Providers
{
    public interface ISettingsProvider
    {
        /// <summary>
        /// Gets the tealium utag js URI format.
        /// </summary>
        /// <value>
        /// The tealium utag js URI format.
        /// </value>
        string TealiumUtagJsUriFormat { get; }

        /// <summary>
        /// Gets the tealium utag synchronize js URI format.
        /// </summary>
        /// <value>
        /// The tealium utag synchronize js URI format.
        /// </value>
        string TealiumUtagSyncJsUriFormat { get; }

        /// <summary>
        /// Gets the tealium settings.
        /// </summary>
        /// <value>
        /// The tealium settings.
        /// </value>
        IUtagConfiguration TealiumSettings { get; }

        void DataChanged(string websitename, string language);
    }
}
