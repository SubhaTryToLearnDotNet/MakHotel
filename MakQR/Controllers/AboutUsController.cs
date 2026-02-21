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
    public class AboutUsController(IWebHostEnvironment env, IMemoryCache cache) : Controller
    {

        private readonly IWebHostEnvironment _env = env;
        private readonly IMemoryCache _cache = cache;
        public async ValueTask<IActionResult> Index()
        {
            HomeAboutUsSectionRequestDto Response = new HomeAboutUsSectionRequestDto();
            if (_cache.TryGetValue(CacheKeys.HomeAboutUsSection, out AboutSection? cached))
            {
                Response.Title = cached?.Title ?? "";
                Response.DescriptionHtml = cached?.DescriptionHtml ?? "";
                return View(Response);
            }
            var json = await System.IO.File.ReadAllTextAsync(JsonFilePath.HomeEventSectionPath(_env));
            var data = JsonSerializer.Deserialize<AboutSection>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new AboutSection();
            _cache.Set(CacheKeys.HomeAboutUsSection, data);
            Response.Title = data.Title;
            Response.DescriptionHtml = data.DescriptionHtml;
            return View(Response);
        }

        [Authorize(Roles = RoleNames.Admin)]
        public async ValueTask<IActionResult> AddAboutUsSection(HomeAboutUsSectionRequestDto request)
        {
            if (!ModelState.IsValid) { return BadRequest(); }
            var jsonPath = JsonFilePath.HomeAboutUsSectionPath(_env);
            var imageFolder = Path.Combine(_env.WebRootPath, "images");
            AboutSection eventSection = new()
            {
                Title = request.Title,
                DescriptionHtml = request.DescriptionHtml,
                UpdatedOn = DateTime.Now
            };
            await AppHelper.ReplaceIfUploaded(request.AboutUsImage, "about_us.png", imageFolder);
            await System.IO.File.WriteAllTextAsync(jsonPath, JsonSerializer.Serialize(eventSection, new JsonSerializerOptions { WriteIndented = true }));
            _cache.Set(CacheKeys.HomeAboutUsSection, eventSection);
            return Json(new { success = true, message = "Details updated successfully" });
        }
    }
}
