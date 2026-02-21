using MakQR.Models.Config;
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
    public class RoomViewsController(IWebHostEnvironment env, IMemoryCache cache
       ) : Controller
    {
        private readonly IWebHostEnvironment _env = env;
        private readonly IMemoryCache _cache = cache;
        public async ValueTask<IActionResult> Index()
        {
            HomeRoomViewRequestDto Response = new HomeRoomViewRequestDto();
            if (_cache.TryGetValue(CacheKeys.RoomsViewsSection, out RoomsViewSection? cached))
            {
                Response.Title = cached?.Title ?? "";
                Response.Description = cached?.Description ?? "";
                Response.Caption1 = cached?.Items[0].Caption ?? "";
                Response.Caption2 = cached?.Items[1].Caption ?? "";
                return View(Response);
            }
            var json = await System.IO.File.ReadAllTextAsync(JsonFilePath.HomeWhyUsSectionPath(_env));
            var data = JsonSerializer.Deserialize<RoomsViewSection>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new RoomsViewSection();
            _cache.Set(CacheKeys.RoomsViewsSection, data);
            Response.Title = data.Title;
            Response.Description = data.Description;
            Response.Caption1 = cached?.Items[0].Caption ?? "";
            Response.Caption2 = cached?.Items[1].Caption ?? "";
            return View(Response);
        }

        public async ValueTask<IActionResult> AddRoomViewsSection(HomeRoomViewRequestDto request)
        {
            if (!ModelState.IsValid) { return BadRequest(); }
            var jsonPath = JsonFilePath.RoomsViewPath(_env);
            var imageFolder = Path.Combine(_env.WebRootPath, "images");

            await AppHelper.ReplaceIfUploaded(request.Image1, "room_views1.png", imageFolder);
            await AppHelper.ReplaceIfUploaded(request.Image2, "room_views2.png", imageFolder);
            await AppHelper.ReplaceIfUploaded(request.Image2, "room_views2.png", imageFolder);
            RoomsViewSection Section = new()
            {
                Title = request.Title,
                Description = request.Description,
                UpdatedOn = DateTime.Now,
                Items = new List<ViewItem>
                {
                    new ViewItem { Caption = request.Caption1,Image = "room_views1.png" },
                    new ViewItem { Caption = request.Caption2 ,Image= "room_views2.png"}
                }
            };
            await System.IO.File.WriteAllTextAsync(jsonPath, JsonSerializer.Serialize(Section, new JsonSerializerOptions { WriteIndented = true }));
            _cache.Set(CacheKeys.RoomsViewsSection, Section);
            return Json(new { success = true, message = "Details updated successfully" });
        }
    }
}
