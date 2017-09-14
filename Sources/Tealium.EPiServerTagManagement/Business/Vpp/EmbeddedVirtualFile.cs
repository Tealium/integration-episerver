using System;
using System.IO;
using System.Web.Hosting;

namespace Tealium.EPiServerTagManagement.Business.Vpp
{
    public class EmbeddedVirtualFile : VirtualFile
    {
        private Stream stream;

        public EmbeddedVirtualFile(string virtualPath,
            Stream stream)
            : base(virtualPath)
        {
            if (null == stream)
            {
                throw new ArgumentNullException("stream");
            }

            this.stream = stream;
        }

        public override Stream Open()
        {
            return stream;
        }
    }
}
