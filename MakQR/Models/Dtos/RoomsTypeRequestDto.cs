namespace MakQR.Models.Dtos
{
    public class RoomsTypeRequestDto
    {
        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public IFormFile? ImageFile1 { get; set; }
        public IFormFile? ImageFile2 { get; set; }
        public IFormFile? ImageFile3 { get; set; }
        public IFormFile? ImageFile4 { get; set; }
        public IFormFile? ImageFile5 { get; set; }
        public string GridHeading { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

    }

}
