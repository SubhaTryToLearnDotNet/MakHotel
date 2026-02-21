namespace MakQR.Utils
{
    public static class AppHelper
    {
        public static async Task ReplaceIfUploaded(IFormFile? file, string fixedName, string folderName)
        {
            if (file == null) return;

            var path = Path.Combine(folderName, fixedName);

            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);

            using var stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream);
        }
    }
}
