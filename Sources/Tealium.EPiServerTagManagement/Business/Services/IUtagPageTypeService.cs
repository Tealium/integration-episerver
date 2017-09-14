using System.Collections.Generic;
using Tealium.EPiServerTagManagement.Business.Models;

namespace Tealium.EPiServerTagManagement.Business.Services
{
    public interface IUtagPageTypeService : IUtagService
    {
        IUtagPageType Get(string sitename, string language, string pagetype);

        IEnumerable<IUtagPageType> Get(string sitename, string language);

        bool Update(IUtagPageType item);
        
        void HardReset();
    }
}
