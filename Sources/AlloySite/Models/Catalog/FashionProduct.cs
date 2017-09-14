using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Catalog.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAnnotations;

namespace AlloySite.Models.Catalog
{
    [CatalogContentType(MetaClassName = "Fashion_Product",
        DisplayName = "Fashion_Product",
        GUID = "7876A3C4-0EA8-4D7C-9CE2-A6E78E55A24E")]
    public class FashionProduct : ProductContent 
    {
        [Searchable]
        [CultureSpecific]
        public virtual XhtmlString MainContent { get; set; }

        public virtual string ClothesType { get; set; }

        public virtual string Brand { get; set; }
    }
}