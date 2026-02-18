namespace MakQR.Models.Dto
{
    public class HomeEventSectionRequestDto
    {
        public string Title { get; set; } = string.Empty;
        public string DescriptionHtml { get; set; } = string.Empty;
        public IFormFile? EventImg { get; set; }
    }
}
