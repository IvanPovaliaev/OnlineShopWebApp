using Newtonsoft.Json;
using System;

namespace OnlineShopWebApp.Models
{
    public record class FavoriteProduct(Guid UserId, Product Product)
    {
        public Guid Id { get; init; } = new Guid();

        [JsonConstructor]
        public FavoriteProduct(Guid id, Guid userId, Product product) : this(userId, product)
        {
            Id = id;
        }
    }
}
