using Microsoft.AspNetCore.Mvc;

namespace MakQR.ViewComponents
{
    public class RoomsSectionViewComponent : ViewComponent 
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {

            return View();
        }
    }
}
