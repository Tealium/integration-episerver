using System;
using System.Globalization;
using System.Linq;
using EPiServer.Data.Dynamic;
using EPiServer.ServiceLocation;
using log4net;
using Tealium.EPiServerTagManagement.Business.DataStore;
using Tealium.EPiServerTagManagement.Business.Models;

namespace Tealium.EPiServerTagManagement.Business.Services
{
    [ServiceConfiguration(ServiceType = typeof(IUtagConfigurationService), Lifecycle = ServiceInstanceScope.HttpContext)]
    public class UtagConfigurationService : IUtagConfigurationService
    {
        private readonly ILog log = LogManager.GetLogger(typeof(UtagConfigurationService));

        public virtual IUtagConfiguration Get(string sitename, string language)
        {
            using (var ds = typeof(UtagConfigurationStore).GetStore())
            {
                return (from record in ds.Items<UtagConfigurationStore>()
                        select record)
                        .FirstOrDefault(x => x.WebsiteName.Equals(sitename) && x.Language.Equals(language));
            }
        }

        public virtual bool Update(IUtagConfiguration item)
        {
            try
            {
                using (var ds = typeof(UtagConfigurationStore).GetStore())
                {
                    ds.Save(item);
                    return true;
                }
            }
            catch (Exception ex)
            {
                this.log.ErrorFormat(CultureInfo.InvariantCulture, "[UTAG ConfigurationService] {0}", ex);
            }

            return false;
        }

        /// <summary>
        /// Deletes all items from UtagConfigurationStore DDS.
        /// </summary>
        public virtual void HardReset()
        {
            try
            {
                using (var ds = typeof(UtagConfigurationStore).GetStore())
                {
                    ds.DeleteAll();
                }
            }
            catch (Exception ex)
            {
                this.log.ErrorFormat(CultureInfo.InvariantCulture, "[UTAG ConfigurationService] {0}", ex);
            }
        }
    }
}
