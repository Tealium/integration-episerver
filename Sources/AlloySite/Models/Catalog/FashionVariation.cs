using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Catalog.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAnnotations;

namespace AlloySite.Models.Catalog
{
    [CatalogContentType(MetaClassName = "Fashion_Variation",
        DisplayName = "Fashion_Variation",
        GUID = "C0BB1FD3-2737-4385-BD1F-C9461C70D166")]
    public class FashionVariation : VariationContent
    {
        [Searchable]
        [CultureSpecific]
        public virtual XhtmlString MainContent { get; set; }

        public virtual string Color { get; set; }

        public virtual string Size { get; set; }

        public virtual bool CanBeMonogrammed { get; set; }
    }
}