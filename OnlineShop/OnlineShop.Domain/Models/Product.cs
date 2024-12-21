using System;
using System.Collections.Generic;

namespace OnlineShop.Domain.Models
{
    public class Product
    {
        public Guid Id { get; init; }
        public string Name { get; set; }
        public decimal Cost { get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; init; } = DateTime.Now.ToUniversalTime();
        public ProductCategories Category { get; set; }
        public List<ProductImage> Images { get; set; } = [];
        public string SpecificationsJson { get; set; }
    }
}
