using System;

namespace OnlineShopWebApp.Models
{
    public class ComparisonProduct
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; } //Для дальнейшей привязки пользователя
        public Product Product { get; set; }

        public ComparisonProduct(Guid userId, Product product)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Product = product;
        }
    }
}
