using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using Tealium.EPiServerTagManagement.App_Start;
using Tealium.EPiServerTagManagement.Business.Vpp;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(Vpp), "RegisterPreApplicationStart", Order = 100)]
[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(Vpp), "RegisterPostApplicationStart", Order = 200)]

namespace Tealium.EPiServerTagManagement.App_Start
{
    public class Vpp
    {
        public static void RegisterPreApplicationStart()
        {
            HostingEnvironment.RegisterVirtualPathProvider(new EmbeddedVirtualPathProvider());
        }

        public static void RegisterPostApplicationStart()
        {
            RegisterRoutes(RouteTable.Routes);
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("utagplugin", "UtagPlugin/{action}", new { controller = "UtagPlugin", action = "Index" });
        }
    }
}
