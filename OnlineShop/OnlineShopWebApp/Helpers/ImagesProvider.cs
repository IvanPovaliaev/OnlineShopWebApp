using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;

namespace OnlineShopWebApp.Helpers
{
    public class ImagesProvider
    {
        private readonly IWebHostEnvironment _appEnvironment;

        public ImagesProvider(IWebHostEnvironment appEnvironment)
        {
            _appEnvironment = appEnvironment;
        }

        public string? Save(IFormFile? image, string folderPath)
        {
            if (image is null)
            {
                return null;
            }

            var localFolderPath = Path.Combine(_appEnvironment.WebRootPath + folderPath);

            if (!Directory.Exists(localFolderPath))
            {
                Directory.CreateDirectory(localFolderPath);
            }

            var imageType = image.FileName.Split('.')
                                          .Last();

            var imageName = $"{Guid.NewGuid()}.{imageType}";
            var imagePath = Path.Combine(localFolderPath, imageName);

            using (var fileStream = new FileStream(imagePath, FileMode.Create))
            {
                image.CopyTo(fileStream);
            }

            return Path.Combine(folderPath, imageName);
        }
    }
}
