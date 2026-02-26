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
    public class ReviewsController(IWebHostEnvironment env, IMemoryCache cache,
        IReviewsService iReviewsService,
        IJsonSectionService iJsonSectionService) : Controller
    {

        private readonly IWebHostEnvironment _env = env;
        private readonly IMemoryCache _cache = cache;
        private readonly IReviewsService _iReviewsService = iReviewsService;
        public async ValueTask<IActionResult> Index()
        {
            if (_cache.TryGetValue(CacheKeys.Reviews, out ReviewsSection? cached))
            {
                return View(cached);
            }
            var json = await System.IO.File.ReadAllTextAsync(JsonFilePath.ReviewFilePath(_env));
            var section = JsonSerializer.Deserialize<ReviewsSection>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new ReviewsSection();
            return View(section);
        }

        [HttpGet]
        public async Task<IActionResult> Title()
        {
            var filePath = JsonFilePath.ReviewFilePath(_env);

            ReviewsSection section = new ReviewsSection();

            if (System.IO.File.Exists(filePath))
            {
                var json = await System.IO.File.ReadAllTextAsync(filePath);

                section = JsonSerializer.Deserialize<ReviewsSection>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }
                ) ?? new ReviewsSection();
            }

            ReviewHeaderRequest response = new ReviewHeaderRequest
            {
                Title = section.Title,
                Subtitle = section.Subtitle
            };

            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> Title(ReviewHeaderRequest Request)
        {

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
            section.Title = Request.Title;
            section.Subtitle = Request.Subtitle;
            section.UpdatedOn = DateTime.Now;
            await System.IO.File.WriteAllTextAsync(
                filePath,
                JsonSerializer.Serialize(section, new JsonSerializerOptions
                {
                    WriteIndented = true
                })
            );
            _cache.Set(CacheKeys.Reviews, section);
            return Json(new { success = true, message = "Details saved successfully" });
        }

        public IActionResult AddReview()
        {
            return View(new ReviewRequestDto());
        }

        [HttpPost]
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
    }
}
