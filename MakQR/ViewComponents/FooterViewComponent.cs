using MakQR.Models.Home;
using MakQR.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace MakQR.ViewComponents
{
    public class FooterViewComponent : ViewComponent
    {
        private readonly IWebHostEnvironment _env;

        public FooterViewComponent(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
           string path =  JsonFilePath.FooterFilePath(_env);

            if (!System.IO.File.Exists(path))
                return View(new FooterSection());

            var json = await System.IO.File.ReadAllTextAsync(path);
            var data = JsonSerializer.Deserialize<FooterSection>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new FooterSection();
            return View(data);
        }
    }
}
