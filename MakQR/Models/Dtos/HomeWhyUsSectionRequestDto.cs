namespace MakQR.Models.Dtos
{
    public class HomeWhyUsSectionRequestDto
    {
        public string Title { get; set; } = string.Empty;
        public string DescriptionHtml { get; set; } = string.Empty;
        public IFormFile? Image { get; set; }
    }
}
