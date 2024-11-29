using System;

namespace OnlineShop.Domain.Models
{
    public class FavoriteProduct
    {
        public Guid Id { get; init; }
        public string UserId { get; init; }
        public Product Product { get; init; }
    }
}
