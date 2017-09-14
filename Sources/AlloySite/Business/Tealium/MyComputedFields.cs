using System.Collections.Generic;
using System.Web;
using Tealium.EPiServerTagManagement.Business.Mappings;

namespace AlloySite.Business.Tealium
{
    public class MyComputedFields : IComputedFieldMapper
    {
        public void AddComputedFields(IDictionary<string, object> utagParams)
        {
            this.AddSearchTerm(utagParams);
        }

        private void AddSearchTerm(IDictionary<string, object> utagParams)
        {
            var searchTerm = HttpContext.Current.Request["query"];

            if (searchTerm != null)
            {
                if (!utagParams.ContainsKey("search_term"))
                {
                    utagParams.Add("search_term", searchTerm);
                }
            }
        }
    }
}


/*
 * 
 this.AddPageType(utagParams);
 * 
 * 
 private void AddPageType(IDictionary<string, object> utagParams)
        {
            PageRouteHelper pageRouteHelper = ServiceLocator.Current.GetInstance<PageRouteHelper>();
            IContentLoader contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();

            var currentPageReference = pageRouteHelper.PageLink ?? null;

            if (currentPageReference == null)
            {
                return;
            }

            var page = contentLoader.Get<PageData>(currentPageReference);

            if (page != null)
            {
                if (!utagParams.ContainsKey("utag_ContentType"))
                {
                    utagParams.Add("utag_ContentType", page.PageTypeName);
                }
            }
        }
 */