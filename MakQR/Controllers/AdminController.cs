using MakQR.Models.Admin;
using MakQR.Models.Config;
using MakQR.Models.Dto;
using MakQR.Models.Dtos;
using MakQR.Models.Home;
using MakQR.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Json;
using static System.Collections.Specialized.BitVector32;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace MakQR.Controllers
{
    public class AdminController(
        IOptions<Appconfig> appConfig,
        IWebHostEnvironment env,
        IMemoryCache cache) : Controller

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


            var jsonPath = JsonFilePath.AdminFilePath(_env);
            var json = await System.IO.File.ReadAllTextAsync(jsonPath);
            var admin = JsonSerializer.Deserialize<AdminUser>(json,  new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (model.Email == admin?.Username &&
                model.Password == admin.Password)
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

    


        #region Password Reset
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> UpdatePassword([FromBody]  ChangePasswordModel model)
        {

            if (model.NewPassword != model.ConfirmPassword)
            {
                return Json(new
                {
                    success = false,
                    message = "Password not match"
                });
            }
            string fileName = "admin.json";

            var jsonPath = JsonFilePath.AdminFilePath(_env);
            var json = await System.IO.File.ReadAllTextAsync(jsonPath);
            var admin = JsonSerializer.Deserialize<AdminUser>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (admin.Password != model.OldPassword)
            {

                return Json(new
                {
                    success = false,
                    message = "Old password wrong"
                });

            }

            admin.Password = model.NewPassword;
            await System.IO.File.WriteAllTextAsync(jsonPath, JsonSerializer.Serialize(admin, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }));



            return Json(new
            {
                success = true,
                message = "Password Updated"
            });


        }



        //[HttpPost]
        //public async Task<IActionResult> ResetPassword(string currentPassword, string newPassword)
        //{
        //    if (currentPassword != _appConfig.AdminConfig.Password)
        //    {
        //        return Json(new
        //        {
        //            success = false,
        //            message = "Current password is incorrect."
        //        });
        //    }
        //    if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
        //    {
        //        return Json(new
        //        {
        //            success = false,
        //            message = "New password must be at least 6 characters long."
        //        });
        //    }
        //    try
        //    {
        //        // Update the password in the appsettings.json file
        //        var configPath = Path.Combine(_env.ContentRootPath, "appsettings.json");
        //        var json = await System.IO.File.ReadAllTextAsync(configPath);
        //        var jsonDoc = JsonDocument.Parse(json);
        //        var root = jsonDoc.RootElement.Clone();
        //        using var stream = new MemoryStream();
        //        using (var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true }))
        //        {
        //            writer.WriteStartObject();
        //            foreach (var property in root.EnumerateObject())
        //            {
        //                if (property.NameEquals("AdminConfig"))
        //                {
        //                    writer.WritePropertyName("AdminConfig");
        //                    writer.WriteStartObject();
        //                    foreach (var adminProp in property.Value.EnumerateObject())
        //                    {
        //                        if (adminProp.NameEquals("Password"))
        //                        {
        //                            writer.WriteString("Password", newPassword);
        //                        }
        //                        else
        //                        {
        //                            adminProp.WriteTo(writer);
        //                        }
        //                    }
        //                    writer.WriteEndObject();
        //                }
        //                else
        //                {
        //                    property.WriteTo(writer);
        //                }
        //            }
        //            writer.WriteEndObject();
        //        }
        //        await System.IO.File.WriteAllBytesAsync(configPath, stream.ToArray());
        //        return Json(new
        //        {
        //            success = true,
        //            message = "Password updated successfully."
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new
        //        {
        //            success = false,
        //            message = "Failed to update password.",
        //            error = ex.Message
        //        });
        //    }
        //}
        #endregion

    }
}

