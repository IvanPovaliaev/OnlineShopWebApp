using System;

namespace OnlineShop.Domain.Models
{
    public class OrderPosition
    {
        public Guid Id { get; init; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public Guid OrderId { get; init; }
    }
}
