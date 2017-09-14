using System;
using System.Collections.Generic;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using Tealium.EPiServerTagManagement.Business.Mappings;
using log4net;
using Mediachase.MetaDataPlus;
using Mediachase.MetaDataPlus.Configurator;

namespace Tealium.EPiServerTagManagement.Commerce
{
    public class CommerceTypesService : ITealiumCommerceTypesService
    {
        public Dictionary<int, string> GetTypes()
        {
            var metaClassCollection = MetaClass.GetList(MetaDataContext.Instance, true);
            var result = new Dictionary<int, string>();

            foreach (MetaClass item in metaClassCollection)
            {
                if (item.TableName.StartsWith("CatalogNodeEx_") || item.TableName.StartsWith("CatalogEntryEx_"))
                {
                    result.Add(item.Id, item.Name);
                }
            }

            return result;
        }

        public string GetMetaClassName(IContent content)
        {
            int id = content is EntryContentBase
                            ? (content as EntryContentBase).MetaClassId
                            : content is NodeContent ? (content as NodeContent).MetaClassId : -1;
            if (id > 0)
            {
                try
                {
                    return this.GetTypes()[id];
                }
                catch (NullReferenceException ex)
                {
                    ILog log = LogManager.GetLogger(typeof(CommerceTypesService));
                    log.ErrorFormat("[UTAG] Get Commerce type name for content item {0}. {1}", content.ContentLink, ex);

                    return string.Empty;
                }
            }

            return string.Empty;
        }
    }
}
