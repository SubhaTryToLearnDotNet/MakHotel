using MakQR.Services.Interfaces.Home;
using Microsoft.AspNetCore.Mvc;

namespace MakQR.ViewComponents
{
    public class RoomsSectionViewComponent(IJsonSectionService iJsonSectionService) : ViewComponent
    {
        private readonly IJsonSectionService _iJsonSectionService = iJsonSectionService;
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var RoomsTypesVewModel = await _iJsonSectionService.GetRoomsSection();

            return View(RoomsTypesVewModel);
        }
    }
}
