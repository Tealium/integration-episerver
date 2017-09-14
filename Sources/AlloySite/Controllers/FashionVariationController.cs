using System.Web.Mvc;
using AlloySite.Models.Catalog;
using EPiServer.Web.Mvc;

namespace AlloySite.Controllers
{
    public class FashionVariationController : ContentController<FashionVariation>
    {
        public ActionResult Index(FashionVariation currentContent)
        {
            return View(currentContent);
        }
    }
}