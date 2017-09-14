using System;
using System.Collections.Generic;
using EPiServer.Data;
using EPiServer.Data.Dynamic;
using Tealium.EPiServerTagManagement.Business.Extensions;
using Tealium.EPiServerTagManagement.Business.Models;

namespace Tealium.EPiServerTagManagement.Business.DataStore
{
    [EPiServerDataStore(AutomaticallyCreateStore = true, AutomaticallyRemapStore = true)]
    public class UtagConfigurationStore : IDynamicData, IUtagConfiguration
    {
        public UtagConfigurationStore()
        {
            this.Initialize();
        }

        public Identity Id { get; set; }

        public string WebsiteName { get; set; }

        public string Language { get; set; }

        public bool Enabled { get; set; }

        public string Account { get; set; }

        public string Profile { get; set; }

        public string Environment { get; set; }

        public bool EnableUtagJs { get; set; }

        public bool EnableCustomUdo { get; set; }

        public string CustomUdoAssembly { get; set; }

        public string CommonTags { get; set; }

        public string CustomTags { get; set; }

        public Dictionary<string, string> GetCommonTags()
        {
            return this.CommonTags.TagsStringToDictionary();
        }

        public Dictionary<string, string> GetCustomTags()
        {
            return this.CustomTags.TagsStringToDictionary();
        }

        protected void Initialize()
        {
            this.Id = Identity.NewIdentity(Guid.NewGuid());
        }
    }
}
