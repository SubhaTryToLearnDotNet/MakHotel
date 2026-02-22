using MakQR.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace MakQR.Controllers
{
    [Authorize(Roles = RoleNames.Admin)]
    public class GoogleSettingsController(IWebHostEnvironment env,
        IMemoryCache cache) : Controller
    {
        private readonly IWebHostEnvironment _env = env;
        private readonly IMemoryCache _cache = cache;
        public async ValueTask<IActionResult> Index()
        {
            if (_cache.TryGetValue(CacheKeys.GoogleSettings, out GoogleSettings? cached))
            {
                return View(cached);
            }
            var json = await System.IO.File.ReadAllTextAsync(JsonFilePath.GoogleSettingFilePath(_env));
            var section = JsonSerializer.Deserialize<GoogleSettings>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new GoogleSettings();
            return View(section);
        }

        public async ValueTask<IActionResult> SaveSettings(GoogleSettings model)
        {
            await System.IO.File.WriteAllTextAsync(JsonFilePath.GoogleSettingFilePath(_env), JsonSerializer.Serialize(model, new JsonSerializerOptions { WriteIndented = true }));
            _cache.Set(CacheKeys.Reviews, model);
            return Json(new { success = true, message = "Details updated successfully" });
        }
    }
}
