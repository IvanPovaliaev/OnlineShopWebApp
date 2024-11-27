using System;

namespace OnlineShop.Domain.Models
{
    public class ComparisonProduct
    {
        public Guid Id { get; init; }
        public string UserId { get; init; }
        public Product Product { get; init; }
    }
}
