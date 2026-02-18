namespace MakQR.Models.Dtos
{
    public class HomeAboutUsSectionRequestDto
    {
        public string Title { get; set; } = string.Empty;
        public string DescriptionHtml { get; set; } = string.Empty;
        public IFormFile? AboutUsImage { get; set; }
    }
}
