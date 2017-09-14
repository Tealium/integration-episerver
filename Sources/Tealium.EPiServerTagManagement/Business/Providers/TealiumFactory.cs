using System;
using Tealium.EPiServerTagManagement.Business.Mappings;

namespace Tealium.EPiServerTagManagement.Business.Providers
{
    public static class TealiumFactory
    {
        private static readonly object SiteSync = new object();
        private static readonly object UtagSync = new object();
        private static readonly object ManagerSync = new object();
        private static readonly object SettingsSync = new object();
        private static readonly object PageTypeSettingsSync = new object();
        private static readonly object ComputedSync = new object();

        private static ISettingsProvider settingsProvider;
        private static IPageTypeSettingsProvider pageTypeSettingsProvider;
        private static ISiteManager siteManager;
        private static IUtagDataProvider utagDataProvider;
        private static ITealiumManager manager;
        private static IComputedFieldMapper computedFieldMapper;

        public static ISiteManager SiteManager
        {
            get
            {
                if (siteManager == null)
                {
                    lock (SiteSync)
                    {
                        if (siteManager == null)
                        {
                            siteManager = new TealiumSiteManager();
                        }
                    }
                }

                return siteManager;
            }
        }

        /// <summary>
        /// Gets the tealium manager.
        /// </summary>
        /// <value>
        /// The tealium manager.
        /// </value>
        /// <returns>ITealiumManager object</returns>
        public static ITealiumManager TealiumManager
        {
            get
            {
                if (manager == null)
                {
                    lock (ManagerSync)
                    {
                        if (manager == null)
                        {
                            manager = new TealiumManager(UtagDataProvider, SettingsProvider);
                        }
                    }
                }

                return manager;
            }
        }

        /// <summary>
        /// Gets the utag data provider.
        /// </summary>
        /// <value>
        /// The utag data provider.
        /// </value>
        /// <returns>IUtagDataProvider object</returns>
        public static IUtagDataProvider UtagDataProvider
        {
            get
            {
                if (utagDataProvider == null)
                {
                    lock (UtagSync)
                    {
                        if (utagDataProvider == null)
                        {
                            var type = Type.GetType(typeof(UtagDataProvider).FullName);
                            utagDataProvider = (IUtagDataProvider)Activator.CreateInstance(type, SettingsProvider, PageTypeSettingsProvider);
                        }
                    }
                }

                return utagDataProvider;
            }
        }

        /// <summary>
        /// Gets the settings provider.
        /// </summary>
        /// <value>
        /// The settings provider.
        /// </value>
        /// <returns>ISettingsProvider object</returns>
        public static ISettingsProvider SettingsProvider
        {
            get
            {
                if (settingsProvider == null)
                {
                    lock (SettingsSync)
                    {
                        if (settingsProvider == null)
                        {
                            settingsProvider = new SettingsProvider();
                        }
                    }
                }

                return settingsProvider;
            }
        }

        /// <summary>
        /// Gets the pagetype settings provider.
        /// </summary>
        /// <value>
        /// The page type settings provider.
        /// </value>
        /// <returns>IPageTypeSettingsProvider object</returns>
        public static IPageTypeSettingsProvider PageTypeSettingsProvider
        {
            get
            {
                if (pageTypeSettingsProvider == null)
                {
                    lock (PageTypeSettingsSync)
                    {
                        if (pageTypeSettingsProvider == null)
                        {
                            pageTypeSettingsProvider = new PageTypeSettingsProvider();
                        }
                    }
                }

                return pageTypeSettingsProvider;
            }
        }

        /// <summary>
        /// Gets the settings provider.
        /// </summary>
        /// <value>
        /// The computed field mapper.
        /// </value>
        /// <returns>IComputedFieldMapper object</returns>
        public static IComputedFieldMapper ComputedFieldMapper
        {
            get
            {
                if (computedFieldMapper == null)
                {
                    lock (ComputedSync)
                    {
                        if (computedFieldMapper == null)
                        {
                            var typeQualifiedString = SettingsProvider.TealiumSettings.CustomUdoAssembly;
                            if (string.IsNullOrEmpty(typeQualifiedString))
                            {
                                return null;
                            }

                            var type = Type.GetType(typeQualifiedString);
                            computedFieldMapper = (IComputedFieldMapper)Activator.CreateInstance(type);
                        }
                    }
                }

                return computedFieldMapper;
            }
        }
    }
}
