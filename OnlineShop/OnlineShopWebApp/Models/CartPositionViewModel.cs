﻿using System;

namespace OnlineShopWebApp.Models
{
    public class CartPositionViewModel
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
