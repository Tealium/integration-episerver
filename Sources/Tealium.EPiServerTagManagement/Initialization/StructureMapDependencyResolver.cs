using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using StructureMap;

namespace Tealium.EPiServerTagManagement.Initialization
{
    public class StructureMapDependencyResolver : IDependencyResolver
    {
        private readonly IContainer container;

        public StructureMapDependencyResolver(IContainer container)
        {
            this.container = container;
        }

        public object GetService(Type serviceType)
        {
            if (serviceType.IsInterface || serviceType.IsAbstract)
            {
                return this.GetInterfaceService(serviceType);
            }

            return this.GetConcreteService(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return this.container.GetAllInstances(serviceType).Cast<object>();
        }

        private object GetConcreteService(Type serviceType)
        {
            try
            {
                return this.container.GetInstance(serviceType);
            }
            catch (StructureMapException)
            {
                return null;
            }
        }

        private object GetInterfaceService(Type serviceType)
        {
            return this.container.TryGetInstance(serviceType);
        }
    }
}
