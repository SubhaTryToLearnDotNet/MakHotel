using MakQR.Models.Home;
using MakQR.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace MakQR.Controllers
{
    [Authorize(Roles = RoleNames.Admin)]
    public class FooterController(IWebHostEnvironment env, IMemoryCache cache) : Controller
    {
        private readonly IWebHostEnvironment _env = env;
        private readonly IMemoryCache _cache = cache;
        public async ValueTask<IActionResult> Index()
        {

            if (_cache.TryGetValue(CacheKeys.FooterSection, out FooterSection? cached))
                return View(cached);
            var path = JsonFilePath.FooterFilePath(_env);
            if (!System.IO.File.Exists(path))
                return View(new FooterSection());

            var json = await System.IO.File.ReadAllTextAsync(path);

            var data = JsonSerializer.Deserialize<FooterSection>(json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            _cache.Set(CacheKeys.FooterSection, data);

            return View(data);
        }

        [HttpPost]

        public async Task<IActionResult> SaveFooter(FooterSection model)
        {
            if (!ModelState.IsValid) { return BadRequest(); }
            var path = JsonFilePath.FooterFilePath(_env);

            var json = JsonSerializer.Serialize(model, new JsonSerializerOptions { WriteIndented = true });
            await System.IO.File.WriteAllTextAsync(path, json);
            _cache.Set(CacheKeys.FooterSection, json);

            return Json(new
            {
                success = true,
                message = "Footer Updated Successfully"
            });
        }
    }
}
