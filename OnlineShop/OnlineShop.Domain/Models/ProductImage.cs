using System;

namespace OnlineShop.Domain.Models
{
    public class ProductImage
    {
        public Guid Id { get; init; }
        public required string Url { get; init; }
        public required Guid ProductId { get; init; }
    }
}
