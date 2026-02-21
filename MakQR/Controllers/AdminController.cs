using MakQR.Models.Admin;
using MakQR.Models.Config;
using MakQR.Models.Dto;
using MakQR.Models.Dtos;
using MakQR.Models.Home;
using MakQR.Services;
using MakQR.Services.Interfaces.Home;
using MakQR.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Json;
using static MakQR.Models.Home.RoomsSection;
using static System.Collections.Specialized.BitVector32;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MakQR.Controllers
{
    public class AdminController(
        IOptions<Appconfig> appConfig,
        IWebHostEnvironment env) : Controller

    {
        private readonly Appconfig _appConfig = appConfig.Value;
        private readonly IWebHostEnvironment _env = env;
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


        #region Qr
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
                      await System.IO.File.ReadAllTextAsync(jsonPath)) ?? new HotelSectionData()
                    : new HotelSectionData();

                // Always update text
                data.Title = request.Title;
                data.Description = request.Description;
                data.UpdatedOn = DateTime.Now;


                await AppHelper.ReplaceIfUploaded(request.Image1, "image1.png", imageFolder);
                await AppHelper.ReplaceIfUploaded(request.Image2, "image2.png", imageFolder);
                await AppHelper.ReplaceIfUploaded(request.Image3, "image3.png", imageFolder);
                await AppHelper.ReplaceIfUploaded(request.Image4, "image4.png", imageFolder);
                await AppHelper.ReplaceIfUploaded(request.Image5, "image5.png", imageFolder);
                await AppHelper.ReplaceIfUploaded(request.Image6, "image6.png", imageFolder);
                await System.IO.File.WriteAllTextAsync(jsonPath, JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true }));
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
        #endregion



        #region Facility Gallery Section
        [HttpGet]
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<IActionResult> Facility()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = RoleNames.Admin)]
        public async ValueTask<IActionResult> AddFacilitySection(FacilityGalleryRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid) { return BadRequest(); }
                var imageFolder = Path.Combine(_env.WebRootPath, "images");

                await AppHelper.ReplaceIfUploaded(request.Image, "gallery_img.png", imageFolder);
                await AppHelper.ReplaceIfUploaded(request.Image1, "gallery_img1.png", imageFolder);
                await AppHelper.ReplaceIfUploaded(request.Image2, "gallery_img2.png", imageFolder);
                return Json(new { success = true, message = "Details updated successfully" });
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
        #endregion

    }
}

