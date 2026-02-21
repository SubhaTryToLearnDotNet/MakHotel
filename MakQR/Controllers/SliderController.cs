using MakQR.Models.Dtos;
using MakQR.Models.Home;
using MakQR.Services.Interfaces.Home;
using MakQR.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace MakQR.Controllers
{
    [Authorize(Roles = RoleNames.Admin)]
    public class SliderController(IWebHostEnvironment env, IMemoryCache cache,
        IJsonSectionService iJsonSectionService) : Controller

    {
        private readonly IWebHostEnvironment _env = env;
        private readonly IMemoryCache _cache = cache;
        private readonly IJsonSectionService _iJsonSectionService = iJsonSectionService;

        public async ValueTask<IActionResult> Index()
        {
            var data = await _iJsonSectionService.GetBannerSection();
            BannerRequestDto banner = new()
            {
                TitleHtml1 = data.TitleHtml1,
                Subtitle1 = data.Subtitle1,
                ButtonText1 = data.ButtonText1,
                ButtonUrl1 = data.ButtonUrl1,
                TitleHtml2 = data.TitleHtml2,
                Subtitle2 = data.Subtitle2,
                ButtonText2 = data.ButtonText2,
                ButtonUrl2 = data.ButtonUrl2,
                TitleHtml3 = data.TitleHtml3,
                Subtitle3 = data.Subtitle3,
                ButtonText3 = data.ButtonText3,
                ButtonUrl3 = data.ButtonUrl3,
                TitleHtml4 = data.TitleHtml4,
                Subtitle4 = data.Subtitle4,
                ButtonText4 = data.ButtonText4,
                ButtonUrl4 = data.ButtonUrl4
            };

            return View(banner);
        }

        public async Task<IActionResult> SaveBanner(BannerRequestDto model)
        {
            if (!ModelState.IsValid) { return BadRequest(); }

            var jsonPath = JsonFilePath.BannerFilePath(_env);
            var imageFolder = Path.Combine(_env.WebRootPath, "images");

            if (!Directory.Exists(imageFolder))
                Directory.CreateDirectory(imageFolder);

            var bannerSection = new BannerSection()
            {
                TitleHtml1 = model.TitleHtml1,
                Subtitle1 = model.Subtitle1,
                ButtonText1 = model.ButtonText1,
                ButtonUrl1 = model.ButtonUrl1,
                TitleHtml2 = model.TitleHtml2,
                Subtitle2 = model.Subtitle2,
                ButtonText2 = model.ButtonText2,
                ButtonUrl2 = model.ButtonUrl2,
                TitleHtml3 = model.TitleHtml3,
                Subtitle3 = model.Subtitle3,
                ButtonText3 = model.ButtonText3,
                ButtonUrl3 = model.ButtonUrl3,
                TitleHtml4 = model.TitleHtml4,
                Subtitle4 = model.Subtitle4,
                ButtonText4 = model.ButtonText4,
                ButtonUrl4 = model.ButtonUrl4
            };
            await AppHelper.ReplaceIfUploaded(model.ImageFile1, "banner_img.png", imageFolder);
            await AppHelper.ReplaceIfUploaded(model.ImageFile2, "banner_img1.png", imageFolder);
            await AppHelper.ReplaceIfUploaded(model.ImageFile3, "banner_img2.png", imageFolder);
            await AppHelper.ReplaceIfUploaded(model.ImageFile4, "banner_img3.png", imageFolder);

            var json = JsonSerializer.Serialize(bannerSection, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            await System.IO.File.WriteAllTextAsync(jsonPath, json);
            _cache.Set(CacheKeys.HomeBanner, bannerSection);

            return Json(new
            {
                success = true,
                message = "Review deleted successfully"
            });
        }
    }
}
