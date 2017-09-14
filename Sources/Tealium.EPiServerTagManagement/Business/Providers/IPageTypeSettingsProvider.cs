using System.Collections.Generic;
using EPiServer.Core;
using Tealium.EPiServerTagManagement.Business.Models;

namespace Tealium.EPiServerTagManagement.Business.Providers
{
    public interface IPageTypeSettingsProvider
    {
        /// <summary>
        /// Gets the tealium settings.
        /// </summary>
        /// <value>
        /// The tealium settings.
        /// </value>
        List<IUtagPageType> TealiumPageTypeSettings { get; }

        void DataChanged(string websitename, string language);

        IEnumerable<KeyValuePair<string, string>> GetPageTypes();

        IEnumerable<KeyValuePair<string, string>> GetCommerceTypes();

        string GetMetaClassName(IContent contentItem);
    }
}
