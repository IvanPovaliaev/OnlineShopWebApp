using System;

namespace OnlineShop.Application.Models
{
    public record class FavoriteProductViewModel(Guid Id, string UserId, ProductViewModel Product)
    {
    }
}
