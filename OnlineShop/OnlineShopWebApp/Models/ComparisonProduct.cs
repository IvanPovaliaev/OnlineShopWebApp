using Newtonsoft.Json;
using System;

namespace OnlineShopWebApp.Models
{
    public record class ComparisonProduct(Guid UserId, Product Product)
    {
        public Guid Id { get; init; } = Guid.NewGuid();

        [JsonConstructor]
        public ComparisonProduct(Guid id, Guid userId, Product product) : this(userId, product)
        {
            Id = id;
        }
    }
}
