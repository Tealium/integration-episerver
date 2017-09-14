using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Catalog.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAnnotations;

namespace AlloySite.Models.Catalog
{
    [CatalogContentType(MetaClassName = "Fashion_Node",
        GUID = "2F2A87B4-62DA-42C0-BEE0-2F56C7E55FAB",
        DisplayName = "Fashion_Node")]
    public class FashionNode : NodeContent 
    {
        [Searchable]
        [CultureSpecific]
        public virtual XhtmlString MainContent { get; set; }
    }
}