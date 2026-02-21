using MakQR.Models.Dtos;
using MakQR.Models.Home;
using MakQR.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace MakQR.Controllers
{
    [Authorize(Roles = RoleNames.Admin)]
    public class RoomsController(IWebHostEnvironment env, IMemoryCache cache
       ) : Controller

    {
        private readonly IWebHostEnvironment _env = env;
        private readonly IMemoryCache _cache = cache;
        public async ValueTask<IActionResult> Index()
        {
            RoomsTypeRequestDto Response = new RoomsTypeRequestDto();
            if (_cache.TryGetValue(CacheKeys.RoomsTypesSection, out RoomsSection? cached))
            {
                Response.Title = cached?.Title ?? "";
                Response.Subtitle = cached?.Subtitle ?? "";
                Response.GridHeading = cached?.GridHeading ?? "";
                Response.Description = cached?.Description ?? "";

                return View(Response);
            }
            var json = await System.IO.File.ReadAllTextAsync(JsonFilePath.RoomsTypePath(_env));
            var data = JsonSerializer.Deserialize<RoomsSection>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new RoomsSection();
            _cache.Set(CacheKeys.RoomsTypesSection, data);
            Response.Title = data.Title;
            Response.Subtitle = data.Subtitle;
            Response.GridHeading = cached?.GridHeading ?? "";
            Response.Description = cached?.Description ?? "";
            return View(Response);
        }

        public async ValueTask<IActionResult> AddRoomTypeSection(RoomsTypeRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid) { return BadRequest(); }
                var jsonPath = JsonFilePath.RoomsTypePath(_env);
                var imageFolder = Path.Combine(_env.WebRootPath, "images");
                RoomsSection roomsSection = new()
                {
                    Title = request.Title,
                    Subtitle = request.Subtitle,
                    GridHeading = request.GridHeading,
                    Description = request.Description,
                    UpdatedOn = DateTime.Now

                };
                await AppHelper.ReplaceIfUploaded(request.ImageFile1, "room_type_1.png", imageFolder);
                await AppHelper.ReplaceIfUploaded(request.ImageFile2, "room_type_2.png", imageFolder);
                await AppHelper.ReplaceIfUploaded(request.ImageFile3, "room_type_3.png", imageFolder);
                await AppHelper.ReplaceIfUploaded(request.ImageFile4, "room_type_4.png", imageFolder);
                await AppHelper.ReplaceIfUploaded(request.ImageFile5, "room_type_5.png", imageFolder);
                await System.IO.File.WriteAllTextAsync(jsonPath, JsonSerializer.Serialize(roomsSection, new JsonSerializerOptions { WriteIndented = true }));
                _cache.Set(CacheKeys.RoomsTypesSection, roomsSection);
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
    }
}
