using System;

namespace OnlineShop.Application.Models
{
    public class OrderPositionViewModel
    {
        public Guid Id { get; init; }
        public ProductViewModel Product { get; set; }
        public int Quantity { get; set; }
        public decimal Cost
        {
            get => Product.Cost * Quantity;
        }
    }
}
