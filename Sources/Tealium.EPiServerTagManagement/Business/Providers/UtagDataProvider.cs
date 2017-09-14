using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Web.Routing;
using log4net;
using Tealium.EPiServerTagManagement.Business.Extensions;

namespace Tealium.EPiServerTagManagement.Business.Providers
{
    public class UtagDataProvider : IUtagDataProvider
    {
        private readonly ILog log = LogManager.GetLogger(typeof(UtagDataProvider));

        /// <summary>
        /// Initializes a new instance of the <see cref="UtagDataProvider" /> class.
        /// </summary>
        /// <param name="settingsProvider">The settings provider.</param>
        /// <param name="pageTypeSettingsProvider">The page type settings provider.</param>
        public UtagDataProvider(ISettingsProvider settingsProvider, IPageTypeSettingsProvider pageTypeSettingsProvider)
        {
            this.SettingsProvider = settingsProvider;
            this.PageTypeSettingsProvider = pageTypeSettingsProvider;
        }

        protected ISettingsProvider SettingsProvider { get; private set; }

        protected IPageTypeSettingsProvider PageTypeSettingsProvider { get; private set; }

        /// <summary>
        /// Gets the utag data.
        /// </summary>
        /// <param name="currentPage">The current page.</param>
        /// <returns>
        /// The collection of utags.
        /// </returns>
        public virtual IDictionary<string, string> GetUtagData(IContent currentPage)
        {
            var utagData = new Dictionary<string, string>();

            this.AddCommonParameters(currentPage, utagData);
            this.AddCustomParameters(currentPage, utagData);
            this.AddPageTypeParameters(currentPage, utagData);
            this.AddComputedFields(currentPage, utagData);
            
            return utagData;
        }

        /// <summary>
        /// Adds the common parameters.
        /// </summary>
        /// <param name="currentPage">The current page.</param>
        /// <param name="utagData">The utag data.</param>
        protected virtual void AddCommonParameters(IContent currentPage, IDictionary<string, string> utagData)
        {
            var commonTags = this.SettingsProvider.TealiumSettings.GetCommonTags();
            if (commonTags == null || !commonTags.Any())
            {
                return;
            }

            foreach (var tag in commonTags)
            {
                var value = this.GetPagePropertyValue(currentPage, tag.Value);
                this.AddUtag(utagData, tag.Key, value);
            }
        }

        /// <summary>
        /// Adds the custom parameters.
        /// </summary>
        /// <param name="currentPage">The current page.</param>
        /// <param name="utagData">The utag data.</param>
        protected virtual void AddCustomParameters(IContent currentPage, IDictionary<string, string> utagData)
        {
            var customTags = this.SettingsProvider.TealiumSettings.GetCustomTags();
            if (customTags == null || !customTags.Any())
            {
                return;
            }

            foreach (var customParam in customTags)
            {
                if (!utagData.ContainsKey(customParam.Key))
                {
                    if (customParam.Value.IsNullOrEmpty())
                    {
                        this.AddUtag(utagData, customParam.Key, string.Empty);
                        continue;
                    }

                    var splittedValue = customParam.Value.Split(new[] { "|", "," }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    if (splittedValue.Length > 1)
                    {
                        this.AddUtag(utagData, customParam.Key, splittedValue);
                    }
                    else
                    {
                        this.AddUtag(utagData, customParam.Key, customParam.Value);
                    }
                }
            }
        }

        /// <summary>
        /// Adds the page type parameters.
        /// </summary>
        /// <param name="currentPage">The current page.</param>
        /// <param name="utagData">The utag data.</param>
        protected virtual void AddPageTypeParameters(IContent currentPage, IDictionary<string, string> utagData)
        {
            var pageTypeName = this.ObtainContentItemTypeName(currentPage);

            var settings = this.PageTypeSettingsProvider.TealiumPageTypeSettings;
            var pageTypeTags = settings
                .Where(p => p.PageType != null)
                .FirstOrDefault(p => p.PageType.Equals(pageTypeName));

            if (pageTypeTags == null)
            {
                return;
            }

            var tags = pageTypeTags.GetContentTypeTags();

            if (tags == null || !tags.Any())
            {
                return;
            }

            foreach (var tag in tags)
            {
                var value = this.GetPagePropertyValue(currentPage, tag.Value);
                this.AddUtag(utagData, tag.Key, value);
            }
        }

        /// <summary>
        /// Adds the computed fields.
        /// </summary>
        /// <param name="currentPage">The current page.</param>
        /// <param name="utagData">The utag data.</param>
        protected void AddComputedFields(IContent currentPage, Dictionary<string, string> utagData)
        {
            var computedFieldMapper = TealiumFactory.ComputedFieldMapper;

            if (this.SettingsProvider.TealiumSettings.EnableCustomUdo && computedFieldMapper != null)
            {
                var computedFields = new Dictionary<string, object>();

                try
                {
                    computedFieldMapper.AddComputedFields(computedFields);
                }
                catch (Exception ex)
                {
                    this.log.ErrorFormat("[UtagDataProvider]: Computed Fields Mapper [{0}] threw unhandled exception: {1}", computedFieldMapper.GetType().Name, ex.Message);
                }

                foreach (var utag in computedFields)
                {
                    this.AddUtag(utagData, utag.Key, utag.Value);
                }
            }
        }
        
        private void AddUtag(IDictionary<string, string> utagData, string paramName, object paramValue)
        {
            if (paramValue == null || (paramValue is Enumerable && !((IEnumerable) paramValue).Cast<object>().Any())
                && (paramValue is string && string.IsNullOrWhiteSpace(paramValue.ToString())))
            {
                return;
            }

            if (utagData.Keys.Contains(paramName))
            {
                utagData[paramName] = paramValue.ToJsonFormat();
            }
            else
            {
                utagData.Add(paramName, paramValue.ToJsonFormat());
            }
        }

        private object GetPagePropertyValue(IContent currentPage, string propertyName)
        {
            object paramValue = null;
            string[] propertyNames = this.GetPropertyNames(propertyName);

            try
            {
                if (propertyNames[0].Equals("LinkURL"))
                {
                    paramValue = this.GetExternalUrl(currentPage);
                }
                else if (propertyNames[0].Contains("Language"))
                {
                    // I'm sorry for this workaround but unfortunately 
                    // EPiServer pages and commerce items have different names of language fields.
                    var properties = currentPage.GetType().GetProperties();
                    var any = properties.Any(p => p.Name.Equals(propertyName));
                    var langPropName = any ? propertyName : "Language";
                    paramValue = currentPage.GetType().GetProperty(langPropName).GetValue(currentPage, null);
                }
                else if (propertyNames[0].Equals("PageTypeName"))
                {
                    paramValue = this.ObtainContentItemTypeName(currentPage);
                }
                else if (propertyNames[0].Equals("Category") && currentPage is PageData)
                {
                    // I'm sorry for this workaround but unfortunately 
                    // EPiServer pages needs to be casted to PageData to get Category property.
                    var categoryProperty = ((PageData) currentPage).Category;
                    var values = new List<string>();
                    foreach (var categoryItem in categoryProperty)
                    {
                        var category = Category.Find(categoryItem);
                        if (category != null)
                        {
                            var subProperty = propertyNames.Length > 1
                                                  ? category.GetType()
                                                            .GetProperty(propertyNames[1])
                                                            .GetValue(category, null)
                                                  : category.Description;
                            values.Add(subProperty.ToString());
                        }
                    }

                    paramValue = values;
                }
                else
                {
                    if (propertyNames.Length == 2)
                    {
                        var firstObj = currentPage.GetType().GetProperty(propertyNames[0]).GetValue(currentPage, null);
                        if (firstObj != null)
                        {
                            paramValue = firstObj.GetType().GetProperty(propertyNames[1]).GetValue(firstObj, null);
                        }
                    }
                    else
                    {
                        paramValue = currentPage.GetType().GetProperty(propertyNames[0]).GetValue(currentPage, null);
                    }
                }
            }
            catch (AmbiguousTypeException ex)
            {
                this.log.ErrorFormat("[UTAG] Getting PageData properties. {0}", ex);
            }
            catch (ArgumentNullException ex)
            {
                this.log.ErrorFormat("[UTAG] Getting PageData properties. {0}", ex);
            }
            catch (ArgumentException ex)
            {
                this.log.ErrorFormat("[UTAG] Getting PageData properties. {0}", ex);
            }
            catch (TargetException ex)
            {
                this.log.ErrorFormat("[UTAG] Getting PageData properties. {0}", ex);
            }
            catch (TargetParameterCountException ex)
            {
                this.log.ErrorFormat("[UTAG] Getting PageData properties. {0}", ex);
            }
            catch (MethodAccessException ex)
            {
                this.log.ErrorFormat("[UTAG] Getting PageData properties. {0}", ex);
            }
            catch (TargetInvocationException ex)
            {
                this.log.ErrorFormat("[UTAG] Getting PageData properties. {0}", ex);
            }
            catch (Exception ex)
            {
                this.log.ErrorFormat("[UTAG] Getting PageData properties. {0}", ex);
            }

            return paramValue;
        }

        private string GetExternalUrl(IContent content)
        {
            var internalUrl = UrlResolver.Current.GetUrl(content.ContentLink);

            var url = new UrlBuilder(internalUrl);
            Global.UrlRewriteProvider.ConvertToExternal(url, null, System.Text.Encoding.UTF8);

            var friendlyUrl = UriSupport.AbsoluteUrlBySettings(url.ToString());
            return friendlyUrl;
        }

        private string[] GetPropertyNames(string propertyName)
        {
            string[] propertyNames = new[] { propertyName };

            try
            {
                Regex regex = new Regex(@"\w{1,256}(\[\w{1,256}\])?");
                var obj = regex.Matches(propertyName);
                if (obj.Count > 0)
                {
                    var indexOpenBracket = propertyName.IndexOf("[");
                    if (indexOpenBracket > 0)
                    {
                        var first = propertyName.Substring(0, indexOpenBracket);
                        var second = propertyName.Substring(indexOpenBracket + 1).TrimEnd(']');
                        propertyNames = new[] {first, second};
                    }
                }
            }
            catch (Exception ex)
            {
                this.log.ErrorFormat(CultureInfo.InvariantCulture, "[UTAG] Parsing propert name. {0}", ex);
            }

            return propertyNames;
        }

        private string ObtainContentItemTypeName(IContent currentPage)
        {
            var metaClassName = this.PageTypeSettingsProvider.GetMetaClassName(currentPage);
            var pageTypeName = currentPage is PageData
                                   ? (currentPage as PageData).PageTypeName
                                   : metaClassName.IsNotNullOrEmpty() ? metaClassName : string.Empty;
            return pageTypeName;
        }
    }
}
