using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OnlineShopWebApp.Helpers
{
    public class ImagesProvider(IWebHostEnvironment appEnvironment)
    {
        private readonly IWebHostEnvironment _appEnvironment = appEnvironment;

        /// <summary>
        /// Save target image to root folder path
        /// </summary>        
        /// <returns>Image url</returns>
        /// <param name="image">Target image</param>
        /// <param name="folderPath">Target folderPath</param>
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

        /// <summary>
        /// Save target images to root folder path
        /// </summary>        
        /// <returns>Collections of images urls</returns>
        /// <param name="images">Target images collection</param>
        /// <param name="folderPath">Target folderPath</param>
        public List<string> SaveAll(ICollection<IFormFile>? images, string folderPath)
        {
            if (images is null)
            {
                return [];
            }

            var imagesUrls = new List<string>(images.Count);
            var localFolderPath = Path.Combine(_appEnvironment.WebRootPath + folderPath);

            if (!Directory.Exists(localFolderPath))
            {
                Directory.CreateDirectory(localFolderPath);
            }

            foreach (var image in images)
            {
                var imageType = image.FileName.Split('.')
                                              .Last();

                var imageName = $"{Guid.NewGuid()}.{imageType}";
                var localImagePath = Path.Combine(localFolderPath, imageName);

                using (var fileStream = new FileStream(localImagePath, FileMode.Create))
                {
                    image.CopyTo(fileStream);
                }

                var imagePath = Path.Combine(folderPath, imageName);
                imagesUrls.Add(imagePath);
            }
            return imagesUrls;
        }
    }
}
