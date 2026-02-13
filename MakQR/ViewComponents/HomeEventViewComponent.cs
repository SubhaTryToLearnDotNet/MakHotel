using MakQR.Services.Interfaces.Home;
using Microsoft.AspNetCore.Mvc;

namespace MakQR.ViewComponents
{
    public class HomeEventViewComponent(IJsonSectionService iJsonSectionService) : ViewComponent
    {
        private readonly IJsonSectionService _iJsonSectionService = iJsonSectionService;
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var EventVewModel = await _iJsonSectionService.GetHomeEventSection();
            return View(EventVewModel);
        }
    }
}
