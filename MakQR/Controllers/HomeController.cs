using MakQR.Models.Config;
using MakQR.Models.GoogleMapApi;
using MakQR.Services.Interfaces.Home;
using MakQR.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text.Json;

namespace MakQR.Controllers
{
    public class HomeController(IOptions<Appconfig> appConfig, IJsonSectionService iJsonSectionService,
        IWebHostEnvironment env,
        IMemoryCache cache) : Controller
    {
        private readonly IJsonSectionService _iJsonSectionService = iJsonSectionService;
        private readonly Appconfig _appConfig = appConfig.Value;
        private readonly IWebHostEnvironment _env = env;
        private readonly IMemoryCache _cache = cache;
        // ?? Static MAK HOTEL location (set once)
        private const double HotelLat = 12.955369955637972;   // update if needed
        private const double HotelLon = 77.64351675545757;
        public async Task<IActionResult> index()
        {
            var data = await _iJsonSectionService.GetBannerSection();
            return View(data);
        }



        #region Free
        [HttpPost]
        public async Task<IActionResult> GetDistanceAjax(string userLocation)
        {
            if (string.IsNullOrWhiteSpace(userLocation))
                return Json(new { success = false, message = "Please enter City / Area / PIN code" });

            try
            {
                // ?? Static hotel location
                double hotelLat = 12.971891;
                double hotelLon = 77.641154;

                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", "MakHotelApp");

                string input = userLocation.Trim();

                // If input contains PIN code ? prefer PIN
                var pinMatch = System.Text.RegularExpressions.Regex.Match(input, @"\b\d{6}\b");

                string query;

                if (pinMatch.Success)
                {
                    // Use PIN + country (best accuracy)
                    query = $"{pinMatch.Value}, India";
                }
                else
                {
                    // Use simplified area/city
                    query = $"{input}, India";
                }

                string url =
                    $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(query)}&format=json&limit=1";

                var response = await client.GetStringAsync(url);
                var results = System.Text.Json.JsonSerializer.Deserialize<List<GeoResult>>(response);

                if (results == null || results.Count == 0)
                    return Json(new { success = false, message = "Location not found" });

                double userLat = double.Parse(results[0].lat);
                double userLon = double.Parse(results[0].lon);

                double distance = GetDistanceInKm(userLat, userLon, hotelLat, hotelLon);

                return Json(new
                {
                    success = true,
                    distance = distance.ToString("0.00")
                });
            }
            catch
            {
                return Json(new { success = false, message = "Unable to calculate distance" });
            }
        }

        private double GetDistanceInKm(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371;

            double dLat = DegreesToRadians(lat2 - lat1);
            double dLon = DegreesToRadians(lon2 - lon1);

            double a =
                Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(DegreesToRadians(lat1)) *
                Math.Cos(DegreesToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private double DegreesToRadians(double deg) => deg * Math.PI / 180;

        private class GeoResult
        {
            public string lat { get; set; }
            public string lon { get; set; }
        }
        #endregion


        #region Google - Api
        public async Task<string> GetLatLng(string userLocation, string apikey)
        {
            userLocation = Uri.EscapeDataString(userLocation);
            string url = _appConfig.AdminConfig.GeocodeUrl + $"address={userLocation}" + $"&key={apikey}";
            using var client = new HttpClient();
            var response =
            await client.GetAsync(url);
            var json = await response.Content.ReadAsStringAsync();
            dynamic data = JsonConvert.DeserializeObject(json);
            string lat = data.results[0].geometry.location.lat;
            string lng = data.results[0].geometry.location.lng;
            return $"{lat},{lng}";
        }


        public async Task<IActionResult> GetDistance(string userLocation)
        {

            string ApiKey = string.Empty;
            if (_cache.TryGetValue(CacheKeys.GoogleSettings, out GoogleSettings? cached))
            {
                ApiKey = cached?.GoogleApiKey ?? "";
            }
            else
            {
                var json = await System.IO.File.ReadAllTextAsync(JsonFilePath.GoogleSettingFilePath(_env));
                var googledata = System.Text.Json.JsonSerializer.Deserialize<GoogleSettings>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new GoogleSettings();
                ApiKey = googledata.GoogleApiKey;
            }


            string userLatLng = await GetLatLng(userLocation, ApiKey);
            string url = _appConfig.AdminConfig.DistanceMatrixUrl + $"origins={userLatLng}" + $"&destinations={HotelLat},{HotelLon}" + $"&key={ApiKey}";
            using var client = new HttpClient();
            var response = await client.GetAsync(url);
            var ApiResponse = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<GoogleDistanceResponse>(ApiResponse);
            return Json(new { success = true, distance = data.rows[0].elements[0].distance.text, });

        }
        #endregion


    }
}
