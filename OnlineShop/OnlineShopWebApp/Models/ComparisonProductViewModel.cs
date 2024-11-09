using System;

namespace OnlineShopWebApp.Models
{
    public record ComparisonProductViewModel(Guid Id, string UserId, ProductViewModel Product)
    {
    }
}
