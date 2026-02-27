using MakQR.Models.Home;
using MakQR.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace MakQR.ViewComponents
{
    public class HeaderViewComponent(IWebHostEnvironment env,
        IMemoryCache cache) : ViewComponent
    {
        private readonly IWebHostEnvironment _env = env;
        private readonly IMemoryCache _cache = cache;
        public async Task<IViewComponentResult> InvokeAsync()
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
    }
}
