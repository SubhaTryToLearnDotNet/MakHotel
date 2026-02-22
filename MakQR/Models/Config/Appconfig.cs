namespace MakQR.Models.Config
{
    public class Appconfig
    {
        public AdminConfiguration AdminConfig { get; set; } = new();
    }

    public class AdminConfiguration
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public string DistanceMatrixUrl { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string GeocodeUrl { get; set; } = string.Empty;
    }
}
