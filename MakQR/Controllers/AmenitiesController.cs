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
    public class AmenitiesController(IWebHostEnvironment env, IMemoryCache cache) : Controller
    {
        private readonly IWebHostEnvironment _env = env;
        private readonly IMemoryCache _cache = cache;
        public async ValueTask<IActionResult> Index()
        {
            if (_cache.TryGetValue(CacheKeys.AmenitiesSection, out AmenitiesModel? cached))
            {

                return View(cached);
            }
            var path = JsonFilePath.AmenitiesFilePath(_env);
            var json = await System.IO.File.ReadAllTextAsync(path);
            var data = JsonSerializer.Deserialize<AmenitiesModel>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new AmenitiesModel();
            _cache.Set(CacheKeys.AmenitiesSection, data);
            return View(data);
        }



        [HttpPost]
        public IActionResult SaveAmenitiesJson([FromBody] AmenitiesModel model)
        {
            if (model == null) return BadRequest(new { success = false, message = "Invalid data" });

            var jsonPath = JsonFilePath.AmenitiesFilePath(_env);
            model.Amenities = model.Amenities
                .Where(a => !string.IsNullOrWhiteSpace(a.Icon) && !string.IsNullOrWhiteSpace(a.Title))
                .ToList();

            var json = JsonSerializer.Serialize(model, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(jsonPath, json);
            _cache.Set(CacheKeys.AmenitiesSection, json);
            return Ok(new { success = true, message = "Amenities saved successfully!" });
        }
    }
}
