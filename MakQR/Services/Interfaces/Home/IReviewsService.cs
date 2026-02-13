using MakQR.Models.Home;

namespace MakQR.Services.Interfaces.Home
{
    public interface IReviewsService
    {
        Task<ReviewsSection> GetHomeReviewSection();
    }
}
