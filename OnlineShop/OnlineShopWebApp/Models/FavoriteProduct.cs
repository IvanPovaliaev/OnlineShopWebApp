using System;

namespace OnlineShopWebApp.Models
{
    public record class FavoriteProduct(Guid UserId, Product Product)
    {
        public Guid Id { get; init; } = Guid.NewGuid();
    }
}
