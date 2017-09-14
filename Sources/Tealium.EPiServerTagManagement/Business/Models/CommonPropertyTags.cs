using System.Collections.Generic;
using System.Configuration;
using Tealium.EPiServerTagManagement.Business.Extensions;

namespace Tealium.EPiServerTagManagement.Business.Models
{
    public class CommonPropertyTags
    {
        private static readonly object Padlock = new object();

        private static CommonPropertyTags instance;

        private CommonPropertyTags()
        {
            this.List = new Dictionary<string, string>();
            
            var stringValue = ConfigurationManager.AppSettings["Tealium.Common.Tags"] as string;

            if (string.IsNullOrWhiteSpace(stringValue))
            {
                return;
            }

            this.List = stringValue.TagsStringToDictionary();
        }

        public static CommonPropertyTags Instance
        {
            get 
            {
                if (instance == null)
                {
                    lock (Padlock)
                    {
                        if (instance == null)
                        {
                            instance = new CommonPropertyTags();
                        }
                    }
                }

                return instance;
            }
        }

        public Dictionary<string, string> List { get; set; }
    }
}
