using System;

namespace OnlineShop.Db.Models
{
    public class CartPosition
    {
        public Guid Id { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public Cart Cart { get; set; }

        public CartPosition() { }

        public CartPosition(Product product, Cart cart)
        {
            Product = product;
            Quantity = 1;
            Cart = cart;
        }
    }
}
