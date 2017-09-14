using Tealium.EPiServerTagManagement.Business.Models;

namespace Tealium.EPiServerTagManagement.Business.Services
{
    public interface IUtagConfigurationService : IUtagService
    {
        IUtagConfiguration Get(string sitename, string language);

        bool Update(IUtagConfiguration item);

        void HardReset();
    }
}
