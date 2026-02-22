using MakQR.Models.Home;
using MakQR.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace MakQR.ViewComponents
{
    public class FacilityViewComponent(IWebHostEnvironment env, IMemoryCache cache) : ViewComponent
    {
        private readonly IWebHostEnvironment _env = env;
        private readonly IMemoryCache _cache = cache;
        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (_cache.TryGetValue(CacheKeys.Facility, out FacilityGallerySection? cached))
                return View(cached);
            var json = await File.ReadAllTextAsync(JsonFilePath.FacilityGalleryFilePath(_env));
            var data = JsonSerializer.Deserialize<FacilityGallerySection>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            _cache.Set(CacheKeys.Facility, data);
            return View(data);
        }
    }
}
