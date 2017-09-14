using System;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using EPiServer.Core;
using EPiServer.Editor;
using log4net;

namespace Tealium.EPiServerTagManagement.Business.Providers
{
    public class TealiumManager : ITealiumManager
    {
        private const string BodyScriptFormat = "<script type=\"text/javascript\"> (function(a,b,c,d){{ a='{0}'; b=document; c='script'; d=b.createElement(c); d.src=a; d.type='text/java'+c; d.async=true; a=b.getElementsByTagName(c)[0]; a.parentNode.insertBefore(d,a); }})(); </script>";
        private const string HeadScriptFormat = "<script type='text/javascript' src='{0}'></script>";

        private readonly ILog log = LogManager.GetLogger(typeof(TealiumManager));

        /// <summary>
        /// Initializes a new instance of the <see cref="TealiumManager"/> class.
        /// </summary>
        /// <param name="dataProvider">The data provider.</param>
        /// <param name="settingsProvider">The settings provider.</param>
        public TealiumManager(IUtagDataProvider dataProvider, ISettingsProvider settingsProvider)
        {
            this.DataProvider = dataProvider;
            this.SettingsProvider = settingsProvider;
        }

        public IUtagDataProvider DataProvider { get; protected set; }

        public ISettingsProvider SettingsProvider { get; protected set; }

        /// <summary>
        /// Returns Tealium scripts that will go to the <head></head> section.
        /// </summary>
        /// <param name="currentPage">The current page.</param>
        /// <returns>
        /// IHtmlString object.
        /// </returns>
        public virtual IHtmlString HeadInjections(IContent currentPage)
        {
            if (!this.IsEnabled() || !this.SettingsProvider.TealiumSettings.EnableUtagJs)
            {
                return new HtmlString(string.Empty);
            }

            return new HtmlString(this.GenerateHeadScriptFormat());
        }

        /// <summary>
        /// Returns Tealium scripts that will go to the <body></body> section.
        /// </summary>
        /// <param name="currentPage">The current page.</param>
        /// <returns>
        /// IHtmlString object.
        /// </returns>
        public virtual IHtmlString BodyInjections(IContent currentPage)
        {
            if (!this.IsEnabled())
            {
                return new HtmlString(string.Empty);
            }

            var sb = new StringBuilder("<script type=\"text/javascript\"> ");

            var dataLayerName = ConfigurationManager.AppSettings.Get("Tealium.Utag.DataLayer.Name") ?? "utag_data";
            sb.AppendLine("\n\tvar " + dataLayerName + " = { ");

            try
            {
                var utagParams = this.DataProvider.GetUtagData(currentPage);
                var lastItem = utagParams.Last();

                foreach (var utagData in utagParams)
                {
                    var strToAdd = string.Format("\t\t{0}: {1}, ", utagData.Key, utagData.Value);
                    
                    if (utagData.Equals(lastItem))
                    {
                        strToAdd = strToAdd.TrimEnd().Trim(',');
                    }

                    sb.AppendLine(strToAdd);
                }
            }
            catch (Exception ex)
            {
                this.log.ErrorFormat("[TealliumManager]: {0}", ex);
            }

            sb.AppendLine("\t}; ");
            sb.AppendLine("</script> ");

            sb.AppendLine(this.GenerateBodyScript());

            return new HtmlString(sb.ToString());
        }

        protected bool IsEnabled()
        {
            return this.SettingsProvider.TealiumSettings != null
                && this.SettingsProvider.TealiumSettings.Enabled && !PageEditing.PageIsInEditMode;
        }

        protected string GenerateBodyScript()
        {
            var settings = this.SettingsProvider.TealiumSettings;

            return string.Format(BodyScriptFormat, string.Format(
                TealiumFactory.SettingsProvider.TealiumUtagJsUriFormat,
                settings.Account,
                settings.Profile,
                settings.Environment));
        }

        protected string GenerateHeadScriptFormat()
        {
            var settings = this.SettingsProvider.TealiumSettings;

            return string.Format(HeadScriptFormat, string.Format(
                TealiumFactory.SettingsProvider.TealiumUtagSyncJsUriFormat,
                settings.Account,
                settings.Profile,
                settings.Environment));
        }
    }
}
