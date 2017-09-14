using System;
using System.Collections.Generic;
using EPiServer.Data;
using EPiServer.Data.Dynamic;
using Tealium.EPiServerTagManagement.Business.Extensions;
using Tealium.EPiServerTagManagement.Business.Models;

namespace Tealium.EPiServerTagManagement.Business.DataStore
{
    [EPiServerDataStore(AutomaticallyCreateStore = true, AutomaticallyRemapStore = true)]
    public class UtagPageTypeStore : IDynamicData, IUtagPageType
    {
        public Identity Id { get; set; }

        public string WebsiteName { get; set; }

        public string Language { get; set; }

        public string PageType { get; set; }

        public string ContentTypeTags { get; set; }

        public Dictionary<string, string> GetContentTypeTags()
        {
            return this.ContentTypeTags.TagsStringToDictionary();
        }

        protected void Initialize()
        {
            this.Id = Identity.NewIdentity(Guid.NewGuid());
        }
    }
}
