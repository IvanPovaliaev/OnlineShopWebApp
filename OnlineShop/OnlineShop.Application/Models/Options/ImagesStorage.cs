using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Application.Models.Options
{
    public class ImagesStorage
    {
        [Required]
        public string ProductsPath { get; init; } = string.Empty;

        [Required]
        public string AvatarsPath { get; init; } = string.Empty;
    }
}
