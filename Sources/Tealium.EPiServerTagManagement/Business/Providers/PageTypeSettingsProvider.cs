using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;
using Tealium.EPiServerTagManagement.Business.Mappings;
using Tealium.EPiServerTagManagement.Business.Models;
using Tealium.EPiServerTagManagement.Business.Services;

namespace Tealium.EPiServerTagManagement.Business.Providers
{
    public class PageTypeSettingsProvider : IPageTypeSettingsProvider
    {
        private readonly IUtagPageTypeService pageTypeService;
        private readonly IContentTypeRepository contentTypeRepository;

        private ITealiumCommerceTypesService tealiumCommerceTypesService;

        public PageTypeSettingsProvider()
        {
            this.pageTypeService = new UtagPageTypeService();
            this.contentTypeRepository = ServiceLocator.Current.GetInstance<IContentTypeRepository>();

            this.InitializeCommerceService();
        }

        /// <summary>
        /// Gets the tealium settings.
        /// </summary>
        /// <value>
        /// The tealium settings.
        /// </value>
        /// <returns>Tealium settings.</returns>
        public List<IUtagPageType> TealiumPageTypeSettings
        {
            get
            {
                var contextLanguage = TealiumFactory.SiteManager.ContextLanguageName;
                var contextSiteName = TealiumFactory.SiteManager.ContextSiteName;
                
                var siteSpecificSettings = this.pageTypeService.Get(contextSiteName, contextLanguage);

                return siteSpecificSettings.ToList();
            }
        }

        public void DataChanged(string websitename, string language)
        {
        }

        public IEnumerable<KeyValuePair<string, string>> GetPageTypes()
        {
            return this.contentTypeRepository.List()
                       .Where(t => (t is PageType) && ((t as PageType).HasTemplate))
                       .Select(x => new KeyValuePair<string, string>(x.Name, x.Name))
                       .ToList();
        }

        public IEnumerable<KeyValuePair<string, string>> GetCommerceTypes()
        {
            var pageTypes = new List<KeyValuePair<string, string>>();

            if (this.tealiumCommerceTypesService == null)
            {
                return pageTypes;
            }

            var commerceTypes = this.tealiumCommerceTypesService.GetTypes();
            if (commerceTypes != null)
            {
                pageTypes.AddRange(commerceTypes.Select(x => new KeyValuePair<string, string>(x.Value, x.Value)));
            }

            return pageTypes;
        }

        public string GetMetaClassName(IContent contentItem)
        {
            if (this.tealiumCommerceTypesService == null)
            {
                return string.Empty;
            }

            return this.tealiumCommerceTypesService.GetMetaClassName(contentItem);
        }

        private void InitializeCommerceService()
        {
            Assembly[] asms = AppDomain.CurrentDomain.GetAssemblies();
            var commerceDllExists = asms.Any(x => x.FullName.Contains("Tealium.EPiServerTagManagement.Commerce"));
            if (!commerceDllExists)
            {
                this.tealiumCommerceTypesService = null;
                return;
            }

            var type = Type.GetType("Tealium.EPiServerTagManagement.Commerce.CommerceTypesService, Tealium.EPiServerTagManagement.Commerce");
            if (type == null)
            {
                this.tealiumCommerceTypesService = null;
                return;
            }

            this.tealiumCommerceTypesService = (ITealiumCommerceTypesService)Activator.CreateInstance(type);
        }
    }
}
