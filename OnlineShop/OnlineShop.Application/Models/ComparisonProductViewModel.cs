using System;

namespace OnlineShop.Application.Models
{
    public record ComparisonProductViewModel(Guid Id, string UserId, ProductViewModel Product)
    {
    }
}
