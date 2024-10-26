using System;

namespace OnlineShopWebApp.Models
{
    public record ComparisonProductViewModel(Guid Id, Guid UserId, ProductViewModel Product)
    {
    }
}
