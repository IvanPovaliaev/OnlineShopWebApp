using System;

namespace OnlineShop.Db.Models
{
    public record class ComparisonProduct(Guid UserId, Product Product)
    {
        public Guid Id { get; init; }
    }
}
