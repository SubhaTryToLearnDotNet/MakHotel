using MakQR.Models.Admin;
using MakQR.Models.Common;
using MakQR.Models.Config;
using MakQR.Models.Dto;
using MakQR.Models.Dtos;
using MakQR.Models.Home;
using MakQR.Services;
using MakQR.Services.Interfaces.Home;
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
    public class AdminController(IOptions<Appconfig> appConfig, IWebHostEnvironment env, IMemoryCache cache,
        IReviewsService iReviewsService,
        IJsonSectionService iJsonSectionService) : Controller

    {
        private readonly Appconfig _appConfig = appConfig.Value;
        private readonly IWebHostEnvironment _env = env;
        private readonly IMemoryCache _cache = cache;
        private readonly IReviewsService _iReviewsService = iReviewsService;
        private readonly IJsonSectionService _iJsonSectionService = iJsonSectionService;

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

        #region Banner
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<IActionResult> AddBanner()
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

        [HttpPost]
        [Authorize(Roles = RoleNames.Admin)]
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
            await ReplaceIfUploaded(model.ImageFile1, "banner_img.png", imageFolder);
            await ReplaceIfUploaded(model.ImageFile2, "banner_img1.png", imageFolder);
            await ReplaceIfUploaded(model.ImageFile3, "banner_img2.png", imageFolder);
            await ReplaceIfUploaded(model.ImageFile4, "banner_img3.png", imageFolder);

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
        #endregion

        #region  AboutUs Section 
        [HttpGet]
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<IActionResult> AboutUs()
        {
            HomeAboutUsSectionRequestDto Response = new HomeAboutUsSectionRequestDto();
            if (_cache.TryGetValue(CacheKeys.HomeAboutUsSection, out AboutSection? cached))
            {
                Response.Title = cached?.Title ?? "";
                Response.DescriptionHtml = cached?.DescriptionHtml ?? "";
                return View(Response);
            }
            var json = await System.IO.File.ReadAllTextAsync(JsonFilePath.HomeEventSectionPath(_env));
            var data = JsonSerializer.Deserialize<AboutSection>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new AboutSection();
            _cache.Set(CacheKeys.HomeAboutUsSection, data);
            Response.Title = data.Title;
            Response.DescriptionHtml = data.DescriptionHtml;
            return View(Response);
        }

        [HttpPost]
        [Authorize(Roles = RoleNames.Admin)]
        public async ValueTask<IActionResult> AddAboutUsSection(HomeAboutUsSectionRequestDto request)
        {
            if (!ModelState.IsValid) { return BadRequest(); }
            var jsonPath = JsonFilePath.HomeAboutUsSectionPath(_env);
            var imageFolder = Path.Combine(_env.WebRootPath, "images");
            AboutSection eventSection = new()
            {
                Title = request.Title,
                DescriptionHtml = request.DescriptionHtml,
                UpdatedOn = DateTime.Now
            };
            await ReplaceIfUploaded(request.AboutUsImage, "about_us.png", imageFolder);
            await System.IO.File.WriteAllTextAsync(jsonPath, JsonSerializer.Serialize(eventSection, new JsonSerializerOptions { WriteIndented = true }));
            _cache.Set(CacheKeys.HomeAboutUsSection, eventSection);
            return Json(new { success = true, message = "Details updated successfully" });
        }
        #endregion

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
            if (!ModelState.IsValid) { return BadRequest(); }
            var jsonPath = JsonFilePath.HomeEventSectionPath(_env);
            var imageFolder = Path.Combine(_env.WebRootPath, "images");
            EventSection eventSection = new()
            {
                Title = request.Title,
                DescriptionHtml = request.DescriptionHtml,
                UpdatedOn = DateTime.Now
            };
            await ReplaceIfUploaded(request.EventImg, "home_event.png", imageFolder);
            await System.IO.File.WriteAllTextAsync(jsonPath, JsonSerializer.Serialize(eventSection, new JsonSerializerOptions { WriteIndented = true }));
            _cache.Set(CacheKeys.HomeEventSection, eventSection);
            return Json(new { success = true, message = "Details updated successfully" });
        }
        #endregion

        #region WhyUs
        [HttpGet]
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<IActionResult> WhyUs()
        {
            HomeWhyUsSectionRequestDto Response = new HomeWhyUsSectionRequestDto();
            if (_cache.TryGetValue(CacheKeys.HomeWhySection, out AboutSection? cached))
            {
                Response.Title = cached?.Title ?? "";
                Response.DescriptionHtml = cached?.DescriptionHtml ?? "";
                return View(Response);
            }
            var json = await System.IO.File.ReadAllTextAsync(JsonFilePath.HomeWhyUsSectionPath(_env));
            var data = JsonSerializer.Deserialize<WhyUsSection>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new WhyUsSection();
            _cache.Set(CacheKeys.HomeWhySection, data);
            Response.Title = data.Title;
            Response.DescriptionHtml = data.DescriptionHtml;
            return View(Response);
        }


        [HttpPost]
        [Authorize(Roles = RoleNames.Admin)]
        public async ValueTask<IActionResult> AddWhyUsSection(HomeWhyUsSectionRequestDto request)
        {
            if (!ModelState.IsValid) { return BadRequest(); }
            var jsonPath = JsonFilePath.HomeWhyUsSectionPath(_env);
            var imageFolder = Path.Combine(_env.WebRootPath, "images");
            WhyUsSection Section = new()
            {
                Title = request.Title,
                DescriptionHtml = request.DescriptionHtml,
                UpdatedOn = DateTime.Now
            };
            await ReplaceIfUploaded(request.Image, "why_us.png", imageFolder);
            await System.IO.File.WriteAllTextAsync(jsonPath, JsonSerializer.Serialize(Section, new JsonSerializerOptions { WriteIndented = true }));
            _cache.Set(CacheKeys.HomeWhySection, Section);
            return Json(new { success = true, message = "Details updated successfully" });
        }
        #endregion

        #region Room Views Section
        [HttpGet]
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<IActionResult> Views()
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


        [HttpPost]
        [Authorize(Roles = RoleNames.Admin)]
        public async ValueTask<IActionResult> AddRoomViewsSection(HomeRoomViewRequestDto request)
        {
            if (!ModelState.IsValid) { return BadRequest(); }
            var jsonPath = JsonFilePath.RoomsViewPath(_env);
            var imageFolder = Path.Combine(_env.WebRootPath, "images");

            await ReplaceIfUploaded(request.Image1, "room_views1.png", imageFolder);
            await ReplaceIfUploaded(request.Image2, "room_views2.png", imageFolder);
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
        #endregion

        #region Room Types Section
        [HttpGet]
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<IActionResult> Rooms()
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


        [HttpPost]
        [Authorize(Roles = RoleNames.Admin)]
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
                await ReplaceIfUploaded(request.ImageFile1, "room_type_1.png", imageFolder);
                await ReplaceIfUploaded(request.ImageFile2, "room_type_2.png", imageFolder);
                await ReplaceIfUploaded(request.ImageFile3, "room_type_3.png", imageFolder);
                await ReplaceIfUploaded(request.ImageFile4, "room_type_4.png", imageFolder);
                await ReplaceIfUploaded(request.ImageFile5, "room_type_5.png", imageFolder);
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

                await ReplaceIfUploaded(request.Image, "gallery_img.png", imageFolder);
                await ReplaceIfUploaded(request.Image1, "gallery_img1.png", imageFolder);
                await ReplaceIfUploaded(request.Image2, "gallery_img2.png", imageFolder);
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

        #region Reviews Section
        [HttpGet]
        [Authorize(Roles = RoleNames.Admin)]

        public async Task<IActionResult> Reviews()
        {
            if (_cache.TryGetValue(CacheKeys.Reviews, out ReviewsSection? cached))
            {
                return View(cached);
            }
            var json = await System.IO.File.ReadAllTextAsync(JsonFilePath.ReviewFilePath(_env));
            var section = JsonSerializer.Deserialize<ReviewsSection>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new ReviewsSection();
            return View(section);
        }

        public IActionResult AddReview()
        {
            return View(new ReviewRequestDto());
        }

        [HttpPost]
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<IActionResult> AddReview(ReviewRequestDto request)
        {
            if (!ModelState.IsValid) { return BadRequest(); }
            var filePath = JsonFilePath.ReviewFilePath(_env);
            ReviewsSection section;
            if (System.IO.File.Exists(JsonFilePath.ReviewFilePath(_env)))
            {
                var json = await System.IO.File.ReadAllTextAsync(filePath);
                section = JsonSerializer.Deserialize<ReviewsSection>(
                    json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                ) ?? new ReviewsSection();
            }
            else
            {
                section = new ReviewsSection();
            }
            section.Items.Add(new ReviewItem
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = request.Name,
                CommentHtml = request.CommentHtml,
                Tag = request.Tag
            });

            section.UpdatedOn = DateTime.Now;
            await System.IO.File.WriteAllTextAsync(
                filePath,
                JsonSerializer.Serialize(section, new JsonSerializerOptions
                {
                    WriteIndented = true
                })
            );
            _cache.Set(CacheKeys.Reviews, section);

            return Json(new { success = true, message = "Review saved successfully" });
        }

        [Authorize(Roles = RoleNames.Admin)]
        public async Task<IActionResult> EditReview(string id)
        {
            var ReviewVewModel = await _iReviewsService.GetHomeReviewSection();
            var review = ReviewVewModel.Items.FirstOrDefault(x => x.Id == id);
            if (review == null)
                return NotFound();

            return View(review);
        }

        [Authorize(Roles = RoleNames.Admin)]
        public async Task<IActionResult> UpdateReview(ReviewItem model)
        {
            if (!ModelState.IsValid) { return BadRequest(); }

            var ReviewVewModel = await _iReviewsService.GetHomeReviewSection();

            var existing = ReviewVewModel.Items.FirstOrDefault(x => x.Id == model.Id);
            if (existing == null)
                return Json(new { success = false, message = "Review not found" });


            existing.Name = model.Name;
            existing.CommentHtml = model.CommentHtml;
            existing.Tag = model.Tag;

            ReviewVewModel.UpdatedOn = DateTime.Now;
            await System.IO.File.WriteAllTextAsync(
                JsonFilePath.ReviewFilePath(_env),
                JsonSerializer.Serialize(ReviewVewModel, new JsonSerializerOptions
                {
                    WriteIndented = true
                })
            );

            _cache.Set(CacheKeys.Reviews, ReviewVewModel);

            return Json(new { success = true, message = "Review Update successfully" });
        }

        [Authorize(Roles = RoleNames.Admin)]
        public async Task<IActionResult> DeleteReview(string id)
        {
            var filePath = JsonFilePath.ReviewFilePath(_env);

            if (!System.IO.File.Exists(filePath))
                return Json(new { success = false, message = "Data file not found" });

            var json = await System.IO.File.ReadAllTextAsync(filePath);
            var section = JsonSerializer.Deserialize<ReviewsSection>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            ) ?? new ReviewsSection();

            var removed = section.Items.RemoveAll(x => x.Id == id);
            if (removed == 0)
                return Json(new { success = false, message = "Review not found" });

            section.UpdatedOn = DateTime.Now;

            await System.IO.File.WriteAllTextAsync(
                filePath,
                JsonSerializer.Serialize(section, new JsonSerializerOptions { WriteIndented = true })
            );

            _cache.Set(CacheKeys.Reviews, section);


            return Json(new
            {
                success = true,
                message = "Review deleted successfully"
            });
        }

        #endregion

        #region Common
        private static async Task ReplaceIfUploaded(IFormFile? file, string fixedName, string folderName)
        {
            if (file == null) return;

            var path = Path.Combine(folderName, fixedName);

            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);

            using var stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream);
        }
        #endregion
    }
}

