using MakQR.Models.Common;
using MakQR.Models.Home;
using MakQR.Services.Interfaces.Home;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace MakQR.Services
{
    public class JsonSectionService(IWebHostEnvironment env,
        IMemoryCache cache) : IJsonSectionService
    {

        private readonly IWebHostEnvironment _env = env;
        private readonly IMemoryCache _cache = cache;
        public async ValueTask<AboutSection> GetHomeAboutSection()
        {
            if (_cache.TryGetValue(CacheKeys.HomeAboutUsSection, out AboutSection? cached))
                return cached ?? new();

            var json = await File.ReadAllTextAsync(JsonFilePath.HomeAboutUsSectionPath(_env));
            var data = JsonSerializer.Deserialize<AboutSection>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            _cache.Set(CacheKeys.HomeAboutUsSection, data);
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
            if (_cache.TryGetValue(CacheKeys.HomeWhySection, out WhyUsSection? cached))
                return cached ?? new();
            var json = await File.ReadAllTextAsync(JsonFilePath.HomeWhyUsSectionPath(_env));
            var data = JsonSerializer.Deserialize<WhyUsSection>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            _cache.Set(CacheKeys.HomeWhySection, data);
            return data ?? new WhyUsSection();
        }

        public async ValueTask<RoomsViewSection> GetRoomsView()
        {
            if (_cache.TryGetValue(CacheKeys.RoomsViewsSection, out RoomsViewSection? cached))
                return cached ?? new();
            var json = await File.ReadAllTextAsync(JsonFilePath.RoomsViewPath(_env));
            var data = JsonSerializer.Deserialize<RoomsViewSection>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            _cache.Set(CacheKeys.RoomsViewsSection, data);
            return data ?? new RoomsViewSection();
        }

        public async ValueTask<RoomsSection> GetRoomsSection()
        {
            if (_cache.TryGetValue(CacheKeys.RoomsTypesSection, out RoomsSection? cached))
                return cached ?? new();
            var json = await File.ReadAllTextAsync(JsonFilePath.RoomsTypePath(_env));
            var data = JsonSerializer.Deserialize<RoomsSection>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            _cache.Set(CacheKeys.RoomsTypesSection, data);
            return data ?? new RoomsSection();
        }

    }
}
