using System.ComponentModel.DataAnnotations;

namespace MakQR.Models.Admin
{
    public class HotelSectionViewModel
    {
        [Required]
        [StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public IFormFile? Image1 { get; set; }

        [Required]
        public IFormFile? Image2 { get; set; }

        [Required]
        public IFormFile? Image3 { get; set; }

        [Required]
        public IFormFile? Image4 { get; set; }

        [Required]
        public IFormFile? Image5 { get; set; }

        [Required]
        public IFormFile? Image6 { get; set; }
    }
}
