using MakQR.Models.Dtos;
using MakQR.Models.Home;
using MakQR.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace MakQR.Controllers
{
    [Authorize(Roles = RoleNames.Admin)]
    public class FacilityController(
        IWebHostEnvironment env, IMemoryCache cache
        ) : Controller
    {
        private readonly IWebHostEnvironment _env = env;
        private readonly IMemoryCache _cache = cache;
        public async ValueTask<IActionResult> Index()
        {
            FacilityGalleryRequestDto facilityGallery = new();
            if (_cache.TryGetValue(CacheKeys.Facility, out FacilityGallerySection? cached))
            {
                facilityGallery.Caption1 = cached?.Caption1 ?? "";
                facilityGallery.Caption2 = cached?.Caption2 ?? "";
                facilityGallery.Caption3 = cached?.Caption3 ?? "";
                facilityGallery.Title = cached?.Title ?? "";
                return View(facilityGallery);
            }
            var json = await System.IO.File.ReadAllTextAsync(JsonFilePath.FacilityGalleryFilePath(_env));
            var data = JsonSerializer.Deserialize<FacilityGallerySection>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            _cache.Set(CacheKeys.Facility, data);
            facilityGallery.Caption1 = data?.Caption1 ?? "";
            facilityGallery.Caption2 = data?.Caption2 ?? "";
            facilityGallery.Caption3 = data?.Caption3 ?? "";
            facilityGallery.Title = data?.Title ?? "";
            return View(facilityGallery);
        }

        public async ValueTask<IActionResult> AddFacilitySection(FacilityGalleryRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid) { return BadRequest(); }
                var imageFolder = Path.Combine(_env.WebRootPath, "images");
                var jsonPath = JsonFilePath.FacilityGalleryFilePath(_env);

                await AppHelper.ReplaceIfUploaded(request.Image, "gallery_img.png", imageFolder);
                await AppHelper.ReplaceIfUploaded(request.Image1, "gallery_img1.png", imageFolder);
                await AppHelper.ReplaceIfUploaded(request.Image2, "gallery_img2.png", imageFolder);
                FacilityGallerySection facilityGallerySection = new FacilityGallerySection
                {
                    Title = request.Title,
                    Caption1 = request.Caption1,
                    Caption2 = request.Caption2,
                    Caption3 = request.Caption3
                };

                await System.IO.File.WriteAllTextAsync(jsonPath, JsonSerializer.Serialize(facilityGallerySection, new JsonSerializerOptions { WriteIndented = true }));
                _cache.Set(CacheKeys.Facility, facilityGallerySection);
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
