using Microsoft.AspNetCore.Mvc;

namespace MakQR.Controllers
{
    public class HomeController() : Controller
    {
        // ?? Static MAK HOTEL location (set once)
        private const double HotelLat = 12.955369955637972 ;   // update if needed
        private const double HotelLon = 77.64351675545757;
        public IActionResult index()
        {
            return View();
        }

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


    }
}
