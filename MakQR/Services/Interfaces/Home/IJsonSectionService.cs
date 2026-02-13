using MakQR.Models.Home;

namespace MakQR.Services.Interfaces.Home
{
    public interface IJsonSectionService
    {
        #region Home Get - Conntent Section
        ValueTask<EventSection> GetHomeEventSection();
        ValueTask<AboutSection> GetHomeAboutSection();
        ValueTask<WhyUsSection> GetHomeWhyUsSection();
        ValueTask<RoomsViewSection> GetRoomsView();
        ValueTask<RoomsSection> GetRoomsSection();
        #endregion
    }
}
