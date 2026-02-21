using MakQR.Models.Dtos;
using MakQR.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MakQR.Controllers
{
    [Authorize(Roles = RoleNames.Admin)]
    public class FacilityController(
        IWebHostEnvironment env
        ) : Controller
    {
        private readonly IWebHostEnvironment _env = env;
        public IActionResult Index()
        {
            return View();
        }

        public async ValueTask<IActionResult> AddFacilitySection(FacilityGalleryRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid) { return BadRequest(); }
                var imageFolder = Path.Combine(_env.WebRootPath, "images");

                await AppHelper.ReplaceIfUploaded(request.Image, "gallery_img.png", imageFolder);
                await AppHelper.ReplaceIfUploaded(request.Image1, "gallery_img1.png", imageFolder);
                await AppHelper.ReplaceIfUploaded(request.Image2, "gallery_img2.png", imageFolder);
                return Json(new { success = true, message = "Details updated successfully" });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Failed to update details",
                    error = ex.Message
                });
            }
        }
    }
}
