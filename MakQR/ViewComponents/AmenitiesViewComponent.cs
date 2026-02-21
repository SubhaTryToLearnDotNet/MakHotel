using MakQR.Models.Home;
using MakQR.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace MakQR.ViewComponents
{
    public class AmenitiesViewComponent(IWebHostEnvironment env,
        IMemoryCache cache) : ViewComponent
    {
        private readonly IWebHostEnvironment _env = env;
        private readonly IMemoryCache _cache = cache;
        public async Task<IViewComponentResult> InvokeAsync()
        {
           
            if (_cache.TryGetValue(CacheKeys.AmenitiesSection, out AmenitiesModel? cached) && cached != null)
            {
                return View(cached);
            }
            var jsonPath = JsonFilePath.AmenitiesFilePath(_env);
            if (!System.IO.File.Exists(jsonPath))
                return View(new AmenitiesModel()); 
            var json = await System.IO.File.ReadAllTextAsync(jsonPath);
            var data = JsonSerializer.Deserialize<AmenitiesModel>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new AmenitiesModel();
            _cache.Set(CacheKeys.AmenitiesSection, data);
            return View(data);
        }
    }
}
