using System;
using System.Collections.Generic;

namespace OnlineShop.Db.Models
{
    public class Product
    {
        public Guid Id { get; init; }
        public string Name { get; set; }
        public decimal Cost { get; set; }
        public string Description { get; set; }
        public ProductCategories Category { get; set; }
        public string? ImageUrl { get; set; }
        public string SpecificationsJson { get; set; }

        public List<CartPosition> CartPositions { get; set; }
    }
}
