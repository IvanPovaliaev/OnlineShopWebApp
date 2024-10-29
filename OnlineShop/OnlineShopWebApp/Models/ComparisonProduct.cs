using System;

namespace OnlineShopWebApp.Models
{
    public record class ComparisonProduct(Guid UserId, ProductViewModel Product)
    {
        public Guid Id { get; init; } = Guid.NewGuid();
    }
}
