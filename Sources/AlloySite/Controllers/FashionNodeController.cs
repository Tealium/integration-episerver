using System.Web.Mvc;
using AlloySite.Models.Catalog;
using EPiServer.Web.Mvc;

namespace AlloySite.Controllers
{
    public class FashionNodeController : ContentController<FashionNode>
    {
        public ActionResult Index(FashionNode currentContent)
        {
            return View(currentContent);
        }
    }
}