using MakQR.Models.Home;
using MakQR.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace MakQR.Controllers
{
    [Authorize(Roles = RoleNames.Admin)]
    public class HeaderController(IWebHostEnvironment env, IMemoryCache cache) : Controller
    {
        private readonly IWebHostEnvironment _env = env;
        private readonly IMemoryCache _cache = cache;


        public async Task<IActionResult> Index()
        {
            if (_cache.TryGetValue(CacheKeys.HeaderSecttion, out HeaderSection? cached))
            {
                return View(cached);
            }
            var path = JsonFilePath.HeaderFilePath(_env);
            var json = await System.IO.File.ReadAllTextAsync(path);
            var data = JsonSerializer.Deserialize<HeaderSection>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new HeaderSection();
            _cache.Set(CacheKeys.HeaderSecttion, data);
            return View(data);
        }



        [HttpPost]
        public async Task<IActionResult> Save(HeaderSection model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid data" });
            }
            var filePath = JsonFilePath.HeaderFilePath(_env);
            var json = JsonSerializer.Serialize(model, new JsonSerializerOptions { WriteIndented = true });
            await System.IO.File.WriteAllTextAsync(filePath, json);
            _cache.Set(CacheKeys.HeaderSecttion, json);
            return Json(new { success = true, message = "Details saved successfully" });
        }
    }
}
