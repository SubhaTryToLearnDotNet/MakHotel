using MakQR.Models.Home;
using MakQR.Services.Interfaces.Home;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace MakQR.Services
{
    public class JsonReviewsService(IWebHostEnvironment env,
        IMemoryCache cache) : IReviewsService
    {

        private readonly IWebHostEnvironment _env = env;

        private readonly IMemoryCache _cache = cache;
        private const string CacheKey = "REVIEWS_CACHE";
        private const string FileName = "reviews.json";
        private string FilePath => Path.Combine(_env.ContentRootPath, "ContentData", FileName);
        public async Task<ReviewsSection> GetHomeReviewSection()
        {
            if (_cache.TryGetValue(CacheKey, out ReviewsSection? cached))
                return cached ?? new();

            if (!File.Exists(FilePath))
                return new ReviewsSection();

            var json = await File.ReadAllTextAsync(FilePath);
            var data = JsonSerializer.Deserialize<ReviewsSection>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            _cache.Set(CacheKey, data);
            return data ?? new ReviewsSection();
        }
    }
}
