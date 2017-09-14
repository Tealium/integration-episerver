using System;
using System.Collections;
using System.Linq;
using System.Web.Caching;
using System.Web.Hosting;

namespace Tealium.EPiServerTagManagement.Business.Vpp
{
    public class EmbeddedVirtualPathProvider : VirtualPathProvider
    {
        public EmbeddedVirtualPathProvider()
        {
        }

        public override CacheDependency GetCacheDependency(
            string virtualPath,
            IEnumerable virtualPathDependencies,
            DateTime utcStart)
        {
            string embedded = _GetEmbeddedPath(virtualPath);

            // not embedded? fall back
            if (string.IsNullOrEmpty(embedded))
            {
                return base.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
            }

            // there is no cache dependency for embedded resources
            return null;
        }

        public override bool FileExists(string virtualPath)
        {
            string embedded = _GetEmbeddedPath(virtualPath);

            // You can override the embed by placing a real file
            // at the virtual path...
            return base.FileExists(virtualPath)
                || !string.IsNullOrEmpty(embedded);
        }

        public override VirtualFile GetFile(string virtualPath)
        {
            // You can override the embed by placing a real file
            // at the virtual path...
            if (base.FileExists(virtualPath))
            {
                return base.GetFile(virtualPath);
            }

            string embedded = _GetEmbeddedPath(virtualPath);

            // sanity...
            if (string.IsNullOrEmpty(embedded))
            {
                return null;
            }

            return new EmbeddedVirtualFile(virtualPath,
                GetType().Assembly.GetManifestResourceStream(embedded));
        }

        private string _GetEmbeddedPath(string path)
        {
            // ~/views/sample/x.cshtml
            // => /views/sample/x.cshtml
            // => FunWithMvc.views.sample.x.cshtml

            if (path.StartsWith("~/"))
            {
                path = path.Substring(1);
            }

            path = "Tealium.EPiServerTagManagement" + path.Replace('/', '.');
            path = path.ToLowerInvariant();

            // this makes sure the "virtual path" exists as an
            // embedded resource
            return GetType().Assembly.GetManifestResourceNames().FirstOrDefault(o => o.ToLowerInvariant() == path);
        }
    }
}
