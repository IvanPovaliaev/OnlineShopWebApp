using System;

namespace OnlineShop.Db.Models
{
    public class CartPosition
    {
        public Guid Id { get; set; }

        //Связь 1 к 1
        public Product Product { get; set; }

        public int Quantity { get; set; }

        public CartPosition() { }

        public CartPosition(Product product)
        {
            Product = product;
            Quantity = 1;
        }
    }
}
