using System.Web.Mvc;
using AlloySite.Models.Catalog;
using EPiServer.Web.Mvc;

namespace AlloySite.Controllers
{
    public class FashionProductController : ContentController<FashionProduct>
    {
        public ActionResult Index(FashionProduct currentContent)
        {
            return View(currentContent);
        }
    }
}