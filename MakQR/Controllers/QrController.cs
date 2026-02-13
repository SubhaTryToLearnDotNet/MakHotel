using MakQR.Models.Admin;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace MakQR.Controllers
{
    public class QrController(IWebHostEnvironment env) : Controller
    {
        private readonly IWebHostEnvironment _env = env;
        public IActionResult Index()
        {
            var jsonPath = Path.Combine(_env.ContentRootPath, "HotelData.json");
            var data = System.IO.File.Exists(jsonPath) ? JsonSerializer.Deserialize<HotelSectionData>(System.IO.File.ReadAllText(jsonPath)) : new HotelSectionData();
            return View(data);
        }
    }
}
