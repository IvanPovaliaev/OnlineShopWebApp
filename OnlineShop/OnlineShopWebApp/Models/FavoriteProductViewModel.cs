using System;

namespace OnlineShopWebApp.Models
{
    public record class FavoriteProductViewModel(Guid Id, string UserId, ProductViewModel Product)
    {
    }
}
