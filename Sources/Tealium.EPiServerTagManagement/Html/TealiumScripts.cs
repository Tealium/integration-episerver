using System.Web;
using EPiServer.Core;
using Tealium.EPiServerTagManagement.Business.Providers;

namespace Tealium.EPiServerTagManagement.Html
{
    public static class TealiumScripts
    {
        /// <summary>
        /// Returns Tealium scripts that will go to the <head></head> section.
        /// </summary>
        /// <param name="currentPage">The current page.</param>
        /// <returns>
        /// IHtmlString object.
        /// </returns>
        public static IHtmlString Head(IContent currentPage)
        {
            if (currentPage == null)
            {
                return new HtmlString(string.Empty);
            }

            return TealiumFactory.TealiumManager.HeadInjections(currentPage);
        }

        /// <summary>
        /// Returns Tealium scripts that will go to the <body></body> section.
        /// </summary>
        /// <param name="currentPage">The current page.</param>
        /// <returns>
        /// IHtmlString object.
        /// </returns>
        public static IHtmlString Body(IContent currentPage)
        {
            if (currentPage == null)
            {
                return new HtmlString(string.Empty);
            }

            return TealiumFactory.TealiumManager.BodyInjections(currentPage);
        }
    }
}
