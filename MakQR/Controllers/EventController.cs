using MakQR.Models.Dto;
using MakQR.Models.Home;
using MakQR.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace MakQR.Controllers
{
    [Authorize(Roles = RoleNames.Admin)]
    public class EventController(IWebHostEnvironment env, IMemoryCache cache) : Controller
    {
        private readonly IWebHostEnvironment _env = env;
        private readonly IMemoryCache _cache = cache;
        public async ValueTask<IActionResult> Index()
        {
            HomeEventSectionRequestDto Response = new HomeEventSectionRequestDto();
            if (_cache.TryGetValue(CacheKeys.HomeEventSection, out EventSection? cached))
            {
                Response.Title = cached?.Title ?? "";
                Response.DescriptionHtml = cached?.DescriptionHtml ?? "";
                return View(Response);
            }
            var path = JsonFilePath.HomeEventSectionPath(_env);
            var json = await System.IO.File.ReadAllTextAsync(path);
            var data = JsonSerializer.Deserialize<EventSection>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new EventSection();
            _cache.Set(CacheKeys.HomeEventSection, data);
            Response.Title = data.Title;
            Response.DescriptionHtml = data.DescriptionHtml;
            return View(Response);
        }

        public async ValueTask<IActionResult> AddEventSection(HomeEventSectionRequestDto request)
        {
            if (!ModelState.IsValid) { return BadRequest(); }
            var jsonPath = JsonFilePath.HomeEventSectionPath(_env);
            var imageFolder = Path.Combine(_env.WebRootPath, "images");
            EventSection eventSection = new()
            {
                Title = request.Title,
                DescriptionHtml = request.DescriptionHtml,
                UpdatedOn = DateTime.Now
            };
            await AppHelper.ReplaceIfUploaded(request.EventImg, "home_event.png", imageFolder);
            await System.IO.File.WriteAllTextAsync(jsonPath, JsonSerializer.Serialize(eventSection, new JsonSerializerOptions { WriteIndented = true }));
            _cache.Set(CacheKeys.HomeEventSection, eventSection);
            return Json(new { success = true, message = "Details updated successfully" });
        }
    }
}
