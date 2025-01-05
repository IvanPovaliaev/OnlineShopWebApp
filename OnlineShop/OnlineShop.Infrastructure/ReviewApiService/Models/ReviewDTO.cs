using System;

namespace OnlineShop.Infrastructure.ReviewApiService.Models
{
    public class ReviewDTO
    {
        public int Id { get; init; }

        public Guid ProductId { get; init; }

        public string UserId { get; init; }

        public string? Text { get; init; }

        public int Grade { get; init; }

        public DateTime CreationDate { get; init; }

        public int RatingId { get; init; }

        public ReviewStatus Status { get; init; }
    }
}
