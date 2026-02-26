namespace MakQR.Utils
{
    public static class JsonFilePath
    {
        private const string ContentData = "ContentData";
        public static string HomeEventSectionPath(IWebHostEnvironment env) =>
         Path.Combine(env.ContentRootPath, ContentData, "events.json");

        public static string HomeAboutUsSectionPath(IWebHostEnvironment env) =>
        Path.Combine(env.ContentRootPath, ContentData, "about.json");
        public static string HomeWhyUsSectionPath(IWebHostEnvironment env) =>
        Path.Combine(env.ContentRootPath, ContentData, "why-us.json");

        public static string RoomsViewPath(IWebHostEnvironment env) =>
         Path.Combine(env.ContentRootPath, ContentData, "rooms-view.json");

        public static string RoomsTypePath(IWebHostEnvironment env) =>
         Path.Combine(env.ContentRootPath, ContentData, "rooms.json");

        public static string ReviewFilePath(IWebHostEnvironment env) =>
           Path.Combine(env.ContentRootPath, ContentData, "reviews.json");
        public static string BannerFilePath(IWebHostEnvironment env) =>
          Path.Combine(env.ContentRootPath, ContentData, "banner.json");

        public static string FooterFilePath(IWebHostEnvironment env) =>
         Path.Combine(env.ContentRootPath, ContentData, "footer.json");

        public static string AmenitiesFilePath(IWebHostEnvironment env) =>
         Path.Combine(env.ContentRootPath, ContentData, "amenities.json");

        public static string FacilityGalleryFilePath(IWebHostEnvironment env) =>
           Path.Combine(env.ContentRootPath, ContentData, "facility.json");
        public static string GoogleSettingFilePath(IWebHostEnvironment env) =>
       Path.Combine(env.ContentRootPath, ContentData, "googleSettings.json");

        public static string AdminFilePath(IWebHostEnvironment env) =>
     Path.Combine(env.ContentRootPath, ContentData, "admin.json");
    }
}
