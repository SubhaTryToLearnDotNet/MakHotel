namespace MakQR.Models.Common
{
    public static class JsonFilePath
    {
        public static string HomeEventSectionPath(IWebHostEnvironment env) =>
         Path.Combine(env.ContentRootPath, "ContentData", "events.json");
    }
}
