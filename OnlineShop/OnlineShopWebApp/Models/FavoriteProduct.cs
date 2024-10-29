using System;

namespace OnlineShopWebApp.Models
{
    public record class FavoriteProduct(Guid UserId, ProductViewModel Product)
    {
        public Guid Id { get; init; } = Guid.NewGuid();
    }
}
