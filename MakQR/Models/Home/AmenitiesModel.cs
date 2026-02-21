namespace MakQR.Models.Home
{
    public class AmenitiesModel
    {
        public string Title { get; set; } =  string.Empty;
        public string SubTitle { get; set; } = string.Empty;
        public List<AmenityItem> Amenities { get; set; } = new();
    }

    public class AmenityItem
    {
        public string Icon { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;   
    }
}
