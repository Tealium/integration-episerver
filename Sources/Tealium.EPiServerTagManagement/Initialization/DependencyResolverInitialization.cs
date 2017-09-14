using System.Web.Mvc;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using log4net;
using StructureMap;
using Tealium.EPiServerTagManagement.Business.Services;

namespace Tealium.EPiServerTagManagement.Initialization
{
    [InitializableModule]
    [ModuleDependency(typeof(global::EPiServer.Web.InitializationModule))]
    public class DependencyResolverInitialization : IConfigurableModule
    {
        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            context.Container.Configure(this.ConfigureContainer);
            DependencyResolver.SetResolver(new StructureMapDependencyResolver(context.Container));
        }

        private void ConfigureContainer(ConfigurationExpression container)
        {
            container.For<IUtagConfigurationService>().Use(() => new UtagConfigurationService());
            container.For<IUtagPageTypeService>().Use(() => new UtagPageTypeService());
        }

        public void Initialize(InitializationEngine context)
        {
        }

        public void Uninitialize(InitializationEngine context)
        {
        }

        public void Preload(string[] parameters)
        {
        }
    }
}
