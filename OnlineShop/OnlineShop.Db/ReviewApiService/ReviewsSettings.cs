using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Infrastructure.ReviewApiService
{
    public class ReviewsSettings
    {
        [Required]
        public required string Url { get; init; }

        [Required]
        public required string Login { get; init; }

        [Required]
        public required string Password { get; init; }
    }
}
