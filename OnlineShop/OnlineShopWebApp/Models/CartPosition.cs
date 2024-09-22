using System;

namespace OnlineShopWebApp.Models
{
    public class CartPosition
    {
        public Guid Id { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public decimal Cost
        {
            get => Product.Cost * Quantity;
        }

        public CartPosition(Product product, int quantity)
        {
            Id = Guid.NewGuid();
            Product = product;
            Quantity = quantity;
        }
    }
}
