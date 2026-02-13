using MakQR.Services.Interfaces.Home;
using Microsoft.AspNetCore.Mvc;

namespace MakQR.ViewComponents
{
    public class HomeReviewViewComponent(IReviewsService iReviewsService) : ViewComponent
    {
        private readonly IReviewsService _iReviewsService = iReviewsService;
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var ReviewVewModel = await _iReviewsService.GetHomeReviewSection();

            return View(ReviewVewModel);
        }
    }
}
