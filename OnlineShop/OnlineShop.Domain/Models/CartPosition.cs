using System;

namespace OnlineShop.Domain.Models
{
    public class CartPosition
    {
        public Guid Id { get; init; }
        public Product Product { get; init; }
        public int Quantity { get; set; }
        public Guid CartId { get; set; }
    }
}
