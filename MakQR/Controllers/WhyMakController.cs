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
    public class WhyMakController(IWebHostEnvironment env, IMemoryCache cache
       ) : Controller

    {
        private readonly IWebHostEnvironment _env = env;
        private readonly IMemoryCache _cache = cache;
        public async ValueTask<IActionResult> Index()
        {
            HomeWhyUsSectionRequestDto Response = new HomeWhyUsSectionRequestDto();
            if (_cache.TryGetValue(CacheKeys.HomeWhySection, out AboutSection? cached))
            {
                Response.Title = cached?.Title ?? "";
                Response.DescriptionHtml = cached?.DescriptionHtml ?? "";
                return View(Response);
            }
            var json = await System.IO.File.ReadAllTextAsync(JsonFilePath.HomeWhyUsSectionPath(_env));
            var data = JsonSerializer.Deserialize<WhyUsSection>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new WhyUsSection();
            _cache.Set(CacheKeys.HomeWhySection, data);
            Response.Title = data.Title;
            Response.DescriptionHtml = data.DescriptionHtml;
            return View(Response);
        }
        public async ValueTask<IActionResult> AddWhyUsSection(HomeWhyUsSectionRequestDto request)
        {
            if (!ModelState.IsValid) { return BadRequest(); }
            var jsonPath = JsonFilePath.HomeWhyUsSectionPath(_env);
            var imageFolder = Path.Combine(_env.WebRootPath, "images");
            WhyUsSection Section = new()
            {
                Title = request.Title,
                DescriptionHtml = request.DescriptionHtml,
                UpdatedOn = DateTime.Now
            };
            await AppHelper.ReplaceIfUploaded(request.Image, "why_us.png", imageFolder);
            await System.IO.File.WriteAllTextAsync(jsonPath, JsonSerializer.Serialize(Section, new JsonSerializerOptions { WriteIndented = true }));
            _cache.Set(CacheKeys.HomeWhySection, Section);
            return Json(new { success = true, message = "Details updated successfully" });
        }
    }
}
