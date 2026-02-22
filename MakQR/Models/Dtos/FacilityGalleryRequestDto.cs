namespace MakQR.Models.Dtos
{
    public class FacilityGalleryRequestDto
    {
        public string Title { get; set; } = string.Empty;
        public IFormFile? Image { get; set; }
        public string Caption1 { get; set; } = string.Empty;
        public IFormFile? Image1 { get; set; }
        public string Caption2 { get; set; } = string.Empty;
        public IFormFile? Image2 { get; set; }
        public string Caption3 { get; set; } = string.Empty;
    }
}
