using System;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using Tealium.EPiServerTagManagement.Business.Vpp;

namespace AlloySite
{
    public class EPiServerApplication : EPiServer.Global
    {
        protected void Application_Start()
        {
            HostingEnvironment.RegisterVirtualPathProvider(new EmbeddedVirtualPathProvider());

            AreaRegistration.RegisterAllAreas();

            //Tip: Want to call the EPiServer API on startup? Add an initialization module instead (Add -> New Item.. -> EPiServer -> Initialization Module)
        }

        protected override void RegisterRoutes(RouteCollection routes)
        {
            base.RegisterRoutes(routes);
            //routes.MapRoute("utagplugin", "UtagPlugin/{action}", new { controller = "UtagPlugin", action = "Index" });
        } 
    }
}