using System;

namespace OnlineShopWebApp.Models
{
    public record class ComparisonProductViewModel(Guid UserId, ProductViewModel Product)
    {
        public Guid Id { get; init; }
    }
}
