using MakQR.Models.Admin;
using MakQR.Models.Config;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Json;

namespace MakQR.Controllers
{
    public class AdminController(IOptions<Appconfig> appConfig, IWebHostEnvironment env) : Controller

    {
        private readonly Appconfig _appConfig = appConfig.Value;
        private readonly IWebHostEnvironment _env = env;

        public IActionResult Index()
        {

            return View();
        }


        [HttpPost]
        public async ValueTask<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    message = "Please enter valid email and password."
                });
            }

            if (model.Email == _appConfig.AdminConfig.Email &&
                model.Password == _appConfig.AdminConfig.Password)
            {
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, model.Email),
            new Claim(ClaimTypes.Role, "Admin")
        };

                var identity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal);

                return Json(new
                {
                    success = true,
                    redirectUrl = Url.Action("AddHotelOverview", "Admin")
                });
            }

            return Json(new
            {
                success = false,
                message = "Invalid login attempt"
            });
        }


        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult AddHotelOverview()
        {
            var jsonPath = Path.Combine(_env.ContentRootPath, "HotelData.json");
            var data = System.IO.File.Exists(jsonPath) ? JsonSerializer.Deserialize<HotelSectionData>(System.IO.File.ReadAllText(jsonPath)) : new HotelSectionData();
            HotelSectionViewModel Response = new HotelSectionViewModel
            {
                Title = data.Title,
                Description = data.Description
            };
            return View(Response);
        }



        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SaveSection(HotelSectionViewModel request)
        {
            try
            {
                var jsonPath = Path.Combine(_env.ContentRootPath, "HotelData.json");
                var imageFolder = Path.Combine(_env.WebRootPath, "images");

                Directory.CreateDirectory(imageFolder);

                HotelSectionData data = System.IO.File.Exists(jsonPath)
                    ? JsonSerializer.Deserialize<HotelSectionData>(
                        System.IO.File.ReadAllText(jsonPath)) ?? new HotelSectionData()
                    : new HotelSectionData();

                // Always update text
                data.Title = request.Title;
                data.Description = request.Description;
                data.UpdatedOn = DateTime.Now;

                async Task ReplaceIfUploaded(IFormFile? file, string fixedName)
                {
                    if (file == null) return;

                    var path = Path.Combine(imageFolder, fixedName);

                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);

                    using var stream = new FileStream(path, FileMode.Create);
                    await file.CopyToAsync(stream);
                }

                await ReplaceIfUploaded(request.Image1, "image1.png");
                await ReplaceIfUploaded(request.Image2, "image2.png");
                await ReplaceIfUploaded(request.Image3, "image3.png");
                await ReplaceIfUploaded(request.Image4, "image4.png");
                await ReplaceIfUploaded(request.Image5, "image5.png");
                await ReplaceIfUploaded(request.Image6, "image6.png");

                System.IO.File.WriteAllText(
                    jsonPath,
                    JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true })
                );

                return Json(new
                {
                    success = true,
                    message = "Details updated successfully"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Failed to update details",
                    error = ex.Message
                });
            }
        }


    }
}

