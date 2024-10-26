using System;

namespace OnlineShopWebApp.Models
{
    public record class FavoriteProductViewModel(Guid UserId, ProductViewModel Product)
    {
        public Guid Id { get; init; }
    }
}
