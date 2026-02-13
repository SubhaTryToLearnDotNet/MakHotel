using MakQR.Services;
using MakQR.Services.Interfaces.Home;
using Microsoft.AspNetCore.Mvc;

namespace MakQR.ViewComponents
{
    public class RoomsViewComponent(IJsonSectionService iJsonSectionService) : ViewComponent
    {
        private readonly IJsonSectionService _iJsonSectionService = iJsonSectionService;
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var RoomsVewModel = await _iJsonSectionService.GetRoomsView();
            return View(RoomsVewModel);
        }
    }
}
