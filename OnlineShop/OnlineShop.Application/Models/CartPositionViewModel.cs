using System;

namespace OnlineShop.Application.Models
{
    public class CartPositionViewModel
    {
        public Guid Id { get; init; }
        public ProductViewModel Product { get; init; }
        public int Quantity { get; set; }
        public decimal Cost
        {
            get => Product.Cost * Quantity;
        }
    }
}
