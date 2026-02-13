using MakQR.Services.Interfaces.Home;
using Microsoft.AspNetCore.Mvc;

namespace MakQR.ViewComponents
{
    public class HomeAboutViewComponent(IJsonSectionService iJsonSectionService) : ViewComponent
    {
        private readonly IJsonSectionService _iJsonSectionService = iJsonSectionService;
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var AboutVewModel = await _iJsonSectionService.GetHomeAboutSection();
            return View(AboutVewModel);
        }
    }

}
