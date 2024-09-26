using System;

namespace OnlineShopWebApp.Models
{
    public class FavoriteProduct
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; } //Для дальнейшей привязки пользователя
        public Product Product { get; set; }

        public FavoriteProduct(Guid userId, Product product)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Product = product;
        }
    }
}
