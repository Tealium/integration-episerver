using System.Web;
using EPiServer.Core;

namespace Tealium.EPiServerTagManagement.Business.Providers
{
    public interface ITealiumManager
    {
        /// <summary>
        /// Returns Tealium scripts that will go to the <head></head> section.
        /// </summary>
        /// <param name="currentPage">The current page.</param>
        /// <returns>IHtmlString object.</returns>
        IHtmlString HeadInjections(IContent currentPage);

        /// <summary>
        /// Returns Tealium scripts that will go to the <body></body> section.
        /// </summary>
        /// <param name="currentPage">The current page.</param>
        /// <returns>IHtmlString object.</returns>
        IHtmlString BodyInjections(IContent currentPage);
    }
}
