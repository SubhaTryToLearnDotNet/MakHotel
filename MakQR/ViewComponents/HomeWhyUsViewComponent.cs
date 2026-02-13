using MakQR.Services.Interfaces.Home;
using Microsoft.AspNetCore.Mvc;

namespace MakQR.ViewComponents
{
    public class HomeWhyUsViewComponent(IJsonSectionService iJsonSectionService) : ViewComponent
    {
        private readonly IJsonSectionService _iJsonSectionService = iJsonSectionService;
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var WhyUsVewModel = await _iJsonSectionService.GetHomeWhyUsSection();
            return View(WhyUsVewModel);
        }
    }
}
