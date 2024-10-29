using System;

namespace OnlineShopWebApp.Models
{
    public record class FavoriteProductViewModel(Guid Id, Guid UserId, ProductViewModel Product)
    {
    }
}
