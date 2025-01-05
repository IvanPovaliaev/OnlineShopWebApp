using System;

namespace OnlineShop.Infrastructure.ReviewApiService
{
    public class ReviewTokenStorage
    {
        public string? Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}
