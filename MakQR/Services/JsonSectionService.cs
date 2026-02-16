using MakQR.Models.Common;
using MakQR.Models.Home;
using MakQR.Services.Interfaces.Home;
using Microsoft.Extensions.Caching.Memory;
using System.IO;
using System.Text.Json;

namespace MakQR.Services
{
    public class JsonSectionService(IWebHostEnvironment env,
        IMemoryCache cache) : IJsonSectionService
    {

        private readonly IWebHostEnvironment _env = env;
        private readonly IMemoryCache _cache = cache;
        private string FileName = string.Empty;
        private string FilePath => Path.Combine(_env.ContentRootPath, "ContentData", FileName);

        public async ValueTask<AboutSection> GetHomeAboutSection()
        {
            var cacheKey = $"HOME_ABOUT_CACHE";
            if (_cache.TryGetValue(cacheKey, out AboutSection? cached))
                return cached ?? new();

            FileName = $"about.json";
            if (!File.Exists(GetFilePath(FileName)))
                return new AboutSection();

            var json = await File.ReadAllTextAsync(FilePath);
            var data = JsonSerializer.Deserialize<AboutSection>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            _cache.Set(cacheKey, data);
            return data ?? new AboutSection();
        }

        public async ValueTask<EventSection> GetHomeEventSection()
        {
            if (_cache.TryGetValue(CacheKeys.HomeEventSection, out EventSection? cached))
                return cached ?? new();
            var json = await File.ReadAllTextAsync(JsonFilePath.HomeEventSectionPath(_env));
            var data = JsonSerializer.Deserialize<EventSection>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            _cache.Set(CacheKeys.HomeEventSection, data);
            return data ?? new EventSection();
        }

        public async ValueTask<WhyUsSection> GetHomeWhyUsSection()
        {
            var cacheKey = $"HOME_WHYUS_CACHE";
            if (_cache.TryGetValue(cacheKey, out WhyUsSection? cached))
                return cached ?? new();

            FileName = $"events.json";
            if (!File.Exists(GetFilePath(FileName)))
                return new WhyUsSection();

            var json = await File.ReadAllTextAsync(FilePath);
            var data = JsonSerializer.Deserialize<WhyUsSection>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            _cache.Set(cacheKey, data);
            return data ?? new WhyUsSection();
        }

        public async ValueTask<RoomsViewSection> GetRoomsView()
        {
            var cacheKey = $"ROOMSVIEW_CACHE";
            if (_cache.TryGetValue(cacheKey, out RoomsViewSection? cached))
                return cached ?? new();

            FileName = $"rooms-view.json";
            if (!File.Exists(GetFilePath(FileName)))
                return new RoomsViewSection();

            var json = await File.ReadAllTextAsync(FilePath);
            var data = JsonSerializer.Deserialize<RoomsViewSection>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            _cache.Set(cacheKey, data);
            return data ?? new RoomsViewSection();
        }

        public async ValueTask<RoomsSection> GetRoomsSection()
        {
            var cacheKey = $"ROOMS_CACHE";
            if (_cache.TryGetValue(cacheKey, out RoomsSection? cached))
                return cached ?? new();

            FileName = $"rooms-view.json";
            if (!File.Exists(GetFilePath(FileName)))
                return new RoomsSection();

            var json = await File.ReadAllTextAsync(FilePath);
            var data = JsonSerializer.Deserialize<RoomsSection>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            _cache.Set(cacheKey, data);
            return data ?? new RoomsSection();
        }

        private string GetFilePath(string fileName, bool ensureDirectory = false)
        {
            var directory = Path.Combine(_env.ContentRootPath, "ContentData");

            if (ensureDirectory)
                Directory.CreateDirectory(directory);

            return Path.Combine(directory, fileName);
        }

    }
}
