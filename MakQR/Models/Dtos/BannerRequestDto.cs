namespace MakQR.Models.Dtos
{
    public class BannerRequestDto
    {
        public string? Id { get; set; }

        public IFormFile? ImageFile1 { get; set; }
        public string TitleHtml1 { get; set; } = string.Empty;
        public string Subtitle1 { get; set; } = string.Empty;
        public string ButtonText1 { get; set; } = string.Empty;
        public string ButtonUrl1 { get; set; } = string.Empty;
        public IFormFile? ImageFile2 { get; set; }
        public string TitleHtml2 { get; set; } = string.Empty;
        public string Subtitle2 { get; set; } = string.Empty;
        public string ButtonText2 { get; set; } = string.Empty;
        public string ButtonUrl2 { get; set; } = string.Empty;

        public IFormFile? ImageFile3 { get; set; }
        public string TitleHtml3 { get; set; } = string.Empty;
        public string Subtitle3 { get; set; } = string.Empty;
        public string ButtonText3 { get; set; } = string.Empty;
        public string ButtonUrl3 { get; set; } = string.Empty;

        public IFormFile? ImageFile4 { get; set; }
        public string TitleHtml4 { get; set; } = string.Empty;
        public string Subtitle4 { get; set; } = string.Empty;
        public string ButtonText4 { get; set; } = string.Empty;
        public string ButtonUrl4 { get; set; } = string.Empty;


    }
}
