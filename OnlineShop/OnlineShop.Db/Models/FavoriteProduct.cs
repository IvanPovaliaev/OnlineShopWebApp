﻿using System;

namespace OnlineShop.Db.Models
{
    public class FavoriteProduct
    {
        public Guid Id { get; init; }
        public Guid UserId { get; init; }
        public Product Product { get; init; }

        public FavoriteProduct() { }

        public FavoriteProduct(Guid userId, Product product)
        {
            UserId = userId;
            Product = product;
        }
    }
}
