using MakQR.Models.Admin;
using MakQR.Models.Common;
using MakQR.Models.Config;
using MakQR.Models.Dto;
using MakQR.Models.Home;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MakQR.Controllers
{
    public class AdminController(IOptions<Appconfig> appConfig, IWebHostEnvironment env, IMemoryCache cache) : Controller

    {
        private readonly Appconfig _appConfig = appConfig.Value;
        private readonly IWebHostEnvironment _env = env;
        private readonly IMemoryCache _cache = cache;

        #region Admin Login
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
            new Claim(ClaimTypes.Role, RoleNames.Admin)
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

        #endregion



        [HttpGet]
        [Authorize(Roles = RoleNames.Admin)]
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
        [Authorize(Roles = RoleNames.Admin)]
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


                await ReplaceIfUploaded(request.Image1, "image1.png", imageFolder);
                await ReplaceIfUploaded(request.Image2, "image2.png", imageFolder);
                await ReplaceIfUploaded(request.Image3, "image3.png", imageFolder);
                await ReplaceIfUploaded(request.Image4, "image4.png", imageFolder);
                await ReplaceIfUploaded(request.Image5, "image5.png", imageFolder);
                await ReplaceIfUploaded(request.Image6, "image6.png", imageFolder);

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

        #region  Events 
        [HttpGet]
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<IActionResult> Event()
        {
            HomeEventSectionRequestDto Response = new HomeEventSectionRequestDto();
            if (_cache.TryGetValue(CacheKeys.HomeEventSection, out EventSection? cached))
            {
                Response.Title = cached?.Title ?? "";
                Response.DescriptionHtml = cached?.DescriptionHtml ?? "";
                return View(Response);
            }
            var path = JsonFilePath.HomeEventSectionPath(_env);
            if (!System.IO.File.Exists(path))
            {
                var empty = new HomeEventSectionRequestDto();
                _cache.Set(CacheKeys.HomeEventSection, empty);
                return View(empty);
            }
            var json = await System.IO.File.ReadAllTextAsync(path);
            var data = JsonSerializer.Deserialize<EventSection>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new EventSection();
            _cache.Set(CacheKeys.HomeEventSection, data);
            Response.Title = data.Title;
            Response.DescriptionHtml = data.DescriptionHtml;
            return View(Response);
        }





        [HttpPost]
        [Authorize(Roles = RoleNames.Admin)]
        public async ValueTask<IActionResult> AddEventSection(HomeEventSectionRequestDto request)
        {
            var jsonPath = JsonFilePath.HomeEventSectionPath(_env);
            var imageFolder = Path.Combine(_env.WebRootPath, "images");

            EventSection eventSection = new()
            {
                Title = request.Title,
                DescriptionHtml = request.DescriptionHtml,
                UpdatedOn = DateTime.Now
            };

            await ReplaceIfUploaded(request.EventImg, "HomeEvent.png", imageFolder);

            // Save JSON
            System.IO.File.WriteAllText(
                jsonPath,
                JsonSerializer.Serialize(eventSection, new JsonSerializerOptions
                {
                    WriteIndented = true
                })
            );
            _cache.Set(CacheKeys.HomeEventSection, eventSection);
            return Json(new { success = true, message = "Details updated successfully" });
        }
        #endregion


        private static async Task ReplaceIfUploaded(IFormFile? file, string fixedName, string folderName)
        {
            if (file == null) return;

            var path = Path.Combine(folderName, fixedName);

            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);

            using var stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream);
        }
    }
}

