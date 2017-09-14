using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EPiServer.Data.Dynamic;
using EPiServer.ServiceLocation;
using log4net;
using Tealium.EPiServerTagManagement.Business.DataStore;
using Tealium.EPiServerTagManagement.Business.Extensions;
using Tealium.EPiServerTagManagement.Business.Models;

namespace Tealium.EPiServerTagManagement.Business.Services
{
    [ServiceConfiguration(ServiceType = typeof(IUtagPageTypeService), Lifecycle = ServiceInstanceScope.HttpContext)]
    public class UtagPageTypeService : IUtagPageTypeService
    {
        private readonly ILog log = LogManager.GetLogger(typeof(UtagPageTypeService));

        public virtual IUtagPageType Get(string sitename, string language, string pagetype)
        {
            using (var ds = typeof(UtagPageTypeStore).GetStore())
            {
                return (from record in ds.Items<UtagPageTypeStore>()
                        select record)
                        .FirstOrDefault(x => x.WebsiteName.Equals(sitename) 
                            && x.Language.Equals(language)
                            && x.PageType.Equals(pagetype));
            }
        }

        public virtual IEnumerable<IUtagPageType> Get(string sitename, string language)
        {
            using (var ds = typeof(UtagPageTypeStore).GetStore())
            {
                List<UtagPageTypeStore> result =
                    (from record in ds.Items<UtagPageTypeStore>()
                        select record)
                    .Where(x => x.WebsiteName.Equals(sitename)
                                && x.Language.Equals(language))
                    .ToList();

                return result;
            }
        }

        public virtual bool Update(IUtagPageType item)
        {
            if (item == null || item.WebsiteName.IsNullOrEmpty() || item.Language.IsNullOrEmpty())
            {
                return false;
            }

            try
            {
                using (var ds = typeof(UtagPageTypeStore).GetStore())
                {
                    ds.Save(item);
                    return true;
                }
            }
            catch (Exception ex)
            {
                this.log.ErrorFormat(CultureInfo.InvariantCulture, "[UTAG PageTypeService] {0}", ex);
            }

            return false;
        }

        /// <summary>
        /// Deletes all items from UtagPageTypeStore DDS.
        /// </summary>
        public virtual void HardReset()
        {
            try
            {
                using (var ds = typeof(UtagPageTypeStore).GetStore())
                {
                    ds.DeleteAll();
                }
            }
            catch (Exception ex)
            {
                this.log.ErrorFormat(CultureInfo.InvariantCulture, "[UTAG PageTypeService] {0}", ex);
            }
        }
    }
}
