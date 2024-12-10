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

        /// <summary>
        /// Timeout in milliseconds. Default value is equal 5 seconds.
        /// </summary>
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Timeout must be greater than 0")]
        public required int Timeout { get; init; } = 5000;
    }
}
