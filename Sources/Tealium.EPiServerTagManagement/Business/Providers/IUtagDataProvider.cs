using System.Collections.Generic;
using EPiServer.Core;

namespace Tealium.EPiServerTagManagement.Business.Providers
{
    public interface IUtagDataProvider
    {
        /// <summary>
        /// Gets the utag data.
        /// </summary>
        /// <param name="currentPage">The current page.</param>
        /// <returns>The collection of utags.</returns>
        IDictionary<string, string> GetUtagData(IContent currentPage);
    }
}
