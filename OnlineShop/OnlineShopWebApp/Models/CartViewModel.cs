using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlineShopWebApp.Models
{
    public class CartViewModel
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public List<CartPositionViewModel> Positions { get; set; } = [];
        public decimal TotalCost
        {
            get => Positions?.Sum(p => p.Cost) ?? 0;
        }

        public int TotalQuantity
        {
            get => Positions?.Sum(p => p.Quantity) ?? 0;
        }
    }
}
